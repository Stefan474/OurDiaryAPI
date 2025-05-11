using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace OurDiaryAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public bool isConnected { get; set; }

        [EmailAddress]
        public string? partnerEmail { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public List<DiaryEntry> DiaryEntries { get; set; }

    }
}