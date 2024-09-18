using AutoMapper;
using DocumentManagementSystem.Business.Interfaces;
using DocumentManagementSystem.Common.Enums;
using DocumentManagementSystem.Dtos;
using DocumentManagementSystem.UI.Extensions;
using DocumentManagementSystem.UI.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DocumentManagementSystem.DataAccess.Contexts;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using DocumentManagementSystem.Business.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace DocumentManagementSystem.UI.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IDeparmentService _deparmentService;
        private readonly IValidator<UserCreateModel> _userCreateValidator;
        private readonly IAppUserService _appUserService;
        private readonly IMapper _mapper;
        private readonly DocumentContext _context;

        public AccountController(
            IWebHostEnvironment hostingEnvironment,
            IDeparmentService deparmentService,
            IValidator<UserCreateModel> userCreateValidator,
            IAppUserService appUserService,
            IMapper mapper,
            DocumentContext context,
            IAnnouncementService announcementService) // Pass IAnnouncementService here
            : base(announcementService) // Pass the service to the base controller
        {
            _hostingEnvironment = hostingEnvironment;
            _deparmentService = deparmentService;
            _userCreateValidator = userCreateValidator;
            _appUserService = appUserService;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IActionResult> SignUp()
        {
            var response = await _deparmentService.GetAllAsync();
            var model = new UserCreateModel
            {
                Departments = new SelectList(response.Data, "Id", "Definition")
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserCreateModel model)
        {
            var result = _userCreateValidator.Validate(model);
            if (result.IsValid)
            {
                var dto = _mapper.Map<AppUserCreateDto>(model);
                var createResponse = await _appUserService.CreateWithRoleAsync(dto, (int)RoleType.Member);
                return this.ResponseRedirectAction(createResponse, "SignIn");

            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            }
            var response = await _deparmentService.GetAllAsync();
            model.Departments = new SelectList(response.Data, "Id", "Definition", model.DeparmentId);
            return View(model);
        }

        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(AppUserLoginDto dto)
        {
            var result = await _appUserService.CheckUserAsync(dto);
            if (result.ResponseType == Common.ResponseType.Success)
            {
                var roleResult = await _appUserService.GetRolesByUserIdAsync(result.Data.Id);
                var claims = new List<Claim>();
                if (roleResult.ResponseType == Common.ResponseType.Success)
                {
                    foreach (var role in roleResult.Data)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Definition));
                    }
                }
                claims.Add(new Claim(ClaimTypes.NameIdentifier, result.Data.Id.ToString()));

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = dto.RememberMe
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn", "Account");
        }

        public async Task<IActionResult> Detail()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.AppUsers.FirstOrDefault(x => x.Id == userId);

            if (user != null)
            {
                var userDto = _mapper.Map<AppUserUpdateDto>(user);
                var model = _mapper.Map<UserUpdateModel>(userDto);

                // Make sure to set ImagePath from the user's database record
                model.ImagePath = user.ImagePath; // The ImagePath should be coming from your database (something like "images/example.jpg")

                // Load departments (for dropdown)
                var response = await _deparmentService.GetAllAsync();
                model.Departments = new SelectList(response.Data, "Id", "Definition");

                return View(model); // Pass the updated model with ImagePath to the view
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Detail(UserUpdateModel model, IFormFile profileImage)
        {
            if (ModelState.IsValid)
            {
                if (profileImage != null && profileImage.Length > 0)
                {
                    // Get the file name
                    var fileName = Path.GetFileName(profileImage.FileName);

                    // Set the path to save the file in the wwwroot/images folder
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the file to the wwwroot/images folder
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profileImage.CopyToAsync(fileStream);
                    }

                    // Log the file path for debugging
                    Console.WriteLine("File saved to: " + filePath);

                    // Set the relative image path
                    model.ImagePath = "images/" + fileName;

                    // Save the image path to the database
                    await UpdateUserAndImagePath(model);

                    // After uploading, you should redirect to refresh the page so that the image gets displayed
                    return RedirectToAction("Detail");
                }
            }

            // Reload departments in case of invalid submission
            var response = await _deparmentService.GetAllAsync();
            model.Departments = new SelectList(response.Data, "Id", "Definition");

            return View(model);
        }

        private async Task UpdateUserAndImagePath(UserUpdateModel model)
        {
            // Fetch the user from the database by their ID
            var user = _context.AppUsers.FirstOrDefault(x => x.Id == model.Id);

            if (user != null)
            {
                // Update the user's ImagePath
                user.ImagePath = model.ImagePath;

                // Save changes to the database
                _context.AppUsers.Update(user);
                await _context.SaveChangesAsync();
            }
        }




    }
}
