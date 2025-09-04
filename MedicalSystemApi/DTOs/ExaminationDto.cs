using System.ComponentModel.DataAnnotations;

namespace MedicalSystemApi.DTOs
{
    public class ExaminationDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Required]
        [StringLength(10)]
        public string ExaminationType { get; set; } = string.Empty;

        [Required]
        public DateTime ExaminationDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation properties for display
        public string PatientName { get; set; } = string.Empty;
        public List<PrescriptionDto> Prescriptions { get; set; } = new();
        public List<ExaminationFileDto> ExaminationFiles { get; set; } = new();
    }

    public class CreateExaminationDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        [StringLength(10)]
        public string ExaminationType { get; set; } = string.Empty;

        [Required]
        public DateTime ExaminationDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class PrescriptionDto
    {
        public int Id { get; set; }
        public int ExaminationId { get; set; }

        [Required]
        [StringLength(200)]
        public string MedicationName { get; set; } = string.Empty;

        [StringLength(100)]
        public string Dosage { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public DateTime PrescriptionDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ExaminationFileDto
    {
        public int Id { get; set; }
        public int ExaminationId { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
    }
}