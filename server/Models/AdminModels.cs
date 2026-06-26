namespace RoboSchool.Models;

public record HealthResponse(
    string Status,
    string Database,
    bool DatabaseConnected,
    DateTime Timestamp,
    string? Version = null
);

public record AdminOverviewResponse(
    int ApplicationsCount,
    int PackagesCount,
    int TrainersCount,
    DateTime? LastApplicationAt,
    string Database,
    bool DatabaseConnected
);
