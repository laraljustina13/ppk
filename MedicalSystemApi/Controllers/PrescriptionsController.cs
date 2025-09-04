using MedicalSystemApi.DTOs;
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystemApi.Controllers
{
    [ApiController]
    [Route("api/examinations/{examinationId}/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IExaminationRepository _examinationRepository;
        private readonly ILogger<PrescriptionsController> _logger;

        public PrescriptionsController(
            IPrescriptionRepository prescriptionRepository,
            IExaminationRepository examinationRepository,
            ILogger<PrescriptionsController> logger)
        {
            _prescriptionRepository = prescriptionRepository;
            _examinationRepository = examinationRepository;
            _logger = logger;
        }

        // GET: api/examinations/5/prescriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetPrescriptions(int examinationId)
        {
            try
            {
                var examination = await _examinationRepository.GetByIdAsync(examinationId);
                if (examination == null)
                {
                    _logger.LogWarning("Examination with ID {ExaminationId} not found", examinationId);
                    return NotFound("Examination not found");
                }

                var prescriptions = await _prescriptionRepository.GetByExaminationIdAsync(examinationId);
                var prescriptionDtos = prescriptions.Select(p => MapToDto(p));

                _logger.LogInformation("Retrieved {Count} prescriptions for examination ID: {ExaminationId}",
                    prescriptions.Count(), examinationId);

                return Ok(prescriptionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prescriptions for examination ID: {ExaminationId}", examinationId);
                return StatusCode(500, "Error retrieving prescriptions");
            }
        }

        // POST: api/examinations/5/prescriptions
        [HttpPost]
        public async Task<ActionResult<PrescriptionDto>> CreatePrescription(int examinationId, CreatePrescriptionDto createPrescriptionDto)
        {
            try
            {
                var examination = await _examinationRepository.GetByIdAsync(examinationId);
                if (examination == null)
                {
                    _logger.LogWarning("Examination with ID {ExaminationId} not found", examinationId);
                    return NotFound("Examination not found");
                }

                var prescription = new Prescription
                {
                    ExaminationId = examinationId,
                    MedicationName = createPrescriptionDto.MedicationName,
                    Dosage = createPrescriptionDto.Dosage,
                    Instructions = createPrescriptionDto.Instructions,
                    PrescriptionDate = createPrescriptionDto.PrescriptionDate,
                    CreatedAt = DateTime.UtcNow
                };

                await _prescriptionRepository.AddAsync(prescription);
                await _prescriptionRepository.SaveAsync();

                _logger.LogInformation("Created prescription ID: {PrescriptionId} for examination ID: {ExaminationId}",
                    prescription.Id, examinationId);

                return CreatedAtAction(nameof(GetPrescriptions),
                    new { examinationId = examinationId },
                    MapToDto(prescription));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating prescription for examination ID: {ExaminationId}", examinationId);
                return StatusCode(500, "Error saving prescription to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating prescription for examination ID: {ExaminationId}", examinationId);
                return StatusCode(500, "Error creating prescription");
            }
        }

        // PUT: api/examinations/5/prescriptions/10
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int examinationId, int id, UpdatePrescriptionDto updatePrescriptionDto)
        {
            try
            {
                var examination = await _examinationRepository.GetByIdAsync(examinationId);
                if (examination == null)
                {
                    return NotFound("Examination not found");
                }

                var prescription = await _prescriptionRepository.GetByIdAsync(id);
                if (prescription == null || prescription.ExaminationId != examinationId)
                {
                    return NotFound("Prescription not found");
                }

                prescription.MedicationName = updatePrescriptionDto.MedicationName;
                prescription.Dosage = updatePrescriptionDto.Dosage;
                prescription.Instructions = updatePrescriptionDto.Instructions;
                prescription.PrescriptionDate = updatePrescriptionDto.PrescriptionDate;

                _prescriptionRepository.Update(prescription);
                await _prescriptionRepository.SaveAsync();

                _logger.LogInformation("Updated prescription ID: {PrescriptionId} for examination ID: {ExaminationId}", id, examinationId);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating prescription ID: {PrescriptionId} for examination ID: {ExaminationId}", id, examinationId);
                return StatusCode(500, "Error updating prescription in database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating prescription ID: {PrescriptionId} for examination ID: {ExaminationId}", id, examinationId);
                return StatusCode(500, "Error updating prescription");
            }
        }

        // DELETE: api/examinations/5/prescriptions/10
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int examinationId, int id)
        {
            try
            {
                var examination = await _examinationRepository.GetByIdAsync(examinationId);
                if (examination == null)
                {
                    return NotFound("Examination not found");
                }

                var prescription = await _prescriptionRepository.GetByIdAsync(id);
                if (prescription == null || prescription.ExaminationId != examinationId)
                {
                    return NotFound("Prescription not found");
                }

                _prescriptionRepository.Delete(prescription);
                await _prescriptionRepository.SaveAsync();

                _logger.LogInformation("Deleted prescription ID: {PrescriptionId} for examination ID: {ExaminationId}", id, examinationId);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting prescription ID: {PrescriptionId} for examination ID: {ExaminationId}", id, examinationId);
                return StatusCode(500, "Error deleting prescription from database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting prescription ID: {PrescriptionId} for examination ID: {ExaminationId}", id, examinationId);
                return StatusCode(500, "Error deleting prescription");
            }
        }

        private PrescriptionDto MapToDto(Prescription prescription)
        {
            return new PrescriptionDto
            {
                Id = prescription.Id,
                ExaminationId = prescription.ExaminationId,
                MedicationName = prescription.MedicationName,
                Dosage = prescription.Dosage,
                Instructions = prescription.Instructions,
                PrescriptionDate = prescription.PrescriptionDate,
                CreatedAt = prescription.CreatedAt
            };
        }
    }

    public class CreatePrescriptionDto
    {
        [Required]
        [StringLength(200)]
        public string MedicationName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Dosage { get; set; }
        public string? Instructions { get; set; }

        [Required]
        public DateTime PrescriptionDate { get; set; }
    }

    public class UpdatePrescriptionDto
    {
        [Required]
        [StringLength(200)]
        public string MedicationName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Dosage { get; set; }
        public string? Instructions { get; set; }

        [Required]
        public DateTime PrescriptionDate { get; set; }
    }
}