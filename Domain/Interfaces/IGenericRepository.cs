namespace Domain.Interfaces;

/// <summary>
/// واجهة عامة للـ Repository تحتوي على عمليات CRUD الأساسية
/// T: نوع الكيان، يجب أن يكون كلاس
/// </summary>
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}
