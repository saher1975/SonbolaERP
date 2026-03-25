namespace Application.Features.Products.DTOs;

/// <summary>
/// DTO للمنتج: البيانات التي تُرسل للمستخدم
/// </summary>
public record ProductDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    DateTime CreatedAt
);
