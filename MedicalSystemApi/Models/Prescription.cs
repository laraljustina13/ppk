using MedicalSystemApi.Models;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystemApi.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public int ExaminationId { get; set; }

        [Required]
        [StringLength(200)]
        public string MedicationName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Dosage { get; set; }
        public string? Instructions { get; set; }

        [Required]
        public DateTime PrescriptionDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual Examination Examination { get; set; } = null!;
    }
}