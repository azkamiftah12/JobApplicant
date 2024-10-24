using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobApplicant.Models
{
    public class Applicant
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama tidak boleh kosong.")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Email tidak boleh kosong.")]
        [EmailAddress(ErrorMessage = "Format Email tidak sesuai.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Phone tidak boleh kosong.")]
        public string Phone { get; set; } = "";

        public string? ResumeFileName { get; set; }

        [NotMapped]
        public IFormFile? ResumeFile { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
