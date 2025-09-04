using MedicalSystemApi.Models;

namespace MedicalSystemApi.Interfaces
{
    public interface IPrescriptionRepository : IRepository<Prescription>
    {
        Task<IEnumerable<Prescription>> GetByExaminationIdAsync(int examinationId);
    }
}