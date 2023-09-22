using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using Microsoft.Data.Sqlite;

namespace resume_builder.models;

public partial class Database
{
	public List<Job> GetJobs()
	{
		SqliteCommand sqliteCommand = new SqliteCommand($"SELECT * FROM jobs", MainConnection);
		var dataReader = sqliteCommand.ExecuteReader();
		return ParseJobsFromQuery(dataReader).ToList();
	}

	private List<Job> ParseJobsFromQuery(DbDataReader dataReader)
	{
		List<Job> jobs = new();
		while(dataReader.Read())
		{
			var dic = GetPropertySqlColumnNamePairs<Job>();
			Dictionary<string, dynamic> dbPropertyValuePairs = new();
			foreach(var (property, sqlColumnName) in dic)
				if(!dataReader.IsDBNull(sqlColumnName))
					dbPropertyValuePairs.Add(property, dataReader[sqlColumnName]);
			jobs.Add(Job.FromDictionary(dbPropertyValuePairs));
		}

		return jobs;
	}

	public int AddJob(Job job)
	{
		if(!string.IsNullOrWhiteSpace(job.Company) && GetCompany(job.Company) == null)
			AddCompany(job.Company);

		return AddData(job, "jobs");
	}

	private int AddData(dynamic obj, string tableName)
	{
		var cmd = MainConnection.CreateCommand();

		Dictionary<string, string> propertySqlColumnNamePairs =
			GetPropertySqlColumnNamePairs(obj, escapeSqlColumnNames: true);
		var columnNames = string.Join(",", propertySqlColumnNamePairs.Values);

		var jobPropertyNames = propertySqlColumnNamePairs.Keys.ToList();
		var placeholders = jobPropertyNames.Prefix("$");

		cmd.CommandText = $"INSERT INTO {tableName}({columnNames}) VALUES ({string.Join(",", placeholders)});";

		var propertyPlaceholderNamePairs = jobPropertyNames.Zip(placeholders);
		BindCommandParameters(obj, cmd, propertyPlaceholderNamePairs);
		cmd.Prepare();
		return cmd.ExecuteNonQuery();
	}


	public List<Job> GetJob(Job job) => ParseJobsFromQuery(GetData("jobs", job)).ToList();

	/// <summary>
	/// retrieve rows from the given table like the given object,
	/// with columns matching the object's properties
	/// if no object is given, retrieve all rows
	/// </summary>
	/// <param name="tableName">sqlite table name</param>
	/// <param name="obj">reference object to filter rows based on similarity to its properties</param>
	/// <returns>a <see cref="DbDataReader"/> with the rows from the table</returns>
	private DbDataReader GetData(string tableName, dynamic? obj = null)
	{
		var cmd = MainConnection.CreateCommand();

		if(obj == null)
		{
			cmd.CommandText = $"SELECT * from {tableName};";
			return cmd.ExecuteReader();
		}

		var whereFilter = new List<string>();
		Dictionary<string, string> propertySqlColumnNamePairs =
			GetPropertySqlColumnNamePairs(obj, escapeSqlColumnNames: true);
		foreach(var (propertyName, sqlColumnName) in propertySqlColumnNamePairs)
		{
			object? value = ((PropertyInfo?)obj.GetType().GetProperty(propertyName))?.GetValue(obj);
			whereFilter.Add($"{sqlColumnName} = @{propertyName}");
			cmd.Parameters.AddWithValue($"@{propertyName}", value);
		}

		cmd.CommandText = $"SELECT * from {tableName} WHERE {string.Join(" AND", whereFilter)};";
		cmd.Prepare();
		return cmd.ExecuteReader();
	}


	private string? GetCompany(string company)
	{
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "SELECT name FROM companies WHERE name == @company";
		cmd.Parameters.AddWithValue("@company", company);
		cmd.Prepare();
		return (string?)cmd.ExecuteScalar();
	}

	private SQLResult AddCompany(string company)
	{
		//todo: make sure company is not null or empty
		if(string.IsNullOrWhiteSpace(company))
			return SQLResult.invalid;
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "INSERT INTO companies(name) VALUES ($Company)";
		cmd.Parameters.AddWithValue("$Company", company);
		cmd.Prepare();
		cmd.ExecuteNonQuery();
		cmd.CommandText = "SELECT last_insert_rowid()";
		cmd.Prepare();
		cmd.ExecuteScalar();
		return SQLResult.success;
	}

	public void Wipe()
	{
		var cmd = MainConnection.CreateCommand();
		foreach(var table in TemplateTableStructure.Keys)
			try
			{
				cmd.CommandText = $"DELETE FROM \"{table}\";";
				cmd.ExecuteNonQuery();
			}
			catch(Exception)
			{
				if(IsInitialized())
					throw;
			}
	}
}