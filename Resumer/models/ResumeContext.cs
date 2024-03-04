using Microsoft.EntityFrameworkCore;

namespace Resumer.models;

public class ResumeContext: DbContext
{
    public ResumeContext()
    {

    #if DEBUG
        DbPath = Directory.GetCurrentDirectory();
    #else
        const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "resume.db");
    #endif
        DbPath = Path.Join(DbPath, "resume.db");
    }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Skill> Skills { get; set; }

    public string DbPath { get; }

// The following configures EF to create a Sqlite database file in the
// special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}