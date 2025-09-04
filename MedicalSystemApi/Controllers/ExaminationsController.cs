
using MedicalSystemApi.DTOs;
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystemApi.Controllers
{
    [ApiController]
    [Route("api/patients/{patientId}/[controller]")]
    public class ExaminationsController : ControllerBase
    {
        private readonly IExaminationRepository _examinationRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<ExaminationsController> _logger;

        public ExaminationsController(
            IExaminationRepository examinationRepository,
            IPatientRepository patientRepository,
            ILogger<ExaminationsController> logger)
        {
            _examinationRepository = examinationRepository;
            _patientRepository = patientRepository;
            _logger = logger;
        }

        // GET: api/patients/5/examinations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExaminationDto>>> GetExaminations(int patientId)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", patientId);
                    return NotFound("Patient not found");
                }

                var examinations = await _examinationRepository.GetByPatientIdAsync(patientId);
                var examinationDtos = examinations.Select(e => MapToDto(e));

                _logger.LogInformation("Retrieved {Count} examinations for patient ID: {PatientId}",
                    examinations.Count(), patientId);

                return Ok(examinationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving examinations for patient ID: {PatientId}", patientId);
                return StatusCode(500, "Error retrieving examinations");
            }
        }

        // GET: api/patients/5/examinations/10
        [HttpGet("{id}")]
        public async Task<ActionResult<ExaminationDto>> GetExamination(int patientId, int id)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    return NotFound("Patient not found");
                }

                var examination = await _examinationRepository.GetExaminationWithDetailsAsync(id);
                if (examination == null || examination.PatientId != patientId)
                {
                    _logger.LogWarning("Examination with ID {ExamId} not found for patient ID: {PatientId}", id, patientId);
                    return NotFound("Examination not found");
                }

                _logger.LogInformation("Retrieved examination ID: {ExamId} for patient ID: {PatientId}", id, patientId);
                return Ok(MapToDto(examination));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving examination ID: {ExamId} for patient ID: {PatientId}", id, patientId);
                return StatusCode(500, "Error retrieving examination");
            }
        }

        // POST: api/patients/5/examinations
        [HttpPost]
        public async Task<ActionResult<ExaminationDto>> CreateExamination(int patientId, CreateExaminationDto createExaminationDto)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", patientId);
                    return NotFound("Patient not found");
                }

                var examination = new Examination
                {
                    PatientId = patientId,
                    ExaminationType = createExaminationDto.ExaminationType,
                    ExaminationDate = createExaminationDto.ExaminationDate,
                    Notes = createExaminationDto.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                await _examinationRepository.AddAsync(examination);
                await _examinationRepository.SaveAsync();

                _logger.LogInformation("Created examination ID: {ExamId} for patient ID: {PatientId}",
                    examination.Id, patientId);

                return CreatedAtAction(nameof(GetExamination),
                    new { patientId = patientId, id = examination.Id },
                    MapToDto(examination));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating examination for patient ID: {PatientId}", patientId);
                return StatusCode(500, "Error saving examination to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating examination for patient ID: {PatientId}", patientId);
                return StatusCode(500, "Error creating examination");
            }
        }

        // PUT: api/patients/5/examinations/10
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExamination(int patientId, int id, CreateExaminationDto updateExaminationDto)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    return NotFound("Patient not found");
                }

                var examination = await _examinationRepository.GetByIdAsync(id);
                if (examination == null || examination.PatientId != patientId)
                {
                    return NotFound("Examination not found");
                }

                examination.ExaminationType = updateExaminationDto.ExaminationType;
                examination.ExaminationDate = updateExaminationDto.ExaminationDate;
                examination.Notes = updateExaminationDto.Notes;

                _examinationRepository.Update(examination);
                await _examinationRepository.SaveAsync();

                _logger.LogInformation("Updated examination ID: {ExamId} for patient ID: {PatientId}", id, patientId);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating examination ID: {ExamId} for patient ID: {PatientId}", id, patientId);
                return StatusCode(500, "Error updating examination in database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating examination ID: {ExamId} for patient ID: {PatientId}", id, patientId);
                return StatusCode(500, "Error updating examination");
            }
        }

        // DELETE: api/patients/5/examinations/10
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamination(int patientId, int id)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    return NotFound("Patient not found");
                }

                var examination = await _examinationRepository.GetByIdAsync(id);
                if (examination == null || examination.PatientId != patientId)
                {
                    return NotFound("Examination not found");
                }

                _examinationRepository.Delete(examination);
                await _examinationRepository.SaveAsync();

                _logger.LogInformation("Deleted examination ID: {ExamId} for patient ID: {PatientId}", id, patientId);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting examination ID: {ExamId} for patient ID: {PatientId}", id, patientId);
                return StatusCode(500, "Error deleting examination from database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting examination ID: {ExamId} for patient ID: {PatientId}", id, patientId);
                return StatusCode(500, "Error deleting examination");
            }
        }

        private ExaminationDto MapToDto(Examination examination)
        {
            return new ExaminationDto
            {
                Id = examination.Id,
                PatientId = examination.PatientId,
                ExaminationType = examination.ExaminationType,
                ExaminationDate = examination.ExaminationDate,
                Notes = examination.Notes,
                CreatedAt = examination.CreatedAt,
                PatientName = $"{examination.Patient?.FirstName} {examination.Patient?.LastName}",
                Prescriptions = examination.Prescriptions?.Select(p => new PrescriptionDto
                {
                    Id = p.Id,
                    ExaminationId = p.ExaminationId,
                    MedicationName = p.MedicationName,
                    Dosage = p.Dosage,
                    Instructions = p.Instructions,
                    PrescriptionDate = p.PrescriptionDate,
                    CreatedAt = p.CreatedAt
                }).ToList() ?? new List<PrescriptionDto>(),
                ExaminationFiles = examination.ExaminationFiles?.Select(f => new ExaminationFileDto
                {
                    Id = f.Id,
                    ExaminationId = f.ExaminationId,
                    FileName = f.FileName,
                    FilePath = f.FilePath,
                    FileSize = f.FileSize,
                    UploadDate = f.UploadDate
                }).ToList() ?? new List<ExaminationFileDto>()
            };
        }
    }
}