using Microsoft.EntityFrameworkCore;

namespace resume_builder.models;

public class ResumeContext: DbContext
{
    public ResumeContext()
    {
        const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
    #if DEBUG
        DbPath = "TestResumeBuilder.db";
    #else
        DbPath = System.IO.Path.Join(path, "resume.db");
    #endif
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