using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Spectre.Console;

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

    /// <summary>
    /// creates a spectre console renderable table to display Job objects
    /// </summary>
    /// <returns> a table to display Job objects</returns>
    public static Table CreateTable()
    {
        var table = new Table()
        {
            Title = new TableTitle("Jobs"),
            Border = TableBorder.Rounded,
            Expand = true,
            ShowRowSeparators = true,
            ShowHeaders = true,
            UseSafeBorder = true,
        };

        table.AddColumn("Title");
        table.AddColumn("Company");
        table.AddColumn("Start Date");
        table.AddColumn("End Date");
        table.AddColumn("Description");
        table.AddColumn("Experience");
        return table;
    }

    /// <inheritdoc cref="CreateTable()" />
    /// <param name="jobs">jobs to populate within the table</param>
    /// <returns>Spectre console table filled with the given jobs</returns>
    public static Table CreateTable(List<Job> jobs)
    {
        var table = CreateTable();

        foreach(var job in jobs)
            table.AddRow(new Text(job.Title),
                new Text(job.Company),
                new Text(job.Description.Print()),
                new Text(job.Experience.Print()),
                new Text(job.StartDate.ToString()),
                new Text(job.EndDate?.ToString() ?? "present")
            );
        return table;
    }

    /// <inheritdoc cref="CreateTable()" />
    /// <param name="job">job to populate within the table</param>
    /// <returns>Spectre console table filled with the given job</returns>
    public static Table CreateTable(Job job)
    {
        var table = CreateTable();
        var columns = new Columns(new Text(job.Title),
            new Text(job.Company),
            new Text(job.Description.Print()),
            new Text(job.Experience.Print()),
            new Text(job.StartDate.ToString()),
            new Text(job.EndDate?.ToString() ?? "present")
        );
        table.AddRow(columns);
        return table;
    }

}