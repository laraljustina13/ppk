using MedicalSystemApi.Repository;
using MedicalSystemApi.Data;
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystemApi.Repository
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Patient> GetByOibAsync(string oib)
            => await _context.Patients.FirstOrDefaultAsync(p => p.OIB == oib);

        public async Task<IEnumerable<Patient>> SearchAsync(string searchTerm)
            => await _context.Patients
                .Where(p => p.LastName.Contains(searchTerm) || p.OIB.Contains(searchTerm))
                .ToListAsync();

        public async Task<Patient> GetPatientWithDetailsAsync(int id)
            => await _context.Patients
                .Include(p => p.MedicalRecords)
                .Include(p => p.Examinations)
                    .ThenInclude(e => e.Prescriptions)
                .Include(p => p.Examinations)
                    .ThenInclude(e => e.ExaminationFiles)
                .FirstOrDefaultAsync(p => p.Id == id);
    }
}