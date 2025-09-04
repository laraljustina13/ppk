
using MedicalSystemApi.DTOs;
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystemApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(IPatientRepository patientRepository, ILogger<PatientsController> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }

        // GET: api/patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients([FromQuery] string search = null)
        {
            try
            {
                IEnumerable<Patient> patients;

                if (!string.IsNullOrEmpty(search))
                {
                    patients = await _patientRepository.SearchAsync(search);
                    _logger.LogInformation("Searched patients with term: {SearchTerm}, found {Count} results", search, patients.Count());
                }
                else
                {
                    patients = await _patientRepository.GetAllAsync();
                    _logger.LogInformation("Retrieved all patients, total: {Count}", patients.Count());
                }

                var patientDtos = patients.Select(p => MapToDto(p));
                return Ok(patientDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patients");
                return StatusCode(500, "Error retrieving patients from database");
            }
        }

        // GET: api/patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatient(int id)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(id);

                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", id);
                    return NotFound();
                }

                _logger.LogInformation("Retrieved patient with ID: {PatientId}", id);
                return Ok(MapToDto(patient));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient with ID: {PatientId}", id);
                return StatusCode(500, "Error retrieving patient");
            }
        }

        // GET: api/patients/oib/12345678901
        [HttpGet("oib/{oib}")]
        public async Task<ActionResult<PatientDto>> GetPatientByOib(string oib)
        {
            try
            {
                var patient = await _patientRepository.GetByOibAsync(oib);

                if (patient == null)
                {
                    _logger.LogWarning("Patient with OIB {OIB} not found", oib);
                    return NotFound();
                }

                _logger.LogInformation("Retrieved patient with OIB: {OIB}", oib);
                return Ok(MapToDto(patient));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient with OIB: {OIB}", oib);
                return StatusCode(500, "Error retrieving patient");
            }
        }

        // GET: api/patients/5/details
        [HttpGet("{id}/details")]
        public async Task<ActionResult<Patient>> GetPatientWithDetails(int id)
        {
            try
            {
                var patient = await _patientRepository.GetPatientWithDetailsAsync(id);

                if (patient == null)
                {
                    return NotFound();
                }

                return Ok(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient details with ID: {PatientId}", id);
                return StatusCode(500, "Error retrieving patient details");
            }
        }

        // POST: api/patients
        [HttpPost]
        public async Task<ActionResult<PatientDto>> CreatePatient(CreatePatientDto createPatientDto)
        {
            try
            {
                // Check if OIB already exists
                var existingPatient = await _patientRepository.GetByOibAsync(createPatientDto.OIB);
                if (existingPatient != null)
                {
                    _logger.LogWarning("Attempt to create patient with duplicate OIB: {OIB}", createPatientDto.OIB);
                    return BadRequest("Patient with this OIB already exists");
                }

                var patient = new Patient
                {
                    OIB = createPatientDto.OIB,
                    FirstName = createPatientDto.FirstName,
                    LastName = createPatientDto.LastName,
                    DateOfBirth = createPatientDto.DateOfBirth,
                    Gender = createPatientDto.Gender,
                    CreatedAt = DateTime.UtcNow
                };

                await _patientRepository.AddAsync(patient);
                await _patientRepository.SaveAsync();

                _logger.LogInformation("Created new patient with ID: {PatientId}, OIB: {OIB}", patient.Id, patient.OIB);

                return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, MapToDto(patient));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating patient with OIB: {OIB}", createPatientDto.OIB);
                return StatusCode(500, "Error saving patient to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient");
                return StatusCode(500, "Error creating patient");
            }
        }

        // PUT: api/patients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, UpdatePatientDto updatePatientDto)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(id);

                if (patient == null)
                {
                    return NotFound();
                }

                patient.FirstName = updatePatientDto.FirstName;
                patient.LastName = updatePatientDto.LastName;
                patient.DateOfBirth = updatePatientDto.DateOfBirth;
                patient.Gender = updatePatientDto.Gender;

                _patientRepository.Update(patient);
                await _patientRepository.SaveAsync();

                _logger.LogInformation("Updated patient with ID: {PatientId}", id);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating patient with ID: {PatientId}", id);
                return StatusCode(500, "Error updating patient in database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient with ID: {PatientId}", id);
                return StatusCode(500, "Error updating patient");
            }
        }

        // DELETE: api/patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(id);

                if (patient == null)
                {
                    return NotFound();
                }

                _patientRepository.Delete(patient);
                await _patientRepository.SaveAsync();

                _logger.LogInformation("Deleted patient with ID: {PatientId}", id);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting patient with ID: {PatientId}", id);
                return StatusCode(500, "Error deleting patient from database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient with ID: {PatientId}", id);
                return StatusCode(500, "Error deleting patient");
            }
        }

        private PatientDto MapToDto(Patient patient)
        {
            return new PatientDto
            {
                Id = patient.Id,
                OIB = patient.OIB,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                CreatedAt = patient.CreatedAt
            };
        }
    }
}
