using MedicalPortal.Domain;
using Microsoft.EntityFrameworkCore;
namespace MedicalPortal.Infrastructure;
public class Repository<T>(AppDbContext db) : IRepository<T> where T : class
{
    public Task<List<T>> GetAllAsync() => db.Set<T>().AsNoTracking().ToListAsync();
    public async Task<T?> GetByIdAsync(int id) => await db.Set<T>().FindAsync(id);
    public async Task AddAsync(T entity)
    {
        db.Set<T>().Add(entity);
        await db.SaveChangesAsync();
    }
}
