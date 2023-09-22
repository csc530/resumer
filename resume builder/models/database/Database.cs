using System.Data.Common;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace resume_builder.models;

public sealed partial class Database : IDisposable, IAsyncDisposable
{
	private const string SqliteFileName = "resume.sqlite";

	private static readonly Dictionary<string, List<string>> TemplateTableStructure = new()
	{
		{
			"jobs", new List<string> { "id", "company", "title", "start date", "end date", "description", "experience" }
		},
		{ "skills", new() { "skill" } },
		{ "companies", new() { "name" } },
		{ "job_skills", new() { "jobID", "skillID" } },
		{
			"user",
			new() { "id", "first name", "middle name", "last name", "phone number", "email", "website", "summary" }
		}
	};

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
		Open();
	}

	private SqliteConnection MainConnection { get; }

	async ValueTask IAsyncDisposable.DisposeAsync()
	{
		await MainConnection.DisposeAsync();
	}

	public void Dispose()
	{
		Close();
		MainConnection.Dispose();
		SqliteConnection.ClearPool(MainConnection);
	}

	public bool IsInitialized() => IsInitialized(MainConnection);

	private void Open()
	{
		MainConnection.Open();
	}


	private void Close()
	{
		MainConnection.Close();
	}

	public void Initialize()
	{
		if(IsInitialized())
			return;
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText = File.ReadAllText("tables.sql");
		cmd.ExecuteNonQuery();
	}

	/// <summary>
	/// check if a database has the required tables to be used for resume builder app
	/// </summary>
	/// <param name="connection">database connection to check</param>
	/// <returns>true if it has the required tables for the app</returns>
	private static bool IsInitialized(SqliteConnection connection)
	{
		using var tableNamesCmd = connection.CreateCommand();
		tableNamesCmd.CommandText =
			"SELECT DISTINCT name FROM sqlite_master WHERE type='table' AND name NOT IN ('sqlite_sequence');";
		using var result = tableNamesCmd.ExecuteReader();
		if(!result.HasRows)
			return false;

		List<string> tables = new();
		while(result.Read())
			tables.Add((string)result["name"]);
		if(!TemplateTableStructure.Keys.ToList()
		                          .TrueForAll(name => tables.Contains(name, StringComparer.OrdinalIgnoreCase)))
			return false;

		using var columnNamesCmd = connection.CreateCommand();
		foreach(string tableName in tables)
		{
			columnNamesCmd.CommandText = $"PRAGMA TABLE_INFO(\"{tableName}\");";
			columnNamesCmd.Prepare();
			using var tableInfo = columnNamesCmd.ExecuteReader();
			if(!tableInfo.HasRows)
				return false;
			var columnNames = new List<string>();
			while(tableInfo.Read())
			{
				var columnName = (string)tableInfo["name"];
				columnNames.Add(columnName);
			}

			if(!TemplateTableStructure[tableName]
				   .TrueForAll(name => columnNames.Contains(name, StringComparer.OrdinalIgnoreCase)))
				return false;
		}

		return true;
	}

	~Database() => Dispose();
}