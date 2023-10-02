using Microsoft.Data.Sqlite;

namespace resume_builder.models.database;

//todo: add catch for sqliteexceptions on db queries and updates, etc.
public sealed partial class Database : IDisposable, IAsyncDisposable
{
	private const string SqliteFileName = "resume.sqlite";

	private static readonly Dictionary<string, List<string>> TemplateTableStructure = new()
	{
		{
			"job", new List<string> { "id", "company", "title", "startDate", "endDate", "description", "experience" }
		},
		{ "name", new() { "name", "type" } },
		{ "company", new() { "name" } },
		{ "job_skills", new() { "jobID", "skillID" } },
		{
			"profile",
			new() { "id", "firstName", "middleName", "lastName", "phoneNumber", "email", "website", "summary" }
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
		MainConnection.Open();
	}

	private SqliteConnection MainConnection { get; }

	async ValueTask IAsyncDisposable.DisposeAsync()
	{
		await MainConnection.DisposeAsync();
		SqliteConnection.ClearPool(MainConnection);
		GC.SuppressFinalize(this);
	}

	public void Dispose()
	{
		MainConnection.Dispose();
		SqliteConnection.ClearPool(MainConnection);
		GC.SuppressFinalize(this);
	}

	public bool IsInitialized() => IsInitialized(MainConnection);

	public void Initialize()
	{
		if(IsInitialized())
			return;
		var cmd = MainConnection.CreateCommand();
		//todo: if there exists a previous db file that it must overwrite, do it, etc.
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
		using var tableNamesReader = tableNamesCmd.ExecuteReader();
		if(!tableNamesReader.HasRows)
			return false;

		List<string> dbTables = new();
		while(tableNamesReader.Read())
			dbTables.Add((string)tableNamesReader["name"]);
		if(!TemplateTableStructure.Keys.ToList()
		                          .TrueForAll(name => dbTables.Contains(name, StringComparer.OrdinalIgnoreCase)))
			return false;

		using var columnNamesCmd = connection.CreateCommand();
		foreach(var tableName in dbTables)
		{
			columnNamesCmd.CommandText = $"PRAGMA TABLE_INFO(\"{tableName}\");";
			columnNamesCmd.Prepare();
			using var tableInfoReader = columnNamesCmd.ExecuteReader();
			if(!tableInfoReader.HasRows)
				return false;
			var columnNames = new List<string>();
			while(tableInfoReader.Read())
			{
				var columnName = (string)tableInfoReader["name"];
				columnNames.Add(columnName);
			}

			if(!TemplateTableStructure[tableName]
				   .TrueForAll(name => columnNames.Contains(name, StringComparer.OrdinalIgnoreCase)))
				return false;
		}

		return true;
	}


	~Database() => Dispose();

	private SqliteCommand GetJobsLikeQuery(string? jobTitle, DateOnly? startDate, DateOnly? endDate, string? company,
	                                       string? description, string? experience)
	{
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "SELECT * FROM job WHERE ";
		var conditions = new List<string>();
		if(company != null)
		{
			conditions.Add("company LIKE $company");
			cmd.Parameters.AddWithValue("$company", company.Surround("%"));
		}

		if(jobTitle != null)
		{
			conditions.Add("title LIKE $title");
			cmd.Parameters.AddWithValue("$title", jobTitle.Surround("%"));
		}

		if(description != null)
		{
			conditions.Add("description LIKE $desc");
			cmd.Parameters.AddWithValue("$desc", description.Surround("%"));
		}

		if(experience != null)
		{
			conditions.Add("experience LIKE $exp");
			cmd.Parameters.AddWithValue("$exp", experience.Surround("%"));
		}

		if(startDate != null)
		{
			conditions.Add("startDate IS $start ");
			cmd.Parameters.AddWithValue("$start", startDate);
		}

		if(endDate != null)
		{
			conditions.Add("endDate IS $end");
			cmd.Parameters.AddWithValue("$end", endDate);
		}

		cmd.CommandText += string.Join(" OR ", conditions);
		return cmd;
	}
}