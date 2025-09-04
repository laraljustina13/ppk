
using MedicalSystemApi.Data;
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Models;
using MedicalSystemApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystemApi.Repository
{
    public class MedicalRecordRepository : Repository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId)
            => await _context.MedicalRecords
                .Where(mr => mr.PatientId == patientId)
                .ToListAsync();
    }
}