using EntityFramework.Exceptions.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Resumer.models;

public sealed class ResumeContext: DbContext
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
    public DbSet<TypstTemplate> Templates { get; set; }


    public string DbPath { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}").UseExceptionProcessor();
        optionsBuilder.ConfigureWarnings(builder =>
            builder.Log(CoreEventId.QueryExecutionPlanned, CoreEventId.PropertyChangeDetected, CoreEventId.QueryCanceled));
    #if DEBUG
        optionsBuilder.ConfigureWarnings(builder => builder.Throw());
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
    #endif
    }
}