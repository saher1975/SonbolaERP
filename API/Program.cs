using API.Middlewares;
using Application.DependencyInjection;
using Infrastructure.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// تسجيل الخدمات (Services Registration)
// =============================================

// تسجيل خدمات طبقة Application (MediatR, FluentValidation)
builder.Services.AddApplication();

// تسجيل خدمات طبقة Infrastructure (DbContext, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// تسجيل Controllers
builder.Services.AddControllers();

// تسجيل OpenAPI / Scalar
builder.Services.AddOpenApi();

// =============================================
// بناء التطبيق
// =============================================
var app = builder.Build();

// معالجة الأخطاء العامة (يجب أن يكون أول Middleware)
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    // عرض وثائق API
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
