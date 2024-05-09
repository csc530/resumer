using Microsoft.EntityFrameworkCore;

namespace Resumer.models;

public sealed class ResumeContext : DbContext
{
    public ResumeContext()
    {
    #if DEBUG
        var path = Directory.GetCurrentDirectory();
    #else
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    #endif
        DbPath = Path.Join(path, "resume.db");
    }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Skill> Skills { get; set; }

    public string DbPath { get; }

// The following configures EF to create a sqlite database file in the
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}