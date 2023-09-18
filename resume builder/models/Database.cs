using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace resume_builder;

public class Database //perhaps implement IDisposable
{
    private const string SqliteFileName = "resume.sqlite";

    ///todo: edit user table to entries and not columns
    private static readonly List<string> RequiredTables = new() { "companies", "jobs", "skills", "job_skills", "user" };

    public Database(string? path = ".")
    {
        path ??=
        #if DEBUG
            "."
        #else
            Path.GetFullPath("resume_builder", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create));
        #endif
            ;

        if(!Path.Exists(path))
            Directory.CreateDirectory(path);

        SqliteConnectionStringBuilder sqliteConnectionStringBuilder = new()
        {
            DataSource = Path.Combine(path, SqliteFileName),
            Mode = SqliteOpenMode.ReadWriteCreate
        };

        MainConnection = new SqliteConnection(sqliteConnectionStringBuilder.ConnectionString);
        sqliteConnectionStringBuilder.DataSource = Path.Combine(path, $"backup_{SqliteFileName}");
        BackupConnection = new SqliteConnection(sqliteConnectionStringBuilder.ToString());

        Open();
    }

    private SqliteConnection MainConnection { get; }
    private SqliteConnection BackupConnection { get; }

    ~Database()
    {
        MainConnection.Close();
        BackupConnection.Close();
    }

    public bool IsInitialized()=>HasRequiredTables(MainConnection);

    private void Open()
    {
        MainConnection.Open();
        BackupConnection.Open();
    }

    public bool AddJob(Job job)
    {
        Open();
        var cmd = MainConnection.CreateCommand();
        cmd.CommandText =
            """INSERT INTO main.jobs(company, title, "start date", "end date", "job description", experience) VALUES ($Company,:Title,@start,@end,@descr,$exp);""";
        var properties = typeof(Job).GetProperties();
        foreach(var item in properties)
        {
            cmd.Parameters.AddWithValue(item.Name, item.GetValue(job));
            Debug.WriteLine(item.Name + " : " +item.GetValue(job));
        }

        Console.WriteLine(cmd.CommandText);
        cmd.Prepare();
        Console.WriteLine(cmd.CommandText);
        Close();
        return true;
        throw new NotImplementedException();
    }

    private void Close()
    {
        MainConnection.Close();
        BackupConnection.Close();
    }

    public void Initialize()
    {
        if(IsInitialized())
            return;
        var cmd = MainConnection.CreateCommand();
        cmd.CommandText = File.ReadAllText("tables.sql");
        //cmd.Prepare();
        cmd.ExecuteNonQuery();
        MainConnection.BackupDatabase(BackupConnection);
        AnsiConsole.WriteLine("Database initialized");
        //add prompt for basic information populaation
    }

    public void RestoreBackup() => BackupConnection.BackupDatabase(MainConnection);

    private bool HasRequiredTables(SqliteConnection connection)
    {
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT DISTINCT name FROM sqlite_master WHERE type='table' AND name NOT IN ('sqlite_sequence');";
        cmd.Prepare();
        var result = cmd.ExecuteReader();
        List<string> tables = new();
        if(!result.HasRows)
        {
            result.Close();
            return false;
        }
        //check if all required tables exist, perf?
        while(result.Read())
            tables.Add((string)result["name"]);
        result.Close();
        return tables.TrueForAll(name => RequiredTables.Contains(name));
    }
    public bool BackupExists()=>HasRequiredTables(BackupConnection);
}