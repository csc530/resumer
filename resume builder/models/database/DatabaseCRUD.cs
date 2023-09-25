using Microsoft.Data.Sqlite;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace resume_builder.models;

public partial class Database
{
	public int AddJob(Job job)
	{
		if(!string.IsNullOrWhiteSpace(job.Company) && GetCompany(job.Company) == null)
			AddCompany(job.Company);
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText =
			"INSERT INTO job(title,company,startDate) VALUES($title,$company,$start);";
		cmd.Parameters.AddWithValue("$title", job.Title);
		cmd.Parameters.AddWithNullableValue("$company", job.Company);
		cmd.Parameters.AddWithNullableValue("$start", job.StartDate);
		cmd.Parameters.AddWithNullableValue("end", job.EndDate);
		cmd.Parameters.AddWithNullableValue("$desc", job.Description);
		cmd.Parameters.AddWithNullableValue("$exp", job.Experience);
		cmd.Prepare();
		return cmd.ExecuteNonQuery();
	}


	public List<Job> GetJobs(Job? job = null)
	{
		using var cmd = MainConnection.CreateCommand();
		if(job == null)
			cmd.CommandText = "SELECT * FROM job";
		else
		{
			cmd.CommandText =
				"SELECT * FROM job WHERE title = $title AND company IS $company AND startDate IS $start AND endDate IS $end AND description IS $desc AND experience IS $exp";
			cmd.Parameters.AddWithValue("$title", job.Title);
			cmd.Parameters.AddWithValue("company", job.Company);
			cmd.Parameters.AddWithValue("$start", job.StartDate);
			cmd.Parameters.AddWithValue("end", job.EndDate);
			cmd.Parameters.AddWithValue("$desc", job.Description);
			cmd.Parameters.AddWithValue("$exp", job.Experience);
			cmd.Prepare();
		}

		using var data = cmd.ExecuteReader();
		var jobs = new List<Job>();
		while(data.Read())
			jobs.Add(Job.ParseJobsFromQuery(data));
		return jobs;
	}

	public Job? GetJob(Job job)
	{
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText =
			"SELECT * FROM job WHERE title = $title AND company IS $company AND startDate IS $start AND endDate IS $end AND description IS $desc AND experience IS $exp";
		cmd.Parameters.AddWithNullableValue("$title", job.Title);
		cmd.Parameters.AddWithNullableValue("company", job.Company);
		cmd.Parameters.AddWithNullableValue("$start", job.StartDate);
		cmd.Parameters.AddWithNullableValue("end", job.EndDate);
		cmd.Parameters.AddWithNullableValue("$desc", job.Description);
		cmd.Parameters.AddWithNullableValue("$exp", job.Experience);
		cmd.Prepare();
		var data = cmd.ExecuteReader();
		data.Read();
		return Job.ParseJobsFromQuery(data);
	}


	private string? GetCompany(string company)
	{
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "SELECT name FROM company WHERE name == @company";
		cmd.Parameters.AddWithValue("@company", company);
		cmd.Prepare();
		return (string?)cmd.ExecuteScalar();
	}

	private SQLResultCode AddCompany(string company)
	{
		if(string.IsNullOrWhiteSpace(company))
			throw new ArgumentException("company is null or empty", nameof(company));
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "INSERT INTO company(name) VALUES ($Company)";
		cmd.Parameters.AddWithValue("$Company", company);
		cmd.Prepare();
		cmd.ExecuteNonQuery();
		cmd.CommandText = "SELECT last_insert_rowid()";
		cmd.Prepare();
		cmd.ExecuteScalar();
		return SQLResultCode.Success;
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

	public int AddProfile(Profile profile)
	{
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText =
			"Insert into profile(firstName, middleName, lastName, phoneNumber, email, website, summary) VALUES ($firstName, $middleName, $lastName, $phoneNumber, $email, $website, $summary)";
		cmd.Parameters.AddWithNullableValue("$firstName", profile.FirstName);
		cmd.Parameters.AddWithNullableValue("$middleName", profile.MiddleName);
		cmd.Parameters.AddWithNullableValue("$lastName", profile.LastName);
		cmd.Parameters.AddWithNullableValue("$phoneNumber", profile.PhoneNumber);
		cmd.Parameters.AddWithNullableValue("$email", profile.EmailAddress);
		cmd.Parameters.AddWithNullableValue("$website", profile.Website);
		cmd.Parameters.AddWithNullableValue("$summary", profile.Summary);
		cmd.Prepare();
		return cmd.ExecuteNonQuery();
	}

	public List<Profile> GetProfiles()
	{
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "SELECT * FROM profile";
		using var data = cmd.ExecuteReader();
		var profiles = new List<Profile>();
		while(data.Read())
		{
			var profile = Profile.ParseProfilesFromQuery(data);
			if(profile == null)
				continue;
			profiles.Add(profile);
		}

		return profiles;
	}

	public void AddSkill(Skill skill)
	{
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "INSERT INTO skill(skill, type) VALUES ($name, $type)";
		cmd.Parameters.AddWithValue("$name", skill.Name);
		cmd.Parameters.AddWithValue("$type", skill.Type?.ToString());
		cmd.Prepare();
		cmd.ExecuteNonQuery();
	}

	public List<Job> SearchJobs(string? jobTitle = null, DateOnly? startDate = null, DateOnly? endDate = null,
	                            string? company = null, string? description = null, string? experience = null,
	                            params string[]? terms)
	{
		if(terms == null || terms.Length == 0)
			if(jobTitle != null && startDate != null)
				return GetJobs(new Job(jobTitle, startDate, endDate, company, description, experience));
			else if(jobTitle == null && startDate == null && endDate == null && company == null &&
			        description == null && experience == null)
				return GetJobs();
			else
			{
				using var cmd = MainConnection.CreateCommand();
				cmd.CommandText = "SELECT * FROM job WHERE ";
				var filter = new List<string>();

				if(jobTitle != null)
				{
					filter.Add("title LIKE $title");
					cmd.Parameters.AddWithNullableValue("$title", jobTitle.Surround("%"));
				}

				if(startDate != null)
				{
					filter.Add("startDate IS $start");
					cmd.Parameters.AddWithNullableValue("$start", startDate);
				}

				if(endDate != null)
				{
					filter.Add("endDate IS $end");
					cmd.Parameters.AddWithNullableValue("$end", endDate);
				}

				if(company != null)
				{
					filter.Add("company LIKE $company");
					cmd.Parameters.AddWithNullableValue("$company", company.Surround("%"));
				}

				if(description != null)
				{
					filter.Add("description LIKE $desc");
					cmd.Parameters.AddWithNullableValue("$desc", description.Surround("%"));
				}

				if(experience != null)
				{
					filter.Add("experience LIKE $exp");
					cmd.Parameters.AddWithNullableValue("$exp", experience.Surround("%"));
				}

				cmd.CommandText += string.Join(" AND ", filter) + ";";
				cmd.Prepare();
				var reader = cmd.ExecuteReader();
				var jobs = new List<Job>();
				while(reader.Read())
					jobs.Add(Job.ParseJobsFromQuery(reader) ?? throw new InvalidOperationException());
				return jobs;
			}
		else
		{
			var cmd = MainConnection.CreateCommand();

			cmd.CommandText = "CREATE TEMPORARY TABLE term(txt TEXT);";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "INSERT INTO term VALUES ";
			for(int i = 0; i < terms.Length; i++)
			{
				cmd.CommandText += $"($term{i})";
				cmd.Parameters.AddWithNullableValue($"$term{i}", terms[i]);
				if(i < terms.Length - 1)
					cmd.CommandText += ", ";
			}

			cmd.CommandText += ";";
			cmd.ExecuteNonQuery();

			cmd.CommandText = "SELECT * FROM job as j " +
			                  "JOIN term t ON (j.title LIKE t.txt) OR (j.company LIKE t.txt) OR (j.description LIKE t.txt) OR (j.experience LIKE t.txt)";

			if(jobTitle == null && startDate == null && endDate == null && company == null && description == null &&
			   experience == null)
			{
				cmd.Prepare();
				var reader = cmd.ExecuteReader();
				var jobs = new List<Job>();
				while(reader.Read())
					jobs.Add(Job.ParseJobsFromQuery(reader) ?? throw new InvalidOperationException());
				return jobs;
			}
			else
			{
				cmd.CommandText += " INNER JOIN job as terms ON j.id = t.id WHERE " +
				                   "(";
				cmd.CommandText += "SELECT * FROM job WHERE ";
				var filter = new List<string>();

				if(jobTitle != null)
				{
					filter.Add("title LIKE $title");
					cmd.Parameters.AddWithNullableValue("$title", jobTitle.Surround("%"));
				}

				if(startDate != null)
				{
					filter.Add("startDate IS $start");
					cmd.Parameters.AddWithNullableValue("$start", startDate);
				}

				if(endDate != null)
				{
					filter.Add("endDate IS $end");
					cmd.Parameters.AddWithNullableValue("$end", endDate);
				}

				if(company != null)
				{
					filter.Add("company LIKE $company");
					cmd.Parameters.AddWithNullableValue("$company", company.Surround("%"));
				}

				if(description != null)
				{
					filter.Add("description LIKE $desc");
					cmd.Parameters.AddWithNullableValue("$desc", description.Surround("%"));
				}

				if(experience != null)
				{
					filter.Add("experience LIKE $exp");
					cmd.Parameters.AddWithNullableValue("$exp", experience.Surround("%"));
				}

				cmd.CommandText += string.Join(" AND ", filter) + ");";
				var reader = cmd.ExecuteReader();
				var jobs = new List<Job>();
				while(reader.Read())
					jobs.Add(Job.ParseJobsFromQuery(reader)!);
				return jobs;
			}
		}
	}
}