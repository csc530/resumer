using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Spectre.Console;

namespace Resumer.models;

public class Project
{
    private string _title;
    private string? _type;
    private string? _description;

    public Project(string title)
    {
        Id = Guid.NewGuid();
        Title = title;
        Details = new List<string>();
    }

    public Guid Id { get; init; }

    [MinLength(1, ErrorMessage = "Project name cannot be empty")]
    public string Title
    {
        get => _title;
        [MemberNotNull(nameof(_title))]
        set
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Project name cannot be empty");
            _title = value.Trim();
        }
    }

    public string? Type
    {
        get => _type;
        set => _type = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public string? Description
    {
        get => _description;
        set => _description = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public List<string> Details { get; set; }

    public Uri? Link { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder($"{Title}");
        if(Type != null)
            sb.Append($": {Type}");

        if(StartDate.HasValue || EndDate.HasValue)
        {
            sb.Append(' ');
            if(StartDate.HasValue && EndDate.HasValue)
                sb.Append($"({StartDate} - {EndDate})");
            else if(StartDate.HasValue)
                sb.Append($"({StartDate} - Present)");
            else
                sb.Append($"({EndDate})");
        }

        return sb.ToString();
    }
}