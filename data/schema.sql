-- SQLite schema for ROBO.SCHOOL
-- Database file: data/robo.db (created automatically on first run)

CREATE TABLE IF NOT EXISTS packages (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Code TEXT NOT NULL UNIQUE,
    Name TEXT NOT NULL,
    Price INTEGER NOT NULL,
    Description TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS trainers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Slug TEXT NOT NULL UNIQUE,
    Name TEXT NOT NULL,
    Role TEXT NOT NULL,
    PhotoUrl TEXT NOT NULL,
    Bio TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS applications (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Phone TEXT NOT NULL,
    Email TEXT NOT NULL,
    PackageCode TEXT,
    CreatedAt TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS IX_applications_CreatedAt ON applications (CreatedAt);
