// File: DocumentManagementSystem.Business/Services/AnnouncementService.cs
using DocumentManagementSystem.Business.Interfaces;
using DocumentManagementSystem.DataAccess.Contexts;
using DocumentManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Business.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly DocumentContext _context;

        public AnnouncementService(DocumentContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Announcement>> GetAllAsync()
        {
            return await _context.Announcements.ToListAsync();
        }

        public async Task<Announcement> GetByIdAsync(int id)
        {
            return await _context.Announcements.FindAsync(id);
        }

        public async Task AddAsync(Announcement announcement)
        {
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Announcement announcement)
        {
            _context.Announcements.Update(announcement);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var announcement = await GetByIdAsync(id);
            if (announcement != null)
            {
                _context.Announcements.Remove(announcement);
                await _context.SaveChangesAsync();
            }
        }
    }
}

