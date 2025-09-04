using System.ComponentModel.DataAnnotations;

namespace MedicalSystemApi.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string OIB { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [RegularExpression("^[MFO]$")]
        public char Gender { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class CreatePatientDto
    {
        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string OIB { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [RegularExpression("^[MFO]$")]
        public char Gender { get; set; }
    }

    public class UpdatePatientDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [RegularExpression("^[MFO]$")]
        public char Gender { get; set; }
    }
}
