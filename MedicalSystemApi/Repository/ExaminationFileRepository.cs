
using MedicalSystemApi.Data;
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;
using MedicalSystemApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystemApi.Repository
{
    public class ExaminationFileRepository : Repository<ExaminationFile>, IExaminationFileRepository
    {
        public ExaminationFileRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ExaminationFile>> GetByExaminationIdAsync(int examinationId)
            => await _context.ExaminationFiles
                .Where(ef => ef.ExaminationId == examinationId)
                .ToListAsync();
    }
}