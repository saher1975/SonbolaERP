namespace Application.Common.Exceptions;

/// <summary>
/// استثناء مخصص: يُرمى عندما لا يُعثر على العنصر المطلوب
/// مثال: طلب منتج بـ Id غير موجود
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
