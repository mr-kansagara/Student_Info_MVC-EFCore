using InformationOfStudent.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace InformationOfStudent.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(DatabaseContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T> GetByIdAsync(Guid? id) => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public async Task UpdateAsync(T entity) => _dbSet.Update(entity);

        public async Task DeleteAsync(T entity) => _dbSet.Remove(entity);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<bool> IsBranchUnique(string name)
        {
            // Assumes that T has a property called "Name" of type string
            return !await _dbSet.AnyAsync(e => EF.Property<string>(e, "BranchName") == name);
        }

        public async Task<bool> IsStudentUnique(string Enrollment)
        {
            return !await _dbSet.AnyAsync(e => EF.Property<string>(e,"Enrollmentnumber") == Enrollment);
        }

        public IQueryable<T> Query() => _dbSet.AsQueryable();
    }
}
