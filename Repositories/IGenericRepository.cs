
namespace InformationOfStudent.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid? id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> SaveChangesAsync();
        Task<bool> IsBranchUnique(string name);
        Task<bool> IsStudentUnique(string Enrollment);
        IQueryable<T> Query();
    }
}