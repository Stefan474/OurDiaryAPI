using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OurDiaryAPI.Models
{
    public class DiaryEntry
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int UserId { get; set; }
        [JsonIgnore] //bugfix for circular referencing
        public User User { get; set; }
    }
}
