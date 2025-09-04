
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;

namespace MedicalSystemApi.Interfaces
{
    public interface IMedicalRecordRepository : IRepository<MedicalRecord>
    {
        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId);
    }
}