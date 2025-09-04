
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;

namespace MedicalSystemApi.Interfaces
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient> GetByOibAsync(string oib);
        Task<IEnumerable<Patient>> SearchAsync(string searchTerm);
        Task<Patient> GetPatientWithDetailsAsync(int id);
    }
}