using Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.DependencyInjection;

/// <summary>
/// Extension Method لتسجيل خدمات طبقة Application
/// يُستخدم في Program.cs بهذا الشكل: builder.Services.AddApplication();
/// </summary>
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // تسجيل MediatR - يبحث عن كل Handlers في Assembly الحالية
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // تسجيل ValidationBehavior في Pipeline
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // تسجيل كل Validators تلقائياً
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
