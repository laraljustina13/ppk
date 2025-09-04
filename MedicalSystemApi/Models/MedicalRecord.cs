using MedicalSystemApi.Models;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystemApi.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Required]
        [StringLength(200)]
        public string DiseaseName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual Patient Patient { get; set; } = null!;
    }
}