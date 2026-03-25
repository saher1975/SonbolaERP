namespace Domain.Common;

/// <summary>
/// كلاس أساسي لكل الكيانات - يحتوي على خصائص مشتركة
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
