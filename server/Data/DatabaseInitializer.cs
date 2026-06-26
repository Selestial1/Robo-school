using Microsoft.EntityFrameworkCore;
using RoboSchool.Models;

namespace RoboSchool.Data;

public static class DatabaseInitializer
{
    private static readonly Dictionary<string, string> LocalTrainerPhotos = new(StringComparer.OrdinalIgnoreCase)
    {
        ["irina"] = "/assets/css/unsplash_IF9TK5Uy-KI.jpg",
        ["marina"] = "/assets/css/unsplash_OhKElOkQ3RE.jpg",
        ["maxim"] = "/assets/css/unsplash_Z_bTArFy6ks.jpg",
        ["konstantin"] = "/assets/css/unsplash_Zz5LQe-VSMY.jpg",
        ["liza"] = "/assets/css/unsplash_rriAI0nhcbc.jpg",
    };

    private static string LocalPhotoFor(string slug) =>
        LocalTrainerPhotos.TryGetValue(slug, out var photo) ? photo : "/assets/css/unsplash_rriAI0nhcbc.jpg";

    public static async Task InitializeAsync(AppDbContext db, bool isPostgreSql)
    {
        if (isPostgreSql)
        {
            await db.Database.ExecuteSqlRawAsync(PostgreSqlSchema.CreateTablesSql);
        }
        else
        {
            var dataDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "data"));
            Directory.CreateDirectory(dataDir);
            await db.Database.EnsureCreatedAsync();
        }

        if (!await db.Packages.AnyAsync())
        {
            db.Packages.AddRange(
                new Package
                {
                    Code = "PRO",
                    Name = "– PRO –",
                    Price = 20000,
                    Description = "УМК по робототетхнике и программированию",
                },
                new Package
                {
                    Code = "ROBO",
                    Name = "– ROBO –",
                    Price = 15000,
                    Description = "УМК по робототетхнике",
                },
                new Package
                {
                    Code = "PROG",
                    Name = "– PROG –",
                    Price = 10000,
                    Description = "УМК по программированию",
                }
            );
        }

        if (!await db.Trainers.AnyAsync())
        {
            db.Trainers.AddRange(
                new Trainer
                {
                    Slug = "irina",
                    Name = "Ирина Лайм",
                    Role = "преподаватель по робототехнике",
                    PhotoUrl = LocalPhotoFor("irina"),
                    Bio = "Ирина — педагог с 8-летним опытом преподавания робототехники в начальной школе. Разрабатывает УМК и проводит практические занятия для учителей по LEGO Education и Arduino.",
                },
                new Trainer
                {
                    Slug = "marina",
                    Name = "Марина Орлова",
                    Role = "преподаватель по робототехнике",
                    PhotoUrl = LocalPhotoFor("marina"),
                    Bio = "Марина специализируется на проектной деятельности и соревнованиях по робототехнике. Помогает педагогам внедрять робототехнику в учебный процесс с нуля.",
                },
                new Trainer
                {
                    Slug = "maxim",
                    Name = "Максим Петров",
                    Role = "преподаватель по программированию",
                    PhotoUrl = LocalPhotoFor("maxim"),
                    Bio = "Максим обучает Scratch, Python и основам алгоритмизации для детей 6–12 лет. Автор методических материалов по программированию для начальной школы.",
                },
                new Trainer
                {
                    Slug = "konstantin",
                    Name = "Константин Назаров",
                    Role = "преподаватель по робототехнике",
                    PhotoUrl = LocalPhotoFor("konstantin"),
                    Bio = "Константин — инженер-педагог с опытом работы в R:ED LAB. Проводит практику для слушателей курсов и помогает освоить оборудование учебных классов.",
                },
                new Trainer
                {
                    Slug = "liza",
                    Name = "Лиза Весенняя",
                    Role = "преподаватель по программированию",
                    PhotoUrl = LocalPhotoFor("liza"),
                    Bio = "Лиза ведёт курсы по визуальному программированию и цифровой грамотности. Уделяет внимание адаптации материала под разный уровень подготовки детей.",
                }
            );
        }

        foreach (var trainer in await db.Trainers.ToListAsync())
        {
            if (trainer.PhotoUrl.Contains("unsplash.com", StringComparison.OrdinalIgnoreCase)
                && LocalTrainerPhotos.TryGetValue(trainer.Slug, out var localPhoto))
            {
                trainer.PhotoUrl = localPhoto;
            }
        }

        await db.SaveChangesAsync();
    }
}
