using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RoboSchool.Data;
using RoboSchool.Models;
using RoboSchool.Services;

var builder = WebApplication.CreateBuilder(args);

var siteRoot = Environment.GetEnvironmentVariable("SITE_ROOT") is { Length: > 0 } configuredSiteRoot
    ? Path.GetFullPath(configuredSiteRoot)
    : Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, ".."));
var connectionString = DatabaseConfiguration.ResolveConnectionString(builder.Configuration);
var isPostgreSql = DatabaseConfiguration.IsPostgreSql(connectionString);
var adminKey = builder.Configuration["AdminKey"] ?? "robo-admin-2026";

builder.Services.AddDbContext<AppDbContext>(options =>
    DatabaseConfiguration.ConfigureDbContext(options, connectionString));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddSingleton<EmailService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DatabaseInitializer.InitializeAsync(db, isPostgreSql);
}

app.UseCors();

var staticFiles = new PhysicalFileProvider(siteRoot);
app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = staticFiles,
    RequestPath = "",
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = staticFiles,
    RequestPath = "",
});

app.MapGet("/api/health", async (AppDbContext db) =>
{
    var connected = false;
    try
    {
        connected = await db.Database.CanConnectAsync();
    }
    catch
    {
        connected = false;
    }

    return Results.Ok(new HealthResponse(
        connected ? "ok" : "degraded",
        isPostgreSql ? "postgresql" : "sqlite",
        connected,
        DateTime.UtcNow,
        typeof(Program).Assembly.GetName().Version?.ToString()
    ));
});

app.MapGet("/api/admin/overview", async (string? key, AppDbContext db) =>
{
    if (key != adminKey)
        return Results.Unauthorized();

    var connected = await db.Database.CanConnectAsync();
    var applicationsCount = connected ? await db.Applications.CountAsync() : 0;
    var packagesCount = connected ? await db.Packages.CountAsync() : 0;
    var trainersCount = connected ? await db.Trainers.CountAsync() : 0;
    DateTime? lastApplicationAt = connected
        ? await db.Applications.OrderByDescending(a => a.CreatedAt).Select(a => (DateTime?)a.CreatedAt).FirstOrDefaultAsync()
        : null;

    return Results.Ok(new AdminOverviewResponse(
        applicationsCount,
        packagesCount,
        trainersCount,
        lastApplicationAt,
        isPostgreSql ? "postgresql" : "sqlite",
        connected
    ));
});

app.MapGet("/api/packages", async (AppDbContext db) =>
{
    var packages = await db.Packages
        .OrderBy(p => p.Price)
        .Select(p => new PackageResponse(p.Code, p.Name, p.Price, p.Description))
        .ToListAsync();
    return Results.Ok(packages);
});

app.MapGet("/api/trainers", async (AppDbContext db) =>
{
    var trainers = await db.Trainers
        .OrderBy(t => t.Id)
        .Select(t => new TrainerResponse(t.Slug, t.Name, t.Role, t.PhotoUrl, t.Bio))
        .ToListAsync();
    return Results.Ok(trainers);
});

app.MapPost("/api/applications", async (
    CreateApplicationRequest request,
    AppDbContext db,
    EmailService emailService,
    ILogger<Program> logger) =>
{
    var name = request.Name?.Trim() ?? "";
    var phone = request.Phone?.Trim() ?? "";
    var email = request.Email?.Trim() ?? "";
    var package = string.IsNullOrWhiteSpace(request.Package) ? null : request.Package.Trim().ToUpperInvariant();

    if (name.Length < 2)
        return Results.BadRequest(new { error = "Укажите имя (минимум 2 символа)." });

    if (!Regex.IsMatch(phone, @"^[\d\s+\-()]{7,20}$"))
        return Results.BadRequest(new { error = "Укажите корректный телефон." });

    if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        return Results.BadRequest(new { error = "Укажите корректный e-mail." });

    if (package is not null)
    {
        var exists = await db.Packages.AnyAsync(p => p.Code == package);
        if (!exists)
            return Results.BadRequest(new { error = "Неизвестный пакет курса." });
    }

    var record = new ApplicationRecord
    {
        Name = name,
        Phone = phone,
        Email = email,
        PackageCode = package,
        CreatedAt = DateTime.UtcNow,
    };

    db.Applications.Add(record);
    await db.SaveChangesAsync();

    var emailSent = false;
    try
    {
        await emailService.SendApplicationConfirmationAsync(record.Email, record.Name, record.Id);
        emailSent = true;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Не удалось отправить письмо-подтверждение на {Email} (заявка №{Id}).", record.Email, record.Id);
    }

    return Results.Created($"/api/applications/{record.Id}", new ApplicationResponse(
        record.Id,
        record.Name,
        record.Phone,
        record.Email,
        record.PackageCode,
        record.CreatedAt,
        emailSent
    ));
});

app.MapGet("/api/applications", async (string? key, AppDbContext db) =>
{
    if (key != adminKey)
        return Results.Unauthorized();

    var items = await db.Applications
        .OrderByDescending(a => a.CreatedAt)
        .Select(a => new ApplicationResponse(
            a.Id,
            a.Name,
            a.Phone,
            a.Email,
            a.PackageCode,
            a.CreatedAt
        ))
        .ToListAsync();

    return Results.Ok(items);
});

app.MapDelete("/api/applications/{id:int}", async (int id, string? key, AppDbContext db) =>
{
    if (key != adminKey)
        return Results.Unauthorized();

    var record = await db.Applications.FindAsync(id);
    if (record is null)
        return Results.NotFound();

    db.Applications.Remove(record);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

if (Environment.GetEnvironmentVariable("PORT") is { Length: > 0 } port)
    app.Run($"http://0.0.0.0:{port}");
else
    app.Run();
