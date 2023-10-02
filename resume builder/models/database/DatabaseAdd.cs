namespace resume_builder.models.database;

public partial class Database

{
	public void AddSkill(Skill skill)
	{
		var cmd = MainConnection.CreateCommand();
		cmd.CommandText = "INSERT INTO skill(name, type) VALUES ($name, $type)";
		cmd.Parameters.AddWithValue("$name", skill.Name);
		cmd.Parameters.AddWithValue("$type", skill.Type.ToString());
		cmd.Prepare();
		cmd.ExecuteNonQuery();
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
}