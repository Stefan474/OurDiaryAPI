using System.ComponentModel.DataAnnotations;

namespace OurDiaryAPI.DTOs
{
    public class PartnerDto
    {
        [Required, EmailAddress]
        public string PartnerEmail { get; set; }
    }
}