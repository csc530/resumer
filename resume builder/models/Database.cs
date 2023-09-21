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

	async ValueTask IAsyncDisposable.DisposeAsync()
	{
		await MainConnection.DisposeAsync();
		await BackupConnection.DisposeAsync();
	}

	public void Dispose()
	{
		Close();
		MainConnection.Dispose();
		BackupConnection.Dispose();
	}

	public bool IsInitialized() => HasRequiredTables(MainConnection);

	private void Open()
	{
		MainConnection.Open();
		BackupConnection.Open();
	}


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

	~Database() => Dispose();

	/// <summary>
	/// get the <see cref="SqlColumnNameAttribute"/> of the given property on <see cref="T"/>'s class
	/// </summary>
	/// <param name="property">The property who has a related sql column name</param>
	/// <typeparam name="T">class that has the given <see cref="property"/>, modeled by a sql table</typeparam>
	/// <returns>the escaped (double quote surrounded) sql column name, or null if the property could not be found on the class or the property does not have a <see cref="SqlColumnNameAttribute"/></returns>
	private string? GetSqlColumnName<T>(string property) =>
		typeof(T).GetProperty(property)?.GetCustomAttribute<SqlColumnNameAttribute>()?.Name;

	/// <summary>
	/// gets the sql column names from a type and it's associated property name
	///
	/// </summary>
	/// <param name="obj">the object to parse it's properties sql column names and check for null properties</param>
	/// <param name="escapeSqlColumnNames">if the sql column name should be escaped - surrounded with quotes</param>
	/// <param name="skipNullProperties">if true will only return the sql column names that have a value in the object</param>
	/// <typeparam name="T">the class that is modeled by a sql table and has properties annotated by <see cref="SqlColumnNameAttribute"/></typeparam>
	/// <returns>A dictionary of the property names (key) and sql column names (value)</returns>
	private Dictionary<string, string> GetPropertySqlColumnNamePairs<T>(T obj, bool escapeSqlColumnNames = false,
	                                                                    bool skipNullProperties = true) =>
		typeof(T).GetProperties()
		         .Where(property => property.GetCustomAttribute<SqlColumnNameAttribute>() != null)
		         //if were not skipping null prop values, then always true; else check for null
		         .Where(property => !skipNullProperties || property.GetValue(obj) != null)
		         .ToDictionary(
			         property => property.Name, //key
			         property => escapeSqlColumnNames
				         ? property.GetCustomAttribute<SqlColumnNameAttribute>()!.EscapedName
				         : property.GetCustomAttribute<SqlColumnNameAttribute>()!.Name //value
		         );
	private Dictionary<string, string> GetPropertySqlColumnNamePairs<T>(bool escapeSqlColumnNames = false) =>
		typeof(T).GetProperties()
		         .Where(property => property.GetCustomAttribute<SqlColumnNameAttribute>() != null)
		         //if were not skipping null prop values, then always true; else check for null
		         .ToDictionary(
			         property => property.Name, //key
			         property => escapeSqlColumnNames
				         ? property.GetCustomAttribute<SqlColumnNameAttribute>()!.EscapedName
				         : property.GetCustomAttribute<SqlColumnNameAttribute>()!.Name //value
		         );

	private static void BindCommandParameters<T>(T obj, IReadOnlyList<string> jobPropertiesNames, SqliteCommand cmd,
	                                             IReadOnlyList<string> placeholders)
	{
		for(var i = 0; i < jobPropertiesNames.Count; i++)
		{
			var value = obj?.GetType().GetProperty(jobPropertiesNames[i])?.GetValue(obj);
			cmd.Parameters.AddWithValue(placeholders[i], value);
			Console.WriteLine($"{placeholders[i]}: {value}");
		}
	}

	private string CreateSqlWhereString<T>(T obj, SqliteCommand cmd)
	{
		var propertySqlColumnNamePairs = GetPropertySqlColumnNamePairs(obj, escapeSqlColumnNames: true);
		List<string> text = new();
		foreach(var (propertyName, sqlColumnName) in propertySqlColumnNamePairs)
		{
			var value = typeof(T).GetProperty(propertyName)?.GetValue(obj);
			text.Add($"{sqlColumnName} = ${propertyName}"); // ex. id = $id
			cmd.Parameters.AddWithValue($"${propertyName}", value);
		}

		return string.Join(" AND", text);
	}
}