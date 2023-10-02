namespace resume_builder.models.database;

public partial class Database
{
	public List<Skill> GetSkillsLike(string? name, SkillType? type)
	{
		if(name == null && type == null)
			return GetSkills();
		using var cmd = MainConnection.CreateCommand();
		if(name != null && type != null)
		{
			cmd.CommandText = "SELECT * FROM skill WHERE name LIKE $name AND type IS $type";
			cmd.Parameters.AddWithNullableValue("$name", name.Surround("%"));
			cmd.Parameters.AddWithNullableValue("$type", type.ToString());
		}
		else if(name != null)
		{
			cmd.CommandText = "SELECT * FROM skill WHERE name LIKE $name";
			cmd.Parameters.AddWithNullableValue("$name", name.Surround("%"));
		}
		else
		{
			cmd.CommandText = "SELECT * FROM skill WHERE type IS $type";
			cmd.Parameters.AddWithNullableValue("$type", type.ToString());
		}

		cmd.Prepare();
		using var reader = cmd.ExecuteReader();
		var skills = new List<Skill>();
		while(reader.Read())
			skills.Add(Skill.ParseSkillsFromQuery(reader));
		return skills;
	}

	public string? GetCompany(string company)
	{
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "SELECT name FROM company WHERE name IS @company";
		cmd.Parameters.AddWithValue("@company", company);
		cmd.Prepare();
		return (string?)cmd.ExecuteScalar();
	}

	public List<Skill> GetSkills()
	{
		using var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "SELECT * FROM skill";

		using var reader = cmd.ExecuteReader();
		var skills = new List<Skill>();
		while(reader.Read())
			skills.Add(Skill.ParseSkillsFromQuery(reader));
		return skills;
	}

	public List<string> GetCompanies()
	{
		using var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "SELECT name FROM company";

		using var reader = cmd.ExecuteReader();
		var companies = new List<string>();
		while(reader.Read())
			companies.Add(reader.GetString(0));
		return companies;
	}

	public List<string> GetCompaniesLike(string name)
	{
		using var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "SELECT name FROM company WHERE name LIKE $name";
		cmd.Parameters.AddWithNullableValue("$name", name.Surround("%"));
		cmd.Prepare();
		using var reader = cmd.ExecuteReader();
		var companies = new List<string>();
		while(reader.Read())
			companies.Add(reader.GetString(0));
		return companies;
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


	public Dictionary<long, Job> GetJobsLike(string? jobTitle = null, DateOnly? startDate = null,
	                                         DateOnly? endDate = null, string? company = null,
	                                         string? description = null, string? experience = null)
	{
		if(jobTitle == null && startDate == null && endDate == null && company == null && description == null &&
		   experience == null)
			return GetJobs();
		using var cmd = GetJobsLikeQuery(jobTitle, startDate, endDate, company, description, experience);
		cmd.Prepare();
		var reader = cmd.ExecuteReader();
		var jobs = new Dictionary<long, Job>();
		while(reader.Read())
		{
			var id = (long)reader["id"];
			var job = Job.ParseJobsFromQuery(reader) ?? throw new InvalidOperationException();
			jobs.Add(id, job);
		}

		return jobs;
	}

	/// <summary>
	/// search for jobs that match the given terms in any of the following fields: title, company, description, experience, startDate, or endDate
	/// </summary>
	/// <param name="terms">search terms</param>
	/// <returns>key value pairs of the job ids and matched jobs</returns>
	/// <exception cref="InvalidOperationException"></exception>
	public Dictionary<long, Job> GetJobsLike(params string[] terms)
	{
		using var cmd = MainConnection.CreateCommand();
		CreateTemporaryTermsTable(terms);

		cmd.ExecuteNonQuery();
		cmd.CommandText = "SELECT j.* FROM job as j JOIN term t ON " +
		                  "(j.title LIKE t.query) OR (j.company LIKE t.query) OR (j.description LIKE t.query) OR (j.experience LIKE t.query)" +
		                  "OR (j.startDate LIKE t.query) OR (j.endDate LIKE t.query);";


		var reader = cmd.ExecuteReader();
		var jobs = new Dictionary<long, Job>();
		while(reader.Read())
		{
			var id = (long)reader["id"];
			var job = Job.ParseJobsFromQuery(reader);
			jobs.Add(id, job);
		}

		return jobs;
	}

	public Dictionary<long, Job> GetJobsLike(Job? job) => job == null
		? GetJobs()
		: GetJobsLike(job.Title, job.StartDate, job.EndDate, job.Company, job.Description, job.Experience);


	public Dictionary<long, Job> GetJobs(long[] ids)
	{
		if(ids.Length == 0)
			return GetJobs();
		using var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "SELECT job.* FROM job WHERE id IN ";

		var values = string.Join(",", ids.Select((_, i) => $"($id{i})"));
		cmd.CommandText += $"({values})";
		for(var i = 0; i < ids.Length; i++)
			cmd.Parameters.AddWithValue($"$id{i}", ids[i]);

		cmd.Prepare();
		using var data = cmd.ExecuteReader();
		var jobs = new Dictionary<long, Job>();
		while(data.Read())
			jobs.Add((long)data["id"], Job.ParseJobsFromQuery(data) ?? throw new InvalidOperationException());
		return jobs;
	}

	public Dictionary<long, Job> GetJobs()
	{
		using var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "SELECT * FROM job";
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
}