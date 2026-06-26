namespace RoboSchool.Data;

public static class PostgreSqlSchema
{
    public const string CreateTablesSql = """
        CREATE TABLE IF NOT EXISTS packages (
            "Id" SERIAL PRIMARY KEY,
            "Code" VARCHAR(20) NOT NULL,
            "Name" VARCHAR(50) NOT NULL,
            "Price" INTEGER NOT NULL,
            "Description" VARCHAR(500) NOT NULL
        );

        CREATE UNIQUE INDEX IF NOT EXISTS "IX_packages_Code" ON packages ("Code");

        CREATE TABLE IF NOT EXISTS trainers (
            "Id" SERIAL PRIMARY KEY,
            "Slug" VARCHAR(40) NOT NULL,
            "Name" VARCHAR(120) NOT NULL,
            "Role" VARCHAR(120) NOT NULL,
            "PhotoUrl" VARCHAR(500) NOT NULL,
            "Bio" VARCHAR(2000) NOT NULL
        );

        CREATE UNIQUE INDEX IF NOT EXISTS "IX_trainers_Slug" ON trainers ("Slug");

        CREATE TABLE IF NOT EXISTS applications (
            "Id" SERIAL PRIMARY KEY,
            "Name" VARCHAR(120) NOT NULL,
            "Phone" VARCHAR(30) NOT NULL,
            "Email" VARCHAR(120) NOT NULL,
            "PackageCode" VARCHAR(20),
            "CreatedAt" TIMESTAMP NOT NULL
        );

        CREATE INDEX IF NOT EXISTS "IX_applications_CreatedAt" ON applications ("CreatedAt");
        """;
}
