
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;

namespace MedicalSystemApi.Interfaces
{
    public interface IExaminationRepository : IRepository<Examination>
    {
        Task<IEnumerable<Examination>> GetByPatientIdAsync(int patientId);
        Task<Examination> GetExaminationWithDetailsAsync(int id);
    }
}