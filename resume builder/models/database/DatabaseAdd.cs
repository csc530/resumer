using Microsoft.Data.Sqlite;
using resume_builder.models.database.query;

namespace resume_builder.models.database;

public partial class Database

{
	public int AddSkill(Skill skill)
	{
		return MainConnection.CreateAddCommand(skill).ExecuteNonQuery();
	}

	public int AddProfile(Profile profile)
	{
		return MainConnection.CreateAddCommand(profile).ExecuteNonQuery();
	}

	private int AddCompany(string company)
	{
		if(string.IsNullOrWhiteSpace(company))
			throw new ArgumentException("company is null or empty", nameof(company));
		return MainConnection.CreateAddCommand(company).ExecuteNonQuery();
	}

	public int AddJob(Job job)
	{
		if(!string.IsNullOrWhiteSpace(job.Company) && GetCompany(job.Company) == null)
			AddCompany(job.Company);
		return MainConnection.CreateAddCommand(job).ExecuteNonQuery();
	}
}