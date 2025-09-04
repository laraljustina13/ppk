using MedicalSystemApi.Models;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystemApi.Models
{
    public class Examination
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Required]
        [StringLength(10)]
        public string ExaminationType { get; set; } = string.Empty;

        [Required]
        public DateTime ExaminationDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual ICollection<ExaminationFile> ExaminationFiles { get; set; } = new List<ExaminationFile>();
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}