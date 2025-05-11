using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OurDiaryAPI.Data;
using OurDiaryAPI.DTOs;
using OurDiaryAPI.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OurDiaryAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/entry")]
    public class DiaryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiaryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEntry([FromBody] DiaryEntryDto dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null) return Unauthorized("User not found.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized("User not found.");

            // Check if the user has already made an entry today
            var today = DateTime.UtcNow.Date;

            var existingEntry = await _context.DiaryEntries
                .FirstOrDefaultAsync(e => e.UserId == user.Id && e.Date.Date == today);

            if (existingEntry != null)
                return BadRequest("You have already made an entry today.");

            // Create the new entry
            var newEntry = new DiaryEntry
            {
                UserId = user.Id,
                Date = today,
                Message = dto.Note,
                ImageUrl = dto.ImageUrl
            };

            _context.DiaryEntries.Add(newEntry);
            await _context.SaveChangesAsync();

            return Ok("Entry created successfully.");
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetTodayEntry()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null) return Unauthorized("User not found.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized("User not found.");

            var today = DateTime.UtcNow.Date;

            var userEntry = await _context.DiaryEntries
                .FirstOrDefaultAsync(e => e.UserId == user.Id && e.Date == today);

            var partnerEntry = (user.isConnected && !string.IsNullOrEmpty(user.partnerEmail))
                ? await _context.DiaryEntries
                    .FirstOrDefaultAsync(e => e.User.Email == user.partnerEmail && e.Date == today)
                : null;

            return Ok(new
            {
                YourEntry = userEntry,
                PartnerEntry = partnerEntry
            });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllEntries()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null) return Unauthorized("User not found.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized("User not found.");

            // Fetch all user entries
            var userEntries = await _context.DiaryEntries
                .Where(e => e.UserId == user.Id)
                .OrderBy(e => e.Date)
                .ToListAsync();

            // Fetch all partner entries, if connected
            var partnerEntries = (user.isConnected && !string.IsNullOrEmpty(user.partnerEmail))
                ? await _context.DiaryEntries
                    .Where(e => e.User.Email == user.partnerEmail)
                    .OrderBy(e => e.Date)
                    .ToListAsync()
                : new List<DiaryEntry>();

            return Ok(new
            {
                YourEntries = userEntries,
                PartnerEntries = partnerEntries
            });
        }
    }
}
