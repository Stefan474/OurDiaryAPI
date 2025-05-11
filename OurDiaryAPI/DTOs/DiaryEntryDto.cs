using System.ComponentModel.DataAnnotations;

namespace OurDiaryAPI.DTOs
{
    public class DiaryEntryDto
    {
        [Required]
        [StringLength(500, ErrorMessage = "Note must be between 1 and 500 characters.")]
        public string Note { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        public string? ImageUrl { get; set; }
    }
}