using InformationOfStudent.Database;
using InformationOfStudent.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InformationOfStudent.Services
{
    public class GenericServices<T> : IGenericServices<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GenericServices(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<T> GetByIdAsync(Guid? id) => await _repository.GetByIdAsync(id);

        public async Task AddAsync(T entity)
        {
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _repository.DeleteAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> IsBranchUniqueAsync(string name) => await _repository.IsBranchUnique(name);

        public async Task<bool> IsStudentUniqueAsync(string Enrollment) => await _repository.IsStudentUnique(Enrollment);

        public IQueryable<T> Query() => _repository.Query();

    }
}
