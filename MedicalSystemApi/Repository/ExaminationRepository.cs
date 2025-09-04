
using MedicalSystemApi.Data;
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;
using MedicalSystemApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystemApi.Repository
{
    public class ExaminationRepository : Repository<Examination>, IExaminationRepository
    {
        public ExaminationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Examination>> GetByPatientIdAsync(int patientId)
            => await _context.Examinations
                .Where(e => e.PatientId == patientId)
                .Include(e => e.Prescriptions)
                .Include(e => e.ExaminationFiles)
                .ToListAsync();

        public async Task<Examination> GetExaminationWithDetailsAsync(int id)
            => await _context.Examinations
                .Include(e => e.Patient)
                .Include(e => e.Prescriptions)
                .Include(e => e.ExaminationFiles)
                .FirstOrDefaultAsync(e => e.Id == id);
    }
}