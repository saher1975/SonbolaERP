using FluentValidation;

namespace Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Validator للتحقق من صحة بيانات إنشاء المنتج
/// يعمل تلقائياً عبر ValidationBehavior في Pipeline
/// </summary>
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم المنتج مطلوب")
            .MaximumLength(200).WithMessage("اسم المنتج لا يتجاوز 200 حرف");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("السعر يجب أن يكون أكبر من صفر");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("الكمية لا يمكن أن تكون سالبة");
    }
}
