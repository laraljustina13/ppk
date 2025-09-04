using System.ComponentModel.DataAnnotations;

namespace MedicalSystemApi.DTOs
{
    public class FileUploadDto
    {
        [Required]
        public IFormFile File { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class FileResponseDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public string DownloadUrl { get; set; } = string.Empty;
    }
}