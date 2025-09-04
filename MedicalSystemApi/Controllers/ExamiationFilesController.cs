
using MedicalSystemApi.DTOs;
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;
using MedicalSystemApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystemApi.Controllers
{
    [ApiController]
    [Route("api/examinations/{examinationId}/[controller]")]
    public class ExaminationFilesController : ControllerBase
    {
        private readonly IExaminationFileRepository _fileRepository;
        private readonly IExaminationRepository _examinationRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<ExaminationFilesController> _logger;
        private readonly IConfiguration _configuration;

        public ExaminationFilesController(
            IExaminationFileRepository fileRepository,
            IExaminationRepository examinationRepository,
            IFileStorageService fileStorageService,
            ILogger<ExaminationFilesController> logger,
            IConfiguration configuration)

        {
            _fileRepository = fileRepository;
            _examinationRepository = examinationRepository;
            _fileStorageService = fileStorageService;
            _logger = logger;
            _configuration = configuration;
        }





        // GET: api/examinations/5/files
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileResponseDto>>> GetExaminationFiles(int examinationId)
        {
            try
            {
                var examination = await _examinationRepository.GetByIdAsync(examinationId);
                if (examination == null)
                {
                    return NotFound("Examination not found");
                }

                var files = await _fileRepository.GetByExaminationIdAsync(examinationId);
                var fileDtos = files.Select(f => MapToDto(f));

                _logger.LogInformation("Retrieved {Count} files for examination ID: {ExamId}", files.Count(), examinationId);
                return Ok(fileDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving files for examination ID: {ExamId}", examinationId);
                return StatusCode(500, "Error retrieving files");
            }
        }

        // POST: api/examinations/5/files/upload
        [HttpPost("upload")]
        public async Task<ActionResult<FileResponseDto>> UploadFile(int examinationId, [FromForm] FileUploadDto fileUploadDto)
        {
            _logger.LogInformation("=== FILE UPLOAD STARTED ===");
            _logger.LogInformation("ExaminationId: {ExamId}", examinationId);

            try
            {
                // 1. Check if examination exists
                _logger.LogInformation("Checking examination existence...");
                var examination = await _examinationRepository.GetByIdAsync(examinationId);

                if (examination == null)
                {
                    _logger.LogWarning("Examination {ExamId} not found", examinationId);
                    return NotFound("Examination not found");
                }
                _logger.LogInformation("Examination found: {ExamId}", examinationId);

                // 2. Validate file
                _logger.LogInformation("Validating file...");
                if (fileUploadDto.File == null || fileUploadDto.File.Length == 0)
                {
                    _logger.LogWarning("No file provided or file is empty");
                    return BadRequest("No file provided");
                }

                _logger.LogInformation("File received: {FileName}, Size: {Size} bytes",
                    fileUploadDto.File.FileName, fileUploadDto.File.Length);

                // 3. Upload to storage
                _logger.LogInformation("Starting file upload to storage...");
                var filePath = await _fileStorageService.UploadFileAsync(
                    fileUploadDto.File,
                    $"examination-{examinationId}");

                _logger.LogInformation("File uploaded to storage: {FilePath}", filePath);

                // 4. Save to database
                _logger.LogInformation("Saving file metadata to database...");
                var examinationFile = new ExaminationFile
                {
                    ExaminationId = examinationId,
                    FileName = fileUploadDto.File.FileName,
                    FilePath = filePath,
                    FileSize = fileUploadDto.File.Length,
                    UploadDate = DateTime.UtcNow
                };

                await _fileRepository.AddAsync(examinationFile);
                await _fileRepository.SaveAsync();

                _logger.LogInformation("File metadata saved to database with ID: {FileId}", examinationFile.Id);

                // 5. Return success
                _logger.LogInformation("=== FILE UPLOAD COMPLETED SUCCESSFULLY ===");
                return Ok(new FileResponseDto
                {
                    Id = examinationFile.Id,
                    FileName = examinationFile.FileName,
                    FilePath = examinationFile.FilePath,
                    FileSize = examinationFile.FileSize,
                    UploadDate = examinationFile.UploadDate,
                    DownloadUrl = Url.ActionLink("DownloadFile", "ExaminationFiles",
                        new { examinationId = examinationId, fileId = examinationFile.Id })
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=== FILE UPLOAD FAILED ===");
                _logger.LogError("Error message: {ErrorMessage}", ex.Message);
                _logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);

                return StatusCode(500, new
                {
                    error = "Error uploading file",
                    details = ex.Message
                });
            }
        }



        // GET: api/examinations/5/files/10/download

        [HttpGet("{fileId}/download")]
        [Produces("application/octet-stream")]
        public async Task<IActionResult> DownloadFile(int examinationId, int fileId)
        {
            try
            {
                var examination = await _examinationRepository.GetByIdAsync(examinationId);
                if (examination == null)
                {
                    return NotFound("Examination not found");
                }

                var examinationFile = await _fileRepository.GetByIdAsync(fileId);
                if (examinationFile == null || examinationFile.ExaminationId != examinationId)
                {
                    return NotFound("File not found");
                }

                // Now returns byte[] instead of Stream
                var fileBytes = await _fileStorageService.DownloadFileAsync(examinationFile.FilePath);

                _logger.LogInformation("Downloaded file ID: {FileId} for examination ID: {ExamId}", fileId, examinationId);

                // Return FileContentResult with bytes
                return File(fileBytes, "application/octet-stream", examinationFile.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file ID: {FileId} for examination ID: {ExamId}", fileId, examinationId);
                return StatusCode(500, "Error downloading file");
            }
        }

        // DELETE: api/examinations/5/files/10
        [HttpDelete("{fileId}")]
        public async Task<IActionResult> DeleteFile(int examinationId, int fileId)
        {
            try
            {
                var examination = await _examinationRepository.GetByIdAsync(examinationId);
                if (examination == null)
                {
                    return NotFound("Examination not found");
                }

                var examinationFile = await _fileRepository.GetByIdAsync(fileId);
                if (examinationFile == null || examinationFile.ExaminationId != examinationId)
                {
                    return NotFound("File not found");
                }

                // Delete from storage
                var storageResult = await _fileStorageService.DeleteFileAsync(examinationFile.FilePath);

                if (storageResult)
                {
                    // Delete from database
                    _fileRepository.Delete(examinationFile);
                    await _fileRepository.SaveAsync();

                    _logger.LogInformation("Deleted file ID: {FileId} for examination ID: {ExamId}", fileId, examinationId);
                    return NoContent();
                }
                else
                {
                    return StatusCode(500, "Error deleting file from storage");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file ID: {FileId} for examination ID: {ExamId}", fileId, examinationId);
                return StatusCode(500, "Error deleting file");
            }
        }

        private FileResponseDto MapToDto(ExaminationFile file)
        {
            return new FileResponseDto
            {
                Id = file.Id,
                FileName = file.FileName,
                FilePath = file.FilePath,
                FileSize = file.FileSize,
                UploadDate = file.UploadDate,
                DownloadUrl = Url.ActionLink("DownloadFile", "ExaminationFiles",
                new { examinationId = file.ExaminationId, fileId = file.Id })
            };
        }
    }
}
