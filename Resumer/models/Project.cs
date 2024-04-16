using System.ComponentModel.DataAnnotations;

namespace Resumer.models;

public class Project
{
    private string _name;
    private string? _type;
    private string? _description;

    public Project()
    {
        Id = Guid.NewGuid();
        Details = new List<string>();
    }

    public Guid Id { get; init; }

    [MinLength(1, ErrorMessage = "Project name cannot be empty")]
    public required string Name
    {
        get => _name;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Project name cannot be empty");
            _name = value.Trim();
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
}