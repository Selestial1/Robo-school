namespace RoboSchool.Models;

public class ApplicationRecord
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public string? PackageCode { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Package
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public int Price { get; set; }
    public required string Description { get; set; }
}

public class Trainer
{
    public int Id { get; set; }
    public required string Slug { get; set; }
    public required string Name { get; set; }
    public required string Role { get; set; }
    public required string PhotoUrl { get; set; }
    public required string Bio { get; set; }
}

public record CreateApplicationRequest(
    string Name,
    string Phone,
    string Email,
    string? Package
);

public record ApplicationResponse(
    int Id,
    string Name,
    string Phone,
    string Email,
    string? PackageCode,
    DateTime CreatedAt,
    bool EmailSent = false
);

public record PackageResponse(
    string Code,
    string Name,
    int Price,
    string Description
);

public record TrainerResponse(
    string Slug,
    string Name,
    string Role,
    string PhotoUrl,
    string Bio
);
