using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

/// <summary>
/// واجهة DbContext للاستخدام في طبقة Application
/// هذا يعزل Application عن تفاصيل قاعدة البيانات
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
