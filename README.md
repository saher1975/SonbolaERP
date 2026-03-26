# توثيق مشروع SonbolaERP
## Clean Architecture + CQRS | ASP.NET Core 10.0 | C#

---

# أولاً: فلسفة البناء

## ما هي Clean Architecture؟

Clean Architecture هي طريقة بناء مشاريع تضمن أن:
- كل طبقة **لا تعرف** تفاصيل الطبقة التي فوقها
- يمكن تغيير قاعدة البيانات أو الـ Framework بدون تغيير منطق العمل
- الكود سهل الاختبار والصيانة والتوسعة

```
┌─────────────────────────────────────┐
│              API                    │  ← واجهة الاتصال مع المستخدم
├─────────────────────────────────────┤
│          Infrastructure             │  ← تفاصيل قاعدة البيانات
├─────────────────────────────────────┤
│           Application               │  ← منطق العمل (CQRS)
├─────────────────────────────────────┤
│             Domain                  │  ← قلب التطبيق (الكيانات)
└─────────────────────────────────────┘
```

**القاعدة الذهبية:** الأسهم تسير للداخل فقط
- API تعرف Application و Infrastructure
- Infrastructure تعرف Application و Domain
- Application تعرف Domain فقط
- Domain لا تعرف أحداً

---

## ما هو CQRS؟

CQRS = Command Query Responsibility Segregation
يعني: **فصل عمليات الكتابة عن عمليات القراءة**

```
طلب المستخدم
     │
     ├── هل هو قراءة؟  ──► Query ──► Handler ──► يُرجع بيانات
     │
     └── هل هو كتابة؟  ──► Command ──► Handler ──► يُعدّل البيانات
```

**الفائدة:** كل عملية لها Handler مستقل، سهل التعديل والاختبار

---

# ثانياً: بنية المشروع الكاملة

```
SonbolaERP/
│
├── SonbolaERP.sln                          ← ملف الحل (يجمع كل المشاريع)
│
├── Domain/                                 ← المشروع 1: قلب التطبيق
│   ├── Domain.csproj
│   ├── Common/
│   │   └── BaseEntity.cs                   ← كلاس أساسي مشترك
│   ├── Entities/
│   │   └── Product.cs                      ← كيان المنتج
│   ├── Interfaces/
│   │   ├── IGenericRepository.cs           ← عقد Repository
│   │   └── IUnitOfWork.cs                  ← عقد UnitOfWork
│   └── Enums/                              ← مجلد للتعدادات (فارغ حالياً)
│
├── Application/                            ← المشروع 2: منطق العمل
│   ├── Application.csproj
│   ├── Common/
│   │   ├── Behaviors/
│   │   │   └── ValidationBehavior.cs       ← فلتر التحقق التلقائي
│   │   ├── Exceptions/
│   │   │   └── NotFoundException.cs        ← خطأ "العنصر غير موجود"
│   │   └── Interfaces/
│   │       └── IApplicationDbContext.cs    ← عقد قاعدة البيانات
│   ├── Features/
│   │   └── Products/
│   │       ├── Commands/
│   │       │   ├── CreateProduct/
│   │       │   │   ├── CreateProductCommand.cs    ← طلب الإنشاء
│   │       │   │   ├── CreateProductValidator.cs  ← قواعد التحقق
│   │       │   │   └── CreateProductHandler.cs    ← تنفيذ الإنشاء
│   │       │   ├── UpdateProduct/
│   │       │   │   ├── UpdateProductCommand.cs
│   │       │   │   └── UpdateProductHandler.cs
│   │       │   └── DeleteProduct/
│   │       │       ├── DeleteProductCommand.cs
│   │       │       └── DeleteProductHandler.cs
│   │       ├── Queries/
│   │       │   ├── GetAllProducts/
│   │       │   │   ├── GetAllProductsQuery.cs     ← طلب جلب الكل
│   │       │   │   └── GetAllProductsHandler.cs   ← تنفيذ الجلب
│   │       │   └── GetProductById/
│   │       │       ├── GetProductByIdQuery.cs
│   │       │       └── GetProductByIdHandler.cs
│   │       └── DTOs/
│   │           └── ProductDto.cs                  ← شكل البيانات للمستخدم
│   └── DependencyInjection/
│       └── ApplicationServiceExtensions.cs        ← تسجيل الخدمات
│
├── Infrastructure/                         ← المشروع 3: قاعدة البيانات
│   ├── Infrastructure.csproj
│   ├── Data/
│   │   ├── ApplicationDbContext.cs         ← الاتصال الفعلي بـ SQL Server
│   │   └── Configurations/
│   │       └── ProductConfiguration.cs     ← إعدادات جدول المنتجات
│   ├── Repositories/
│   │   ├── GenericRepository.cs            ← تطبيق CRUD العام
│   │   └── UnitOfWork.cs                   ← إدارة حفظ التغييرات
│   ├── Migrations/                         ← ملفات Migration التلقائية
│   └── DependencyInjection/
│       └── InfrastructureServiceExtensions.cs  ← تسجيل الخدمات
│
└── API/                                    ← المشروع 4: نقطة الدخول
    ├── API.csproj
    ├── Program.cs                          ← نقطة انطلاق التطبيق
    ├── appsettings.json                    ← الإعدادات (Connection String)
    ├── Controllers/
    │   └── ProductsController.cs           ← نقاط النهاية (Endpoints)
    └── Middlewares/
        └── GlobalExceptionMiddleware.cs    ← معالجة الأخطاء العامة
```

---

# ثالثاً: الاعتماديات (NuGet Packages)

## مشروع Application

| الحزمة | النسخة | السبب |
|--------|--------|-------|
| MediatR | 12.4.1 | تطبيق CQRS - يوصل Commands/Queries بـ Handlers |
| FluentValidation | 11.11.0 | التحقق من صحة البيانات بطريقة نظيفة |
| FluentValidation.DependencyInjectionExtensions | 11.11.0 | تسجيل Validators تلقائياً في الـ DI |
| Microsoft.EntityFrameworkCore | 10.0.0 | مطلوب لـ DbSet<T> في IApplicationDbContext |

## مشروع Infrastructure

| الحزمة | النسخة | السبب |
|--------|--------|-------|
| Microsoft.EntityFrameworkCore | 10.0.0 | ORM للتعامل مع قاعدة البيانات |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.0 | Provider للاتصال بـ SQL Server Express |
| Microsoft.EntityFrameworkCore.Tools | 10.0.0 | أوامر dotnet ef (migrations) |
| Microsoft.EntityFrameworkCore.Design | 10.0.0 | مطلوب لتشغيل أوامر dotnet ef |

## مشروع API

| الحزمة | النسخة | السبب |
|--------|--------|-------|
| Scalar.AspNetCore | 2.6.0 | واجهة رسومية لاختبار الـ API (بديل Swagger) |
| Microsoft.AspNetCore.OpenApi | 10.0.0 | توليد وثائق OpenAPI |
| Microsoft.EntityFrameworkCore.Design | 10.0.0 | مطلوب لتشغيل Migrations من مشروع API |

---

# رابعاً: الربط بين المشاريع (Project References)

```
Domain          ←── لا يعتمد على أحد
Application     ←── يعتمد على: Domain
Infrastructure  ←── يعتمد على: Domain + Application
API             ←── يعتمد على: Application + Infrastructure
```

أوامر الربط التي نُفِّذت:
```bash
dotnet add Application   reference Domain
dotnet add Infrastructure reference Domain
dotnet add Infrastructure reference Application
dotnet add API           reference Application
dotnet add API           reference Infrastructure
```

---

# خامساً: شرح كل ملف والكود الموجود فيه

---

## Domain/Common/BaseEntity.cs

**السبب:** بدلاً من كتابة خصائص Id و CreatedAt في كل Entity، نكتبها مرة واحدة هنا وكل Entity ترث منها.

```csharp
namespace Domain.Common;

public abstract class BaseEntity
{
    // المعرّف الفريد لكل سجل في قاعدة البيانات
    public int Id { get; set; }

    // تاريخ إنشاء السجل - يُضبط تلقائياً عند الإنشاء
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // تاريخ آخر تعديل - يكون null إذا لم يُعدَّل السجل
    public DateTime? UpdatedAt { get; set; }
}
```

**كلمة abstract:** تعني أنه لا يمكن إنشاء كائن منه مباشرة، فقط الوراثة منه.

---

## Domain/Entities/Product.cs

**السبب:** يمثّل جدول المنتجات في قاعدة البيانات. كل خاصية = عمود في الجدول.

```csharp
using Domain.Common;

namespace Domain.Entities;

public class Product : BaseEntity   // يرث Id, CreatedAt, UpdatedAt من BaseEntity
{
    // اسم المنتج - إلزامي
    public string Name { get; set; } = string.Empty;

    // وصف المنتج - اختياري (? تعني nullable)
    public string? Description { get; set; }

    // سعر المنتج
    public decimal Price { get; set; }

    // الكمية المتاحة في المخزن
    public int Stock { get; set; }
}
```

---

## Domain/Interfaces/IGenericRepository.cs

**السبب:** يُحدّد العمليات التي يجب أن يوفرها أي Repository. طبقة Application تتعامل مع هذه الواجهة فقط، ولا تعرف كيف تُنفَّذ.

```csharp
namespace Domain.Interfaces;

// T هو نوع الكيان (Product, Customer, ...)
public interface IGenericRepository<T> where T : class
{
    // جلب كيان واحد بالـ Id
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // جلب كل الكيانات
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    // إضافة كيان جديد
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    // تعديل كيان موجود
    void Update(T entity);

    // حذف كيان
    void Delete(T entity);
}
```

**CancellationToken:** يسمح بإلغاء العملية إذا أغلق المستخدم الاتصال.

---

## Domain/Interfaces/IUnitOfWork.cs

**السبب:** يضمن أن كل التغييرات تُحفظ معاً أو لا تُحفظ أصلاً (مبدأ المعاملات الذرية).

```csharp
namespace Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // يُرجع Repository لأي نوع من الكيانات
    IGenericRepository<T> Repository<T>() where T : class;

    // يحفظ كل التغييرات في قاعدة البيانات دفعة واحدة
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

**IDisposable:** يضمن تحرير موارد قاعدة البيانات عند الانتهاء.

---

## Application/Common/Interfaces/IApplicationDbContext.cs

**السبب:** طبقة Application تحتاج للوصول لقاعدة البيانات، لكنها لا تريد الاعتماد على EF Core مباشرة. هذه الواجهة تحل المشكلة - Application تتعامل مع الواجهة، و Infrastructure توفر التطبيق الفعلي.

```csharp
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // كل DbSet يمثل جدولاً في قاعدة البيانات
    DbSet<Product> Products { get; }

    // حفظ التغييرات
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
```

---

## Application/Common/Exceptions/NotFoundException.cs

**السبب:** عندما يطلب المستخدم منتجاً بـ Id غير موجود، نرمي هذا الخطأ بدلاً من إرجاع null.

```csharp
namespace Application.Common.Exceptions;

public class NotFoundException : Exception
{
    // name: اسم الكيان (Product, Customer, ...)
    // key: القيمة التي بحثنا عنها (مثل Id = 5)
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
```

**مثال على الرسالة:** `Entity "Product" (5) was not found.`

---

## Application/Common/Behaviors/ValidationBehavior.cs

**السبب:** بدلاً من كتابة كود التحقق في كل Handler، نكتبه مرة واحدة هنا. يعمل تلقائياً قبل أي Command أو Query.

```csharp
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors;

// IPipelineBehavior: يعمل كـ "فلتر" في سلسلة معالجة الطلبات
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    // قائمة بكل Validators المسجّلة لهذا النوع من الطلبات
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,    // الخطوة التالية في السلسلة
        CancellationToken cancellationToken)
    {
        // إذا لم يوجد Validator، تابع بدون فحص
        if (!_validators.Any())
            return await next();

        // تشغيل كل الـ Validators
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // جمع كل الأخطاء
        var failures = validationResults
            .Where(r => r.Errors.Count > 0)
            .SelectMany(r => r.Errors)
            .ToList();

        // إذا وجدت أخطاء، أوقف التنفيذ وأرسل الأخطاء
        if (failures.Count > 0)
            throw new ValidationException(failures);

        // إذا كل شيء صحيح، تابع للـ Handler
        return await next();
    }
}
```

---

## Application/Features/Products/DTOs/ProductDto.cs

**السبب:** لا نُرجع كيان Product كاملاً للمستخدم (قد يحتوي على بيانات حساسة). نُرجع فقط ما يحتاجه.

```csharp
namespace Application.Features.Products.DTOs;

// record: نوع خاص من الكلاسات - للبيانات فقط، لا يمكن تعديلها بعد الإنشاء
public record ProductDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    DateTime CreatedAt
);
```

**record vs class:** الـ record مثالي للـ DTOs لأنه:
- أقل كوداً (لا نحتاج Constructor يدوي)
- غير قابل للتعديل بعد الإنشاء (Immutable)
- مقارنة بالقيم وليس بالمرجع

---

## Application/Features/Products/Commands/CreateProduct/CreateProductCommand.cs

**السبب:** يمثّل "طلب إنشاء منتج" - يحتوي فقط على البيانات المطلوبة من المستخدم.

```csharp
using MediatR;

namespace Application.Features.Products.Commands.CreateProduct;

// IRequest<int>: يعني أن هذا Command سيُرجع int (وهو Id المنتج الجديد)
public record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    int Stock
) : IRequest<int>;
```

---

## Application/Features/Products/Commands/CreateProduct/CreateProductValidator.cs

**السبب:** يُحدّد قواعد صحة بيانات CreateProductCommand. يعمل تلقائياً قبل الـ Handler.

```csharp
using FluentValidation;

namespace Application.Features.Products.Commands.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        // قاعدة اسم المنتج: إلزامي + لا يتجاوز 200 حرف
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم المنتج مطلوب")
            .MaximumLength(200).WithMessage("اسم المنتج لا يتجاوز 200 حرف");

        // قاعدة السعر: يجب أن يكون موجباً
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("السعر يجب أن يكون أكبر من صفر");

        // قاعدة الكمية: لا يمكن أن تكون سالبة
        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("الكمية لا يمكن أن تكون سالبة");
    }
}
```

---

## Application/Features/Products/Commands/CreateProduct/CreateProductHandler.cs

**السبب:** يُنفِّذ عملية إنشاء المنتج الفعلية في قاعدة البيانات.

```csharp
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Products.Commands.CreateProduct;

// IRequestHandler<Command, ResponseType>
public sealed class CreateProductHandler
    : IRequestHandler<CreateProductCommand, int>
{
    private readonly IApplicationDbContext _context;

    // Dependency Injection: يحقن DbContext تلقائياً
    public CreateProductHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(
        CreateProductCommand request,     // البيانات القادمة من المستخدم
        CancellationToken cancellationToken)
    {
        // 1. إنشاء كائن المنتج من البيانات المرسلة
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock
        };

        // 2. إضافته لقاعدة البيانات
        _context.Products.Add(product);

        // 3. حفظ التغييرات
        await _context.SaveChangesAsync(cancellationToken);

        // 4. إرجاع Id المنتج الجديد
        return product.Id;
    }
}
```

---

## Application/Features/Products/Queries/GetAllProducts/GetAllProductsQuery.cs

**السبب:** يمثّل "طلب جلب كل المنتجات" - Query لا يُعدِّل أي بيانات.

```csharp
using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetAllProducts;

// IRequest<IEnumerable<ProductDto>>: يُرجع قائمة من ProductDto
public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;
```

---

## Application/Features/Products/Queries/GetAllProducts/GetAllProductsHandler.cs

**السبب:** يُنفِّذ جلب كل المنتجات من قاعدة البيانات.

```csharp
using Application.Common.Interfaces;
using Application.Features.Products.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Queries.GetAllProducts;

public sealed class GetAllProductsHandler
    : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllProductsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductDto>> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()          // تحسين الأداء: لا نتتبع التغييرات عند القراءة
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Stock,
                p.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
```

**AsNoTracking():** عند القراءة فقط، نخبر EF Core بعدم تتبع التغييرات → أسرع وأقل استهلاكاً للذاكرة.

---

## Application/DependencyInjection/ApplicationServiceExtensions.cs

**السبب:** يُسجِّل كل خدمات Application في حاوية الـ DI دفعة واحدة. يُستخدم في Program.cs بسطر واحد فقط.

```csharp
using Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    // Extension Method: يضيف وظيفة AddApplication() على IServiceCollection
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // تسجيل MediatR: يبحث تلقائياً عن كل Handlers في هذا المشروع
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // تسجيل ValidationBehavior في Pipeline
        // يعني: قبل كل Handler، شغّل ValidationBehavior أولاً
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // تسجيل كل Validators الموجودة في هذا المشروع تلقائياً
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
```

---

## Infrastructure/Data/ApplicationDbContext.cs

**السبب:** التطبيق الفعلي لقاعدة البيانات - يتصل بـ SQL Server Express.

```csharp
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

// يرث من DbContext (EF Core) و يُطبِّق IApplicationDbContext
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    // يستقبل الإعدادات (Connection String) عبر DI
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // كل DbSet = جدول في قاعدة البيانات
    // Set<Product>() يُخبر EF Core بجدول المنتجات
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // يبحث تلقائياً عن كل ملفات Configurations في هذا Assembly
        // ويطبقها على قاعدة البيانات
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
```

---

## Infrastructure/Data/Configurations/ProductConfiguration.cs

**السبب:** يُحدِّد تفاصيل جدول Products في قاعدة البيانات (أسماء الأعمدة، أطوال النصوص، القيود).

```csharp
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

// IEntityTypeConfiguration<Product>: إعدادات خاصة بجدول Product
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // تحديد المفتاح الأساسي
        builder.HasKey(p => p.Id);

        // عمود Name: إلزامي، أقصى طول 200 حرف
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        // عمود Description: اختياري، أقصى طول 1000 حرف
        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        // عمود Price: نوعه في SQL هو decimal(18,2)
        // يعني: 18 رقم إجمالاً، 2 منها بعد الفاصلة
        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)");
    }
}
```

---

## Infrastructure/Repositories/GenericRepository.cs

**السبب:** التطبيق الفعلي لـ IGenericRepository. يوفر عمليات CRUD لأي Entity.

```csharp
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;    // يمثل الجدول في قاعدة البيانات

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();    // يُحدد الجدول بناءً على نوع T
    }

    // جلب بالـ Id - يُرجع null إذا لم يجد
    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync([id], cancellationToken);

    // جلب الكل - AsNoTracking لتحسين الأداء
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    // إضافة - async لأن AddAsync أفضل للـ ValueGenerators
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    // تعديل - يُخبر EF Core بتتبع التغييرات
    public void Update(T entity)
        => _dbSet.Update(entity);

    // حذف - يُزيل الكيان من التتبع وقاعدة البيانات
    public void Delete(T entity)
        => _dbSet.Remove(entity);
}
```

---

## Infrastructure/Repositories/UnitOfWork.cs

**السبب:** يجمع كل Repositories ويتحكم في حفظ التغييرات. يضمن أن كل العمليات تنجح معاً أو تفشل معاً.

```csharp
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    // Dictionary: يخزن Repositories حتى لا ننشئها أكثر من مرة
    private readonly Dictionary<Type, object> _repositories = [];

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);

        // إذا لم يُنشأ Repository لهذا النوع بعد، أنشئه
        if (!_repositories.ContainsKey(type))
            _repositories[type] = new GenericRepository<T>(_context);

        return (IGenericRepository<T>)_repositories[type];
    }

    // حفظ كل التغييرات في قاعدة البيانات دفعة واحدة
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    // تحرير موارد قاعدة البيانات
    public void Dispose()
        => _context.Dispose();
}
```

---

## Infrastructure/DependencyInjection/InfrastructureServiceExtensions.cs

**السبب:** يُسجِّل كل خدمات Infrastructure في الـ DI.

```csharp
using Application.Common.Interfaces;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)    // يستقبل الإعدادات من appsettings.json
    {
        // تسجيل DbContext مع SQL Server
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                // قراءة Connection String من appsettings.json
                configuration.GetConnectionString("DefaultConnection"),
                // تحديد مكان ملفات Migrations
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // ربط IApplicationDbContext بـ ApplicationDbContext
        // عندما يطلب أي كلاس IApplicationDbContext، يُعطيه ApplicationDbContext
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // تسجيل UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
```

**AddScoped:** ينشئ نسخة واحدة لكل HTTP Request وتُحذف عند انتهاء الطلب.

---

## API/appsettings.json

**السبب:** يحتوي على إعدادات التطبيق - أهمها Connection String لقاعدة البيانات.

```json
{
  "ConnectionStrings": {
    // DefaultConnection: الاسم الذي نستخدمه في الكود للوصول لهذه الإعدادات
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=SonbolaERPDB;Trusted_Connection=True;TrustServerCertificate=True;"
    //                    ↑ اسم السيرفر    ↑ اسم قاعدة البيانات    ↑ Windows Auth    ↑ تجاهل شهادة SSL
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      // عرض أوامر SQL في الـ Console - مفيد للتطوير
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "AllowedHosts": "*"    // السماح بالاتصال من أي عنوان
}
```

---

## API/Middlewares/GlobalExceptionMiddleware.cs

**السبب:** يمسك كل الأخطاء التي تحدث في التطبيق ويُرجع رسالة واضحة للمستخدم بدلاً من تفاصيل الخطأ التقنية.

```csharp
using Application.Common.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;      // الخطوة التالية في Pipeline
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);    // نفِّذ باقي التطبيق
        }
        catch (Exception ex)
        {
            // سجِّل الخطأ
            _logger.LogError(ex, "خطأ غير متوقع: {Message}", ex.Message);

            // أرسل رسالة مناسبة للمستخدم
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Pattern Matching: حدِّد نوع الخطأ وأعطه كود HTTP المناسب
        var (statusCode, message) = exception switch
        {
            NotFoundException notFound  => (HttpStatusCode.NotFound, notFound.Message),       // 404
            ValidationException valid   => (HttpStatusCode.BadRequest,                         // 400
                string.Join(", ", valid.Errors.Select(e => e.ErrorMessage))),
            _                           => (HttpStatusCode.InternalServerError, "حدث خطأ داخلي")  // 500
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(response);
    }
}
```

---

## API/Controllers/ProductsController.cs

**السبب:** يُعرِّف نقاط النهاية (URLs) التي يمكن للمستخدم الاتصال بها. يستخدم MediatR لإرسال Commands/Queries بدلاً من استدعاء الخدمات مباشرة.

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]    // الرابط الأساسي: /api/products
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;    // واجهة MediatR لإرسال الطلبات

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    // GET /api/products → جلب كل المنتجات
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllProductsQuery(), cancellationToken);
        return Ok(result);    // 200 + البيانات
    }

    // GET /api/products/5 → جلب منتج بالـ Id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetProductByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    // POST /api/products → إنشاء منتج جديد
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        // 201 Created + رابط المنتج الجديد في Header
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    // PUT /api/products/5 → تعديل منتج
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Id في الرابط لا يتطابق مع Id في البيانات");

        await _sender.Send(command, cancellationToken);
        return NoContent();    // 204 - تم التعديل بنجاح
    }

    // DELETE /api/products/5 → حذف منتج
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteProductCommand(id), cancellationToken);
        return NoContent();    // 204 - تم الحذف بنجاح
    }
}
```

---

## API/Program.cs

**السبب:** نقطة انطلاق التطبيق. يُهيئ كل الخدمات ويبني HTTP Pipeline.

```csharp
using API.Middlewares;
using Application.DependencyInjection;
using Infrastructure.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ===== تسجيل الخدمات =====

// تسجيل كل خدمات Application (MediatR + FluentValidation)
builder.Services.AddApplication();

// تسجيل كل خدمات Infrastructure (DbContext + Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// تسجيل Controllers
builder.Services.AddControllers();

// تسجيل توليد وثائق OpenAPI
builder.Services.AddOpenApi();

// ===== بناء التطبيق =====
var app = builder.Build();

// Middleware 1: معالجة الأخطاء (يجب أن يكون أولاً)
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    // عرض وثائق OpenAPI كـ JSON
    app.MapOpenApi();

    // واجهة Scalar الرسومية لاختبار الـ API
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

# سادساً: كيف يسير الطلب في التطبيق؟

**مثال: المستخدم يُرسل POST /api/products**

```
1. HTTP Request
   └──► ProductsController.Create()
        └──► _sender.Send(CreateProductCommand)
             └──► MediatR Pipeline
                  ├── ValidationBehavior يتحقق من البيانات
                  │    ├── اسم موجود؟ ✓
                  │    ├── سعر موجب؟ ✓
                  │    └── كمية غير سالبة؟ ✓
                  └──► CreateProductHandler.Handle()
                       ├── ينشئ كائن Product
                       ├── يحفظه في قاعدة البيانات
                       └──► يُرجع Id المنتج الجديد
                            └──► Controller يُرجع 201 Created
```

---

# سابعاً: أوامر مفيدة

```bash
# تشغيل التطبيق
dotnet run --project API

# إنشاء Migration جديد بعد إضافة Entity
dotnet ef migrations add "اسم_التغيير" --project Infrastructure --startup-project API

# تطبيق Migration على قاعدة البيانات
dotnet ef database update --project Infrastructure --startup-project API

# بناء المشروع للتحقق من الأخطاء
dotnet build

# إضافة Entity جديد - اتبع نفس النمط الموجود في Products
```

---

# ثامناً: كيف تُضيف Entity جديد؟

**مثال: إضافة Customer (عميل)**

```
1. Domain/Entities/Customer.cs          ← أضف الخصائص
2. Application/Common/Interfaces/       ← أضف DbSet<Customer> لـ IApplicationDbContext
3. Application/Features/Customers/      ← انسخ مجلد Products وعدّله
4. Infrastructure/Data/                 ← أضف DbSet<Customer> لـ ApplicationDbContext
5. Infrastructure/Data/Configurations/  ← أضف CustomerConfiguration.cs
6. API/Controllers/                     ← أضف CustomersController.cs
7. Terminal:
   dotnet ef migrations add AddCustomers --project Infrastructure --startup-project API
   dotnet ef database update --project Infrastructure --startup-project API
```
