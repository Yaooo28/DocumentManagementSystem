using DocumentManagementSystem.Business.Interfaces;
using DocumentManagementSystem.Entities; // Make sure to include the correct namespace for the Announcement entity
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DocumentManagementSystem.UI.Controllers // Updated to the correct namespace for controllers
{
    [Authorize(Roles = "Admin")]
    public class AnnouncementController : BaseController
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementController(IAnnouncementService announcementService)
            : base(announcementService) // Pass the service to the base controller
        {
            _announcementService = announcementService;
        }

        // Action to list all announcements
        public async Task<IActionResult> Index()
        {
            var announcements = await _announcementService.GetAllAsync();
            return View(announcements);
        }

        // Action to render the create announcement form
        public IActionResult Create()
        {
            return View(new Announcement());
        }

        // Action to handle form submission for creating a new announcement
        [HttpPost]
        public async Task<IActionResult> Create(Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                await _announcementService.AddAsync(announcement);
                return RedirectToAction(nameof(Index));
            }
            return View(announcement);
        }

        // Action to display the edit form for an announcement
        public async Task<IActionResult> Edit(int id)
        {
            var announcement = await _announcementService.GetByIdAsync(id);
            if (announcement == null)
                return NotFound();

            return View(announcement);
        }

        // Action to handle the edit form submission
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Announcement announcement)
        {
            if (id != announcement.Id)
                return BadRequest(); // Ensure the ID matches the announcement being edited

            if (ModelState.IsValid)
            {
                var existingAnnouncement = await _announcementService.GetByIdAsync(id);
                if (existingAnnouncement == null)
                    return NotFound();

                // Update fields here if necessary, e.g., manually assign fields to prevent overposting.
                existingAnnouncement.Title = announcement.Title;
                existingAnnouncement.Content = announcement.Content;
                existingAnnouncement.DateCreated = announcement.DateCreated;
                // Add more fields as needed

                await _announcementService.UpdateAsync(existingAnnouncement);
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, return to the edit view with the current announcement data
            return View(announcement);
        }

        // Action to display the delete confirmation for an announcement
        public async Task<IActionResult> Delete(int id)
        {
            var announcement = await _announcementService.GetByIdAsync(id);
            if (announcement == null) return NotFound();
            return View(announcement);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _announcementService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


    }
}
