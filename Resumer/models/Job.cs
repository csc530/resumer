using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Resumer.models;

[PrimaryKey(nameof(Title), nameof(Company), nameof(StartDate))]
public class Job
{
    private string _company;
    private string? _description;
    private string? _experience;
    private string _title;

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

    public DateOnly StartDate { get; set; } = Globals.Today;
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