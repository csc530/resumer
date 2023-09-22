using Microsoft.Data.Sqlite;
using resume_builder.models;
using System.Data.Common;
using System.Reflection;

namespace resume_builder;

/// <summary>
///     Modeled after and for the database jobs rows/columns
/// </summary>
[SqlTableName("jobs")]
public sealed class Job
{
	private string? _company;
	private string? _description;
	private string? _experience;

	public Job(string title, DateOnly? startDate = null, DateOnly? endDate = null, string? company = null,
	           string? description = null, string? experience = null)
	{
		Company = company;
		Description = description;
		Experience = experience;
		SetTitle(title);
		SetStartDate(startDate ?? DateOnly.FromDateTime(DateTime.Now));
		SetEndDate(endDate);
	}

	[SqlColumnName("title")] public string Title { get; private set; }

	[SqlColumnName("company")]
	public string? Company
	{
		get => _company;
		set => _company = Trim(value);
	}

	private static string? Trim(string? value) =>
		string.IsNullOrWhiteSpace(value) ? null : value.ReplaceLineEndings(" - ").Trim();

	[SqlColumnName("start date")] public DateOnly StartDate { get; private set; }
	[SqlColumnName("end date")] public DateOnly? EndDate { get; private set; }

	[SqlColumnName("description")]
	public string? Description
	{
		get => _description;
		set => _description = Trim(value);
	}

	[SqlColumnName("experience")]
	public string? Experience
	{
		get => _experience;
		set => _experience = Trim(value);
	}

	public void Deconstruct(out string? company, out string? description, out string? experience, out string title,
	                        out DateOnly startDate, out DateOnly? endDate)
	{
		company = _company;
		description = _description;
		experience = _experience;
		title = Title;
		startDate = StartDate;
		endDate = EndDate;
	}


	public void SetTitle(string title)
	{
		if(title == null)
			throw new ArgumentNullException(nameof(title), "title cannot be null");
		if(string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("title must contain (non-whitespace) text", nameof(title));
		Title = Trim(title)!;
	}

	public void SetStartDate(DateOnly date)
	{
		if(EndDate != null && EndDate > StartDate)
			throw new ArgumentException(
				$"start date ({StartDate}) must be before, or the same day as, the end date ({EndDate})", nameof(date));
		StartDate = date;
	}

	public void SetEndDate(DateOnly? date)
	{
		if(EndDate < StartDate)
			throw new ArgumentException(nameof(date),
				$"end date ({EndDate}) must be after, or the same day as, the start date ({StartDate})");
		EndDate = date;
	}

	/// <summary>
	/// given a db reader parses the current row into a job
	///
	/// it does not call <see cref="DbDataReader.Read"/>
	/// </summary>
	/// <param name="reader">the db reader with the current row</param>
	/// <returns>a new <see cref="Job"/> with the current row's data, null if there are no rows in the reader</returns>
	/// <exception cref="ArgumentNullException">if <paramref name="reader"/> is null</exception>
	public static Job? ParseJobsFromQuery(SqliteDataReader reader)
	{
		if(reader == null)
			throw new ArgumentNullException(nameof(reader));
		if(!reader.HasRows)
			return null;

		var title = reader.GetNullableValue<string>("title");
		var company = reader.GetNullableValue<string>("company");
		var description = reader.GetNullableValue<string>("description");
		var experience = reader.GetNullableValue<string>("experience");
		var startDate = reader.GetNullableValue<DateOnly>("start date");
		var endDate = reader.GetNullableValue<DateOnly?>("end date");

		return new Job(title, startDate, endDate, company, description, experience);
	}
}