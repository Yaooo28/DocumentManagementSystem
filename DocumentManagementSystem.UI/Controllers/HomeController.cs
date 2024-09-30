using AutoMapper;
using DocumentManagementSystem.Business.Interfaces;
using DocumentManagementSystem.Common;
using DocumentManagementSystem.DataAccess.Contexts;
using DocumentManagementSystem.Dtos;
using DocumentManagementSystem.Dtos.DocumentDtos;
using DocumentManagementSystem.Entities;
using DocumentManagementSystem.UI.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        private readonly IValidator<DocumentCreateDto> _documentCreateValidator;
        private readonly DocumentContext _context;
        private readonly IMapper _mapper;

        public HomeController(
            IDocumentService documentService,
            IAnnouncementService announcementService,
            IValidator<DocumentCreateDto> documentCreateValidator,
            DocumentContext context,
            IMapper mapper)
        {
            _documentService = documentService;
            _announcementService = announcementService;
            _documentCreateValidator = documentCreateValidator;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index([FromQuery] string search, [FromQuery] string searchopt, [FromQuery] string sortOption)
        {
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
                                DepartmentDefinition = dep.Definition
                            };

            if (!String.IsNullOrEmpty(search))
            {
                int selectedOpt = int.Parse(searchopt ?? "0");

                switch (selectedOpt)
                {
                    case 1:
                        docsQuery = docsQuery.Where(x => x.SenderName.Contains(search));
                        break;
                    case 2:
                        docsQuery = docsQuery.Where(x => x.ReceiverName.Contains(search));
                        break;
                    case 3:
                        docsQuery = docsQuery.Where(x => x.TypeOfDoc.Contains(search));
                        break;
                    case 4:
                        docsQuery = docsQuery.Where(x => x.ClassOfDoc.Contains(search));
                        break;
                    default:
                        docsQuery = docsQuery.Where(x => x.Title.Contains(search));
                        break;
                }
            }

            switch (sortOption)
            {
                case "title":
                    docsQuery = docsQuery.OrderBy(x => x.Title);
                    break;
                case "type":
                    docsQuery = docsQuery.OrderBy(x => x.TypeOfDoc);
                    break;
                case "status":
                    docsQuery = docsQuery.OrderBy(x => x.DocStatus);
                    break;
                case "state":
                    docsQuery = docsQuery.OrderBy(x => x.DocState);
                    break;
                case "senddate":
                    docsQuery = docsQuery.OrderByDescending(x => x.SendDate);
                    break;
                default:
                    docsQuery = docsQuery.OrderBy(x => x.Id);
                    break;
            }

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



        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public async Task<IActionResult> Create(DocumentCreateDto model)
        {


            var result = _documentCreateValidator.Validate(model);
            if (result.IsValid)
            {
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

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Edit(int id)
        {
            // Fetch the document to be edited
            var response = await _documentService.GetByIdAsync<DocumentUpdateDto>(id);

            // Fetch departments and pass them to ViewBag for the select dropdown
            var departments = _context.Departments.Select(d => new { d.Id, d.Definition }).ToList();
            ViewBag.Departments = new SelectList(departments, "Id", "Definition");

            return this.ResponseView(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Edit(DocumentUpdateDto dto)
        {
            if (dto.DocStatus == DocStatus.Delivered)
            {
                dto.ReceiveDate = DateTime.Now;
            }
            var response = await _documentService.UpdateAsync(dto);
            return this.ResponseRedirectAction(response, "Index");
        }

        public async Task<IActionResult> Detail(int id)
        {
            // Fetch the document details from the service
            var documentResponse = await _documentService.GetByIdAsync<DocumentUpdateDto>(id);
                

            // Check if the document is found
            if (documentResponse == null || documentResponse.ResponseType == ResponseType.NotFound)
            {
                return this.ResponseView(documentResponse);
            }

            // Fetch the department definition based on DepId from the document
            var departmentDefinition = await _context.Departments
                .Where(d => d.Id == documentResponse.Data.DepId)  // Access the DepId from the document response
                .Select(d => d.Definition)
                .FirstOrDefaultAsync();

            // Append the department definition to the DocumentUpdateDto
            documentResponse.Data.DepartmentDefinition = departmentDefinition;

            // Return the document details view with the department definition included
            return this.ResponseView(documentResponse);  // Using your existing ResponseView extension method
        }
        


    }
}