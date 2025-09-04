using System.ComponentModel.DataAnnotations;

namespace MedicalSystemApi.DTOs
{
    public class MedicalRecordDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Required]
        [StringLength(200)]
        public string DiseaseName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Patient info for display
        public string PatientName { get; set; } = string.Empty;
        public string PatientOIB { get; set; } = string.Empty;
    }

    public class CreateMedicalRecordDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        [StringLength(200)]
        public string DiseaseName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateMedicalRecordDto
    {
        [Required]
        [StringLength(200)]
        public string DiseaseName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
