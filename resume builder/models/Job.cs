using System.ComponentModel;
using resume_builder.models;

namespace resume_builder;

/// <summary>
///     Modeled after and for the database jobs rows/columns
/// </summary>
public sealed class Job
{
	private string? _company;
	private string? _description;
	private string? _experience;

	public Job(string title, DateOnly? startDate = null, DateOnly? endDate = null, string? company = null,
	           string? description = null, string? experience = null)
	{
		Company = company;
		_description = description;
		_experience = experience;
		Title = title;
		StartDate = startDate ?? DateOnly.FromDateTime(DateTime.Now);
		EndDate = endDate;
	}

	[SqlColumnName("title")] public string Title { get; private set; }

	[SqlColumnName("company")]
	public string? Company
	{
		get => _company;
		set => _company = value?.Trim();
	}

	[SqlColumnName("start date")] public DateOnly StartDate { get; private set; }
	[SqlColumnName("end date")] public DateOnly? EndDate { get; private set; }

	[SqlColumnName("job description")]
	public string? Description
	{
		get => _description;
		set => _description = value?.Trim();
	}

	[SqlColumnName("experience")]
	public string? Experience
	{
		get => _experience;
		set => _experience = value?.Trim();
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
		Title = title.Trim();
	}

	public void SetStartDate(DateOnly date)
	{
		if(date == null)
			throw new ArgumentNullException(nameof(date), "start date cannot be null");
		if(EndDate != null && EndDate > StartDate)
			throw new ArgumentException(nameof(date),
				$"start date ({StartDate}) must be before, or the same day as, the end date ({EndDate})");
		StartDate = date;
	}

	public void SetEndDate(DateOnly? date)
	{
		if(date == null)
			throw new ArgumentNullException(nameof(date), "start date cannot be null");
		if(EndDate < StartDate)
			throw new ArgumentException(nameof(date),
				$"end date ({EndDate}) must be after, or the same day as, the start date ({StartDate})");
		EndDate = date;
	}
}