using MedicalSystemApi.Models;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystemApi.Models
{
    public class ExaminationFile
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
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual Examination Examination { get; set; } = null!;
    }
}