using AutoMapper;
using DocumentManagementSystem.Business.Interfaces;
using DocumentManagementSystem.Business.Services;
using DocumentManagementSystem.Common;
using DocumentManagementSystem.DataAccess.Contexts;
using DocumentManagementSystem.Dtos;
using DocumentManagementSystem.Dtos.DocumentDtos;
using DocumentManagementSystem.Entities;
using DocumentManagementSystem.UI.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace DocumentManagementSystem.UI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IAnnouncementService _announcementService;
        private readonly IAppUserService _appUserService;
        private readonly IValidator<DocumentCreateDto> _documentCreateValidator;
        private readonly DocumentContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;




        public HomeController(
            IDocumentService documentService,
            IAnnouncementService announcementService,
            IAppUserService appUserService,
            IValidator<DocumentCreateDto> documentCreateValidator,
            DocumentContext context,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)

        {
            _documentService = documentService;
            _announcementService = announcementService;
            _appUserService = appUserService;
            _documentCreateValidator = documentCreateValidator;
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index([FromQuery] string search, [FromQuery] string searchopt, [FromQuery] string sortOption)
        {
            // Retrieve the roles of the current user
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            // Retrieve the current user's DeparmentId (you can fetch this from the claims or from the database)
            // Assuming DepartmentId is stored in the user's claims
            int? userDepartmentId = null;

            // Example: If DepartmentId is stored as a claim
            var departmentClaim = User.FindFirst("DeparmentId");
            if (departmentClaim != null)
            {
                userDepartmentId = int.Parse(departmentClaim.Value);
            }
            else
            {
                // Fetch user data from IAppUserService instead of identityContext or DocumentContext
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Assuming the user ID is in the claims
                if (!string.IsNullOrEmpty(userId))
                {
                    // Use IAppUserService to get the user's DepartmentId
                    var userResponse = await _appUserService.GetUserByIdAsync(int.Parse(userId));
                    if (userResponse.ResponseType == ResponseType.Success && userResponse.Data != null)
                    {
                        userDepartmentId = userResponse.Data.DeparmentId;
                    }
                }
            }

            // If DeparmentId is still null, return a BadRequest
            if (!userDepartmentId.HasValue)
            {
                return BadRequest("Department information not found for the current user.");
            }

            // Load announcements
            var announcements = await _announcementService.GetAllAsync();
            ViewData["Announcements"] = announcements;

            var docsQuery = from doc in _context.Documents
                            join dep in _context.Departments on doc.DepId equals dep.Id
                            select new
                            {
                                doc.Id,
                                doc.Title,
                                doc.TypeOfDoc,
                                doc.SenderName,
                                doc.ReceiverName,
                                doc.ClassOfDoc,
                                doc.DocStatus,
                                doc.DocState,
                                doc.SendDate,
                                DepartmentDefinition = dep.Definition,
                                doc.DepId
                            };

            // If the user is NOT an Admin or Moderator, apply the Department filter
            if (!userRoles.Contains("Admin") && !userRoles.Contains("Moderator"))
            {
                docsQuery = docsQuery.Where(x => x.DepId == userDepartmentId.Value);
            }

            // Filter by search option
            if (!String.IsNullOrEmpty(search))
            {
                if (!int.TryParse(searchopt, out int selectedOpt))
                {
                    selectedOpt = 0; // Default to searching by Title
                }

                search = search.ToLower(); // Convert the search term to lowercase

                docsQuery = selectedOpt switch
                {
                    // Sender
                    1 => docsQuery.Where(x => x.SenderName != null && x.SenderName.ToLower().Contains(search)),
                    // Receiver
                    2 => docsQuery.Where(x => x.ReceiverName != null && x.ReceiverName.ToLower().Contains(search)),
                    // Type of Doc
                    3 => docsQuery.Where(x => x.TypeOfDoc != null && x.TypeOfDoc.ToLower().Contains(search)),
                    // Class of Doc
                    4 => docsQuery.Where(x => x.ClassOfDoc != null && x.ClassOfDoc.ToLower().Contains(search)),
                    // Department
                    5 => docsQuery.Where(x => x.DepartmentDefinition != null && x.DepartmentDefinition.ToLower().Contains(search)),
                    // State
                    6 => docsQuery.Where(x => x.DocState.ToString().ToLower().Contains(search)),
                    // Title
                    _ => docsQuery.Where(x => x.Title != null && x.Title.ToLower().Contains(search)),
                };
            }


            // Sort the results
            docsQuery = sortOption switch
            {
                "title" => docsQuery.OrderBy(x => x.Title),
                "type" => docsQuery.OrderBy(x => x.TypeOfDoc),
                "status" => docsQuery.OrderBy(x => x.DocStatus),
                "state" => docsQuery.OrderBy(x => x.DocState),
                "senddate" => docsQuery.OrderByDescending(x => x.SendDate),
                _ => docsQuery.OrderBy(x => x.Id),
            };
            var docs = await docsQuery.ToListAsync();
            var dtos = docs.Select(x => new DocumentListDto
            {
                Id = x.Id,
                Title = x.Title,
                TypeOfDoc = x.TypeOfDoc,
                SenderName = x.SenderName,
                ClassOfDoc = x.ClassOfDoc,
                ReceiverName = x.ReceiverName,
                DocStatus = x.DocStatus,
                DocState = x.DocState,
                SendDate = x.SendDate,
                DepartmentDefinition = x.DepartmentDefinition
            }).ToList();

            return View("Index", dtos);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _documentService.RemoveAsync(id);
            if (result.ResponseType == Common.ResponseType.Success)
            {
                return this.ResponseRedirectAction(result, "Index");
            }
            else
            {
                return View("Index");
            }
        }
        public IActionResult Create()
        {
            var departments = _context.Departments.Select(d => new { d.Id, d.Definition }).ToList();
            ViewBag.Departments = new SelectList(departments, "Id", "Definition");
            var model = new DocumentCreateDto();

            return View("Create", model);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] DocumentCreateDto model, IFormFile file)
        {
            var result = _documentCreateValidator.Validate(model);
            if (result.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        string fileUploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "fileUpload");

                        if (!Directory.Exists(fileUploadFolder))
                        {
                            Directory.CreateDirectory(fileUploadFolder);
                            Console.WriteLine("Created directory: " + fileUploadFolder);
                        }
                        else
                        {
                            Console.WriteLine("Directory already exists: " + fileUploadFolder);
                        }

                        string userProvidedFileName = model.Title;
                        userProvidedFileName = Path.GetFileNameWithoutExtension(userProvidedFileName);

                        string fileExtension = Path.GetExtension(file.FileName);
                        string finalFileName = userProvidedFileName + fileExtension;
                        string filePath = Path.Combine(fileUploadFolder, finalFileName);
                        if (System.IO.File.Exists(filePath))
                        {
                            finalFileName = userProvidedFileName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + fileExtension;
                            filePath = Path.Combine(fileUploadFolder, finalFileName);
                        }
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            Console.WriteLine("File uploaded successfully to: " + filePath);
                        }
                        model.FileName = finalFileName;
                        model.FilePath = Path.Combine("fileUpload", finalFileName).Replace("\\", "/");

                        Console.WriteLine("FileName stored in database: " + model.FileName);
                        Console.WriteLine("FilePath stored in database: " + model.FilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception during file upload: " + ex.Message);
                        ModelState.AddModelError("", "File upload failed: " + ex.Message);
                        return View(model);
                    }
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.AppUserId = int.Parse(userId);
                var createResponse = await _documentService.CreateAsync(model);

                return this.ResponseRedirectAction(createResponse, "Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult DownloadFile(string fileName)
        {
            string fileUploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "fileUpload");
            string filePath = Path.Combine(fileUploadFolder, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
            if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
            {
                return BadRequest("Image files cannot be downloaded.");
            }
            var fileContentType = "application/octet-stream";
            if (fileExtension == ".pdf") fileContentType = "application/pdf";
            else if (fileExtension == ".txt") fileContentType = "text/plain";
            else if (fileExtension == ".doc" || fileExtension == ".docx") fileContentType = "application/msword";

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, fileContentType, fileName);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var response = await _documentService.GetByIdAsync<DocumentUpdateDto>(id);
            var departments = _context.Departments.Select(d => new { d.Id, d.Definition }).ToList();
            ViewBag.Departments = new SelectList(departments, "Id", "Definition");

            return this.ResponseView(response);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DocumentUpdateDto dto, IFormFile uploadedFile)
        {
            if (dto.DocStatus == DocStatus.Done)
            {
                dto.ReceiveDate = DateTime.Now;
            }

            // Handle the uploaded file if it exists.
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/fileUpload");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + uploadedFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                // Use the original file name for the FileName property
                dto.FileName = uploadedFile.FileName;
                dto.FilePath = $"/fileUpload/{uniqueFileName}";
            }

            var response = await _documentService.UpdateAsync(dto);

            return this.ResponseRedirectAction(response, "Index");
        }

        [HttpGet]
        public async Task<IActionResult> Remarks(int id)
        {
            var response = await _documentService.GetByIdAsync<DocumentRemarksDto>(id);
            if (response.ResponseType != ResponseType.Success || response.Data == null)
            {
                return NotFound();
            }

            var departments = _context.Departments.Select(d => new { d.Id, d.Definition }).ToList();
            ViewBag.Departments = new SelectList(departments, "Id", "Definition");

            var docStatusList = Enum.GetValues(typeof(DocStatus))
                .Cast<DocStatus>()
                .Select(d => new SelectListItem
                {
                    Value = ((int)d).ToString(),
                    Text = d.ToString()
                }).ToList();

            ViewBag.DocStatusList = new SelectList(docStatusList, "Value", "Text", response.Data.DocStatus);

            return View("Remarks", response.Data);
        }


        [HttpPost]
        public async Task<IActionResult> Remarks(DocumentRemarksDto dto, IFormFile uploadedFile)
        {
            // Retrieve existing document
            var existingDocumentResponse = await _documentService.GetByIdAsync<DocumentUpdateDto>(dto.Id);

            if (existingDocumentResponse.ResponseType != ResponseType.Success || existingDocumentResponse.Data == null)
            {
                // Handle not found
                return NotFound();
            }

            var existingDocument = existingDocumentResponse.Data;

            // Update only the fields that are allowed to be edited
            existingDocument.Description = dto.Description;
            existingDocument.DocStatus = dto.DocStatus;
            existingDocument.DepId = dto.DepId;

            if (dto.DocStatus == DocStatus.Done)
            {
                existingDocument.ReceiveDate = DateTime.Now;
            }

            // Handle the uploaded file if it exists.
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "fileUpload");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + uploadedFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                existingDocument.FileName = uploadedFile.FileName;
                existingDocument.FilePath = $"/fileUpload/{uniqueFileName}";
            }

            var response = await _documentService.UpdateAsync(existingDocument);

            return this.ResponseRedirectAction(response, "Index");
        }

    public async Task<IActionResult> Detail(int id)
        {
            var documentResponse = await _documentService.GetByIdAsync<DocumentUpdateDto>(id);

            if (documentResponse == null || documentResponse.ResponseType == ResponseType.NotFound)
            {
                return this.ResponseView(documentResponse);
            }

            var departmentDefinition = await _context.Departments
                .Where(d => d.Id == documentResponse.Data.DepId)
                .Select(d => d.Definition)
                .FirstOrDefaultAsync();

            documentResponse.Data.DepartmentDefinition = departmentDefinition;
            return this.ResponseView(documentResponse);
        }



    }
}