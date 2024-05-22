using System.Linq.Expressions;

namespace InformationOfStudent.Services
{
    public interface IGenericServices<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid? id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> IsBranchUniqueAsync(string name);
        Task<bool> IsStudentUniqueAsync(string Enrollment);
        IQueryable<T> Query();
        //Task<bool> AddAsync(Func<object, bool> value);
    }
}
