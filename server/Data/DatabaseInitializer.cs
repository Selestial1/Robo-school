using Microsoft.EntityFrameworkCore;
using RoboSchool.Models;

namespace RoboSchool.Data;

public static class DatabaseInitializer
{
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
                    PhotoUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=560&h=400&fit=crop",
                    Bio = "Ирина — педагог с 8-летним опытом преподавания робототехники в начальной школе. Разрабатывает УМК и проводит практические занятия для учителей по LEGO Education и Arduino.",
                },
                new Trainer
                {
                    Slug = "marina",
                    Name = "Марина Орлова",
                    Role = "преподаватель по робототехнике",
                    PhotoUrl = "https://images.unsplash.com/photo-1580489944761-15a19d654956?w=560&h=400&fit=crop",
                    Bio = "Марина специализируется на проектной деятельности и соревнованиях по робототехнике. Помогает педагогам внедрять робототехнику в учебный процесс с нуля.",
                },
                new Trainer
                {
                    Slug = "maxim",
                    Name = "Максим Петров",
                    Role = "преподаватель по программированию",
                    PhotoUrl = "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=560&h=400&fit=crop",
                    Bio = "Максим обучает Scratch, Python и основам алгоритмизации для детей 6–12 лет. Автор методических материалов по программированию для начальной школы.",
                },
                new Trainer
                {
                    Slug = "konstantin",
                    Name = "Константин Назаров",
                    Role = "преподаватель по робототехнике",
                    PhotoUrl = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=560&h=400&fit=crop",
                    Bio = "Константин — инженер-педагог с опытом работы в R:ED LAB. Проводит практику для слушателей курсов и помогает освоить оборудование учебных классов.",
                },
                new Trainer
                {
                    Slug = "liza",
                    Name = "Лиза Весенняя",
                    Role = "преподаватель по программированию",
                    PhotoUrl = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=560&h=400&fit=crop",
                    Bio = "Лиза ведёт курсы по визуальному программированию и цифровой грамотности. Уделяет внимание адаптации материала под разный уровень подготовки детей.",
                }
            );
        }

        await db.SaveChangesAsync();
    }
}
