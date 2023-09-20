using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace resume_builder.models;

public sealed partial class Database : IDisposable, IAsyncDisposable
{
	private const string SqliteFileName = "resume.sqlite";

	///todo: edit user table to entries and not columns
	private static readonly List<string>
		RequiredTables = new() { "jobs", "skills", "job_skills", "user", "companies", };

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

	public bool IsInitialized() => HasRequiredTables(MainConnection);

	private void Open()
	{
		MainConnection.Open();
		BackupConnection.Open();
	}

	private static string[] GetSqlColumnNames(IEnumerable<PropertyInfo> properties) =>
		properties // the display name is used to hold the related sql column name in this project*/
			.Select(property => property.GetCustomAttribute<SqlColumnNameAttribute>()?.Name)
			.Where(name => !string.IsNullOrWhiteSpace(name))
			.ToArray()!;//don't know why it needs the suppressione🙄

	private static List<PropertyInfo> GetArgumentValueProperties<T>(T argument) =>
		typeof(T)
			.GetProperties()
			.Where(property => property.GetValue(argument) != null)
			.ToList();

	public void Close()
	{
		MainConnection.Close();
		BackupConnection.Close();
		BackupConnection.Dispose();
		BackupConnection.Dispose();
		SqliteConnection.ClearPool(MainConnection);
		SqliteConnection.ClearPool(BackupConnection);
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
		//add prompt for basic information populaation
	}

	public void RestoreBackup() => BackupConnection.BackupDatabase(MainConnection);

	private bool HasRequiredTables(SqliteConnection connection)
	{
		var cmd = connection.CreateCommand();
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

	public bool BackupExists() => HasRequiredTables(BackupConnection);

	public void Dispose()
	{
		Close();
		MainConnection.Dispose();
		BackupConnection.Dispose();
	}

	async ValueTask IAsyncDisposable.DisposeAsync()
	{
		await MainConnection.DisposeAsync();
		await BackupConnection.DisposeAsync();
	}

	~Database() => Dispose();

	public string? GetJob(Job job)
	{
		throw new NotImplementedException();
	}

	private string? GetSqlColumnName(Type type, string propertyName)
	{
		return type.GetProperty(propertyName)?.GetCustomAttribute<SqlColumnNameAttribute>()?.Name;
	}

	private string? GetSqlColumnName<T>(string propertyName)
	{
		return GetSqlColumnName(typeof(T), propertyName);
	}
}