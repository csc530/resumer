using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace resume_builder;

public class Database
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

        Connection = new SqliteConnection(sqliteConnectionStringBuilder.ConnectionString);
        sqliteConnectionStringBuilder.DataSource = Path.Combine(path, $"backup_{SqliteFileName}");
        BackupConnection = new SqliteConnection(sqliteConnectionStringBuilder.ToString());
    }

    private SqliteConnection Connection { get; }
    private SqliteConnection BackupConnection { get; }

    ~Database()
    {
        Connection.Close();
        BackupConnection.Close();
    }

    public bool IsInitialized()
    {
        var cmd = Connection.CreateCommand();
        cmd.CommandText =
            "SELECT DISTINCT name FROM sqlite_master WHERE type='table' AND name NOT IN ('sqlite_sequence');";
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

    private void Open()
    {
        Connection.Open();
        BackupConnection.Open();
    }

    public bool AddJob(Job job)
    {
        Open();
        var cmd = Connection.CreateCommand();
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
        Connection.Close();
        BackupConnection.Close();
    }

    public void Initialize()
    {
        if(IsInitialized())
            return;
        var cmd = Connection.CreateCommand();
        cmd.CommandText = File.ReadAllText("tables.sql");
        //cmd.Prepare();
        cmd.ExecuteNonQuery();
        Connection.BackupDatabase(BackupConnection);
        AnsiConsole.WriteLine("Database initialized");
        //add prompt for basic information populaation
    }

    public bool RestoreBackup()
    {
        throw new NotImplementedException();
    }
}