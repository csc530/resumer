namespace resume_builder;
/// <summary>
/// Modeled after and for the database jobs rows/columns
/// </summary>
public sealed class Job
{
    public Job(string title, DateOnly? startDate = null, DateOnly? endDate = null, string? company = null, string? description = null, string? experience = null)
    {
        Company = company;
        _description = description;
        _experience = experience;
        Title = title;
        StartDate = startDate ?? DateOnly.FromDateTime(DateTime.Now);
        EndDate = endDate;
    }

    private string? _company;
    private string? _description;
    private string? _experience;


    public void SetTitle(string title)
    {
        if(title == null)
            throw new ArgumentNullException(nameof(title), "title cannot be null");
        if(string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("title must contain (non-whitespace) text", nameof(title));
        Title = title.Trim();
    }

    public string Title { get; private set; }

    public string? Company
    {
        get => _company;
        set => _company = value?.Trim();
    }

    public DateOnly StartDate { get; private set; }

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

    public DateOnly? EndDate { get; private set; }

    public string? Description
    {
        get => _description;
        set => _description = value.Trim();
    }

    public string? Experience
    {
        get => _experience;
        set => _experience = value.Trim();
    }
}