
using MedicalSystemApi.DTOs;
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystemApi.Controllers
{
    [ApiController]
    [Route("api/patients/{patientId}/[controller]")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<MedicalRecordsController> _logger;

        public MedicalRecordsController(
            IMedicalRecordRepository medicalRecordRepository,
            IPatientRepository patientRepository,
            ILogger<MedicalRecordsController> logger)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _patientRepository = patientRepository;
            _logger = logger;
        }

        // GET: api/patients/5/medicalrecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecords(int patientId)
        {
            try
            {
                // Verify patient exists
                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", patientId);
                    return NotFound("Patient not found");
                }

                var medicalRecords = await _medicalRecordRepository.GetByPatientIdAsync(patientId);
                var medicalRecordDtos = medicalRecords.Select(mr => MapToDto(mr, patient));

                _logger.LogInformation("Retrieved {Count} medical records for patient ID: {PatientId}",
                    medicalRecords.Count(), patientId);

                return Ok(medicalRecordDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical records for patient ID: {PatientId}", patientId);
                return StatusCode(500, "Error retrieving medical records");
            }
        }

        // GET: api/patients/5/medicalrecords/10
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecordDto>> GetMedicalRecord(int patientId, int id)
        {
            try
            {
                // Verify patient exists
                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    return NotFound("Patient not found");
                }

                var medicalRecord = await _medicalRecordRepository.GetByIdAsync(id);
                if (medicalRecord == null || medicalRecord.PatientId != patientId)
                {
                    _logger.LogWarning("Medical record with ID {RecordId} not found for patient ID: {PatientId}", id, patientId);
                    return NotFound("Medical record not found");
                }

                _logger.LogInformation("Retrieved medical record ID: {RecordId} for patient ID: {PatientId}", id, patientId);
                return Ok(MapToDto(medicalRecord, patient));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical record ID: {RecordId} for patient ID: {PatientId}", id, patientId);
                return StatusCode(500, "Error retrieving medical record");
            }
        }

        // POST: api/patients/5/medicalrecords
        [HttpPost]
        public async Task<ActionResult<MedicalRecordDto>> CreateMedicalRecord(int patientId, CreateMedicalRecordDto createMedicalRecordDto)
        {
            try
            {
                // Verify patient exists
                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", patientId);
                    return NotFound("Patient not found");
                }

                var medicalRecord = new MedicalRecord
                {
                    PatientId = patientId,
                    DiseaseName = createMedicalRecordDto.DiseaseName,
                    StartDate = createMedicalRecordDto.StartDate,
                    EndDate = createMedicalRecordDto.EndDate,
                    Description = createMedicalRecordDto.Description,
                    CreatedAt = DateTime.UtcNow
                };

                await _medicalRecordRepository.AddAsync(medicalRecord);
                await _medicalRecordRepository.SaveAsync();

                _logger.LogInformation("Created medical record ID: {RecordId} for patient ID: {PatientId}",
                    medicalRecord.Id, patientId);

                return CreatedAtAction(nameof(GetMedicalRecord),
                    new { patientId = patientId, id = medicalRecord.Id },
                    MapToDto(medicalRecord, patient));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating medical record for patient ID: {PatientId}", patientId);
                return StatusCode(500, "Error saving medical record to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medical record for patient ID: {PatientId}", patientId);
                return StatusCode(500, "Error creating medical record");
            }
        }

        // PUT: api/patients/5/medicalrecords/10
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicalRecord(int patientId, int id, UpdateMedicalRecordDto updateMedicalRecordDto)
        {
            try
            {
                // Verify patient exists
                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    return NotFound("Patient not found");
                }

                var medicalRecord = await _medicalRecordRepository.GetByIdAsync(id);
                if (medicalRecord == null || medicalRecord.PatientId != patientId)
                {
                    return NotFound("Medical record not found");
                }

                medicalRecord.DiseaseName = updateMedicalRecordDto.DiseaseName;
                medicalRecord.StartDate = updateMedicalRecordDto.StartDate;
                medicalRecord.EndDate = updateMedicalRecordDto.EndDate;
                medicalRecord.Description = updateMedicalRecordDto.Description;

                _medicalRecordRepository.Update(medicalRecord);
                await _medicalRecordRepository.SaveAsync();

                _logger.LogInformation("Updated medical record ID: {RecordId} for patient ID: {PatientId}", id, patientId);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating medical record ID: {RecordId} for patient ID: {PatientId}", id, patientId);
                return StatusCode(500, "Error updating medical record in database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medical record ID: {RecordId} for patient ID: {PatientId}", id, patientId);
                return StatusCode(500, "Error updating medical record");
            }
        }

        // DELETE: api/patients/5/medicalrecords/10
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalRecord(int patientId, int id)
        {
            try
            {
                // Verify patient exists
                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    return NotFound("Patient not found");
                }

                var medicalRecord = await _medicalRecordRepository.GetByIdAsync(id);
                if (medicalRecord == null || medicalRecord.PatientId != patientId)
                {
                    return NotFound("Medical record not found");
                }

                _medicalRecordRepository.Delete(medicalRecord);
                await _medicalRecordRepository.SaveAsync();

                _logger.LogInformation("Deleted medical record ID: {RecordId} for patient ID: {PatientId}", id, patientId);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting medical record ID: {RecordId} for patient ID: {PatientId}", id, patientId);
                return StatusCode(500, "Error deleting medical record from database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medical record ID: {RecordId} for patient ID: {PatientId}", id, patientId);
                return StatusCode(500, "Error deleting medical record");
            }
        }

        private MedicalRecordDto MapToDto(MedicalRecord medicalRecord, Patient patient)
        {
            return new MedicalRecordDto
            {
                Id = medicalRecord.Id,
                PatientId = medicalRecord.PatientId,
                DiseaseName = medicalRecord.DiseaseName,
                StartDate = medicalRecord.StartDate,
                EndDate = medicalRecord.EndDate,
                Description = medicalRecord.Description,
                CreatedAt = medicalRecord.CreatedAt,
                PatientName = $"{patient.FirstName} {patient.LastName}",
                PatientOIB = patient.OIB
            };
        }
    }
}