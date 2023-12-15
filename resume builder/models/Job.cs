using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Text;
using Microsoft.Data.Sqlite;

namespace resume_builder.models;

public class Job
{
    private string? _company;
    private string? _description;
    private string? _experience;
    private string _title;

    public Guid Id { get; init; }

    [MinLength(1, ErrorMessage = "Job title cannot be empty")]
    public string Title
    {
        get => _title;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _title = Trim(value)!;
        }
    }
    
    public string? Company
    {
        get => _company;
        set => _company = Trim(value);
    }

    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public string? Description
    {
        get => _description;
        set => _description = Trim(value);
    }

    public string? Experience
    {
        get => _experience;
        set => _experience = Trim(value);
    }

    public Job()
    {
        Id = Guid.NewGuid();
    }
    public Job(string title, Guid id, DateOnly? startDate = null): this()
    {
        Title = title;
        StartDate = startDate ?? Globals.Today;
    }

    private static string? Trim(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.ReplaceLineEndings(" - ").Trim();

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"{Title} ({StartDate:yyyy-MM-dd} - ");
        if (EndDate == null)
            stringBuilder.Append("present");
        else
            stringBuilder.Append($"{EndDate:yyyy-MM-dd}");
        stringBuilder.Append(')');
        if (!string.IsNullOrWhiteSpace(Company))
            stringBuilder.Append($" @ {Company}");
        return stringBuilder.ToString();
    }
}