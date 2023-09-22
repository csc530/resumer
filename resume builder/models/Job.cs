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

	private static string? Trim(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

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
		if(EndDate < StartDate)
			throw new ArgumentException(nameof(date),
				$"end date ({EndDate}) must be after, or the same day as, the start date ({StartDate})");
		EndDate = date;
	}

	public static Job FromDictionary(Dictionary<string, dynamic> propertyValuePairs)
	{
		string title = string.Empty;
		string? description = null, company = null, experience = null;
		DateOnly start = default;
		DateOnly? end = null;
		foreach(var (property, value) in propertyValuePairs)
		{
			switch(property)
			{
				case nameof(Title):
					title = value;
					break;
				case nameof(Description):
					description = value;
					break;
				case nameof(Company):
					company = value;
					break;
				case nameof(Experience):
					experience = value;
					break;
				case nameof(StartDate):
					start = value is DateOnly ? value : DateOnly.Parse(value);
					break;
				case nameof(EndDate):
					end = value.GetType() == typeof(DateOnly?) ? value : DateOnly.Parse(value);
					break;
			}
		}

		return new Job(title, start, end, company, description, experience);
	}
}