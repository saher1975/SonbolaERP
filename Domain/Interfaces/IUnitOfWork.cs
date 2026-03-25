namespace Domain.Interfaces;

/// <summary>
/// وحدة العمل - تجمع كل الـ Repositories في مكان واحد
/// وتضمن حفظ كل التغييرات أو التراجع عنها كلها معاً
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
