using MedicalSystemApi.Data;
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystemApi.Repository
{
    public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Prescription>> GetByExaminationIdAsync(int examinationId)
            => await _context.Prescriptions
                .Where(p => p.ExaminationId == examinationId)
                .ToListAsync();
    }
}