using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Resumer.models;

public class Job
{
    private string _company;
    private string _title;

    public Job(string title, string company)
    {
        Title = title;
        Company = company;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; private set; }

    [MinLength(1, ErrorMessage = "Job title cannot be empty")]
    public string Title
    {
        get => _title;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Job title cannot be empty");
            _title = Trim(value.ReplaceLineEndings(" - "));
        }
    }

    public string Company
    {
        get => _company;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Company name cannot be empty");
            _company = Trim(value);
        }
    }

    public DateOnly StartDate { get; set; } = Utility.Today;
    public DateOnly? EndDate { get; set; }

    public List<string> Description { get; set; } = [];

    public List<string> Experience { get; set; } = [];

    [return: NotNullIfNotNull(nameof(value))]
    private static string? Trim(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append($"{Title} ({StartDate:yyyy-MM-dd} - ");
        if(EndDate == null)
            stringBuilder.Append("present");
        else
            stringBuilder.Append($"{EndDate:yyyy-MM-dd}");

        stringBuilder.Append(')');

        if(!string.IsNullOrWhiteSpace(Company))
            stringBuilder.Append($" @ {Company}");
        return stringBuilder.ToString();
    }

}