using AutoMapper;
using DocumentManagementSystem.Business.Interfaces;
using DocumentManagementSystem.Common.Enums;
using DocumentManagementSystem.DataAccess.Contexts;
using DocumentManagementSystem.Dtos;
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

            // Initialize query
            IQueryable<Document> docsQuery = _context.Document;

            // Apply search filters
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

            // Apply sorting
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
                    docsQuery = docsQuery.OrderBy(x => x.Id); // Default sorting
                    break;
            }

            // Execute query and map to DTOs
            var docs = await docsQuery.ToListAsync();
            var dtos = _mapper.Map<List<DocumentListDto>>(docs);

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
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "Definition");  // Ensure departments are fetched from DB
            var departments = _context.Departments.Select(d => new { d.Id, d.Definition }).ToList();
            ViewBag.Department = new SelectList(departments, "Id", "Definition");
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
            var response = await _documentService.GetByIdAsync<DocumentUpdateDto>(id);
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
            var response = await _documentService.GetByIdAsync<DocumentUpdateDto>(id);
            return this.ResponseView(response);
        }
    }
}
