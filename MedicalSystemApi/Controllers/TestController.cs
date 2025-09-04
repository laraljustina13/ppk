
using MedicalSystemApi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSystemApi.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public TestController(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        [HttpGet("repository")]
        public async Task<IActionResult> TestRepository()
        {
            try
            {
                // Test patient repository
                var patientRepo = _repositoryFactory.PatientRepository;
                var patients = await patientRepo.GetAllAsync();

                return Ok(new
                {
                    success = true,
                    message = "Repository pattern working!",
                    patientCount = patients.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Repository test failed",
                    error = ex.Message
                });
            }
        }
    }
}