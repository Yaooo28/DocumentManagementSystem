using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DocumentManagementSystem.Business.Interfaces;
using System.Threading.Tasks;

namespace DocumentManagementSystem.UI.Controllers
{
    public class BaseController : Controller
    {
        private readonly IAnnouncementService _announcementService;

        public BaseController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Fetch announcements asynchronously only if the user is authenticated
                var announcements = await _announcementService.GetAllAsync();
                ViewData["Announcements"] = announcements;
            }
            else
            {
                ViewData["Announcements"] = null; // Ensure announcements are null when not authenticated
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
