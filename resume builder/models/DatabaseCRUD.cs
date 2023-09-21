using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
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

	private List<Job> ParseJobsFromQuery(SqliteDataReader dataReader)
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

	public bool AddJob(Job job)
	{
		var cmd = MainConnection.CreateCommand();

		//todo skip null value properties
		var propColPairs = GetPropertySqlColumnNamePairs<Job>(job, escapeSqlColumnNames: true);
		var columnNames = string.Join(",", propColPairs.Values);
		const string prefix = "$";
		var jobPropertiesNames = propColPairs.Keys.ToList();
		var placeholders = jobPropertiesNames.Prefix(prefix);
		cmd.CommandText = $"INSERT INTO jobs({columnNames}) VALUES ({string.Join(",", placeholders)});";

		BindCommandParameters(job, jobPropertiesNames, cmd, placeholders);
		if(!string.IsNullOrWhiteSpace(job.Company) && GetCompany(job.Company) == null)
			AddCompany(job.Company);

		Console.WriteLine(cmd.CommandText);
		cmd.Prepare();
		Console.WriteLine(cmd.ExecuteNonQuery());
		return true;
	}


	public List<Job> GetJob(Job job)
	{
		var cmd = MainConnection.CreateCommand();
		var filter = CreateSqlWhereString(job, cmd);
		cmd.CommandText = $"SELECT * from jobs WHERE {filter};";
		cmd.Prepare();
		var data = cmd.ExecuteReader();
		return ParseJobsFromQuery(data).ToList();
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
		foreach(var table in RequiredTables)
		{
			cmd.CommandText = $"DELETE FROM \"{table}\";";
			cmd.ExecuteNonQuery();
		}
	}
}