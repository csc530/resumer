using System.Data;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace resume_builder.models;

public partial class Database
{
	public IEnumerable<Job> GetJobs()
	{
		var dataReader = new SqliteCommand($"SELECT * FROM jobs", MainConnection).ExecuteReader();
		List<Job> jobs = new();
		while(dataReader.Read())
		{
			var jobTitle = dataReader.GetString(GetSqlColumnName<Job>(nameof(Job.Title)));
			var company = dataReader.GetString(GetSqlColumnName<Job>(nameof(Job.Company)));
			var description = dataReader.GetString(GetSqlColumnName<Job>(nameof(Job.Description)));
			var experience = dataReader.GetString(GetSqlColumnName<Job>(nameof(Job.Experience)));
			var startDate = dataReader.GetDateTime(GetSqlColumnName<Job>(nameof(Job.StartDate))).ToDateOnly();
			var endDate = dataReader.GetDateTime(GetSqlColumnName<Job>(nameof(Job.EndDate))).ToDateOnly();
			jobs.Add(new Job(jobTitle, startDate, endDate, company, description, experience));
		}

		return jobs;
	}

	public bool AddJob(Job job)
	{
		var cmd = MainConnection.CreateCommand();
		List<PropertyInfo> properties = GetArgumentValueProperties(job);
		string columnNames = string.Join(",", GetSqlColumnNames(properties));
		IEnumerable<string> propertyNames = properties.Select(property => $":{property.Name}");
		string prefixedPropertyNames = string.Join(",", propertyNames);
		cmd.CommandText = $"INSERT INTO jobs({columnNames}) VALUES ({prefixedPropertyNames});";
		foreach(var item in properties)
		{
			if(item.Name == nameof(Job.Company) && job.Company != null&& GetCompany(job.Company) is null)
				AddCompany((string)item.GetValue(job)!); //not null from GetArgValueProperties method
			cmd.Parameters.AddWithValue($":{item.Name}", item.GetValue(job));
			Console.WriteLine(item.Name + ": " + (item.GetValue(job) ?? "<null>"));
		}


		Console.WriteLine(cmd.CommandText);
		cmd.Prepare();
		Console.WriteLine(cmd.ExecuteNonQuery());
		return true;
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