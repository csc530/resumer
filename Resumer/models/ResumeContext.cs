using Microsoft.EntityFrameworkCore;

namespace Resumer.models;

public class ResumeContext: DbContext
{
    public ResumeContext()
    {
        string path;
    #if DEBUG
        path = Directory.GetCurrentDirectory();
    #else
        path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    #endif
        DbPath = Path.Join(path, "resume.db");
    }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Skill> Skills { get; set; }

    public string DbPath { get; }

// The following configures EF to create a Sqlite database file in the
// special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}