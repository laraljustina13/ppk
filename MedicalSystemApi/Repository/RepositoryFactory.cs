

using MedicalSystemApi.Data;
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Repository;

namespace MedicalSystemApi.Repository
{
    public interface IRepositoryFactory
    {
        IRepository<T> GetRepository<T>() where T : class;
        IPatientRepository PatientRepository { get; }
        IMedicalRecordRepository MedicalRecordRepository { get; }
        IExaminationRepository ExaminationRepository { get; }
        IExaminationFileRepository ExaminationFileRepository { get; }
    }

    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public RepositoryFactory(ApplicationDbContext context)
        {
            _context = context;
            PatientRepository = new PatientRepository(context);
            MedicalRecordRepository = new MedicalRecordRepository(context);
            ExaminationRepository = new ExaminationRepository(context);
            ExaminationFileRepository = new ExaminationFileRepository(context);
        }

        public IPatientRepository PatientRepository { get; }
        public IMedicalRecordRepository MedicalRecordRepository { get; }
        public IExaminationRepository ExaminationRepository { get; }
        public IExaminationFileRepository ExaminationFileRepository { get; }

        public IRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
                return (IRepository<T>)_repositories[typeof(T)];

            var repository = new Repository<T>(_context);
            _repositories.Add(typeof(T), repository);
            return repository;
        }
    }
}