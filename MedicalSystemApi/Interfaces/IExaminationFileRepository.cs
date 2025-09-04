
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;

namespace MedicalSystemApi.Interfaces
{
    public interface IExaminationFileRepository : IRepository<ExaminationFile>
    {
        Task<IEnumerable<ExaminationFile>> GetByExaminationIdAsync(int examinationId);
    }
}