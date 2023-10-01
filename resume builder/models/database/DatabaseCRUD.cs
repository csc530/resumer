using Microsoft.Data.Sqlite;

namespace resume_builder.models.database;

public partial class Database
{
	public int AddJob(Job job)
	{
		if(!string.IsNullOrWhiteSpace(job.Company) && GetCompany(job.Company) == null)
			AddCompany(job.Company);
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText =
			"INSERT INTO job(title,company,startDate,endDate,description,experience) VALUES($title,$company,$start,$end,$desc,$exp);";
		cmd.Parameters.AddWithValue("$title", job.Title);
		cmd.Parameters.AddWithNullableValue("$company", job.Company);
		cmd.Parameters.AddWithNullableValue("$start", job.StartDate);
		cmd.Parameters.AddWithNullableValue("$end", job.EndDate);
		cmd.Parameters.AddWithNullableValue("$desc", job.Description);
		cmd.Parameters.AddWithNullableValue("$exp", job.Experience);
		cmd.Prepare();
		return cmd.ExecuteNonQuery();
	}


	public Dictionary<long, Job> GetJobs(Job? job = null)
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
		var jobs = new Dictionary<long, Job>();
		while(data.Read())
			jobs.Add((long)data["id"], Job.ParseJobsFromQuery(data)!);
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

	public Dictionary<long, Job> GetJob(long[] ids)
	{
		if(ids.Length == 0)
			return GetJobs();
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "CREATE TEMPORARY TABLE temp(id INTEGER);";
		cmd.ExecuteNonQuery();
		cmd.CommandText = "INSERT INTO temp(id) values ";
		for(var i = 0; i < ids.Length; i++)
		{
			cmd.CommandText += $"($id{i})";
			if(i < ids.Length - 1)
				cmd.CommandText += ", ";
			else if(i == ids.Length - 1)
				cmd.CommandText += ";";
			else
				cmd.CommandText += " ";
			cmd.Parameters.AddWithValue($"id{i}", ids[i]);
		}

		cmd.Prepare();
		cmd.ExecuteNonQuery();

		cmd.CommandText = "SELECT job.* FROM job WHERE id IN (SELECT * FROM temp);";
		var data = cmd.ExecuteReader();
		var jobs = new Dictionary<long, Job>();
		while(data.Read())
			jobs.Add((long)data["id"], Job.ParseJobsFromQuery(data) ?? throw new InvalidOperationException());
		return jobs;
	}


	private string? GetCompany(string company)
	{
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "SELECT name FROM company WHERE name == @company";
		cmd.Parameters.AddWithValue("@company", company);
		cmd.Prepare();
		return (string?)cmd.ExecuteScalar();
	}

	private void AddCompany(string company)
	{
		if(string.IsNullOrWhiteSpace(company))
			throw new ArgumentException("company is null or empty", nameof(company));
		using var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "INSERT INTO company(name) VALUES ($Company)";
		cmd.Parameters.AddWithValue("$Company", company);
		cmd.Prepare();
		cmd.ExecuteNonQuery();
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

	public Dictionary<long, Job> GetJobsLike(string? jobTitle = null, DateOnly? startDate = null,
	                                         DateOnly? endDate = null,
	                                         string? company = null, string? description = null,
	                                         string? experience = null,
	                                         params string[]? terms)
	{
		using var cmd = MainConnection.CreateCommand();

		if(jobTitle != null && startDate != null && (terms == null || terms.Length == 0))
			return GetJobs(new Job(jobTitle, startDate, endDate, company, description, experience));
		if(jobTitle == null && startDate == null && endDate == null && company == null &&
		   description == null && experience == null && (terms == null || terms.Length == 0))
			return GetJobs();

		cmd.CommandText =
			"SELECT * FROM job WHERE company LIKE $company OR description OR $desc OR experience LIKE $exp OR title LIKE $title ";
		var filter = new List<string>();
		if(startDate != null)
			filter.Add("startDate IS $start");
		if(endDate != null)
			filter.Add("endDate IS $end");
		cmd.CommandText += string.Join(" OR ", filter);

		var placeholderValuePairs = new Dictionary<string, dynamic?>
		{
			{ "$company", (company ?? "").Surround("%") },
			{ "$desc", (description ?? "").Surround("%") },
			{ "$exp", (experience ?? "").Surround("%") },
			{ "$title", (jobTitle ?? "").Surround("%") },
			{ "$end", endDate },
			{ "$start", startDate }
		};
		cmd.BindParameters(placeholderValuePairs);

		if(terms is { Length: > 0 })
		{
			using var tempCmd = MainConnection.CreateCommand();
			tempCmd.CommandText = "CREATE TEMPORARY TABLE term(query TEXT);";
			tempCmd.ExecuteNonQuery();
			tempCmd.CommandText = "INSERT INTO term VALUES ";
			for(var i = 0; i < terms.Length; i++)
			{
				tempCmd.CommandText += $"($term{i})";
				tempCmd.Parameters.AddWithNullableValue($"$term{i}", terms[i].Surround("%"));
				if(i < terms.Length - 1)
					tempCmd.CommandText += ", ";
			}

			tempCmd.ExecuteNonQuery();
			cmd.CommandText += "UNION ALL " +
			                   "SELECT j.* FROM job as j " +
			                   "JOIN term t ON (j.title LIKE t.query) OR (j.company LIKE t.query) OR (j.description LIKE t.query) OR (j.experience LIKE t.query) ";
		}

		cmd.Prepare();
		var reader = cmd.ExecuteReader();
		var jobs = new Dictionary<long, Job>();
		while(reader.Read())
			jobs.Add((long)reader["id"], Job.ParseJobsFromQuery(reader) ?? throw new InvalidOperationException());
		return jobs;
	}
}