using System.ComponentModel.DataAnnotations;

namespace Resumer.models;

public class Project
{
    private string _name;

    public Project(string name)
    {
        Name = name;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; init; }

    [MinLength(1, ErrorMessage = "Project name cannot be empty")]
    public string Name
    {
        get => _name;
        set
        {
            if(value.IsBlank())
                throw new ArgumentException("Project name cannot be empty");
            _name = value;
        }
    }

    public string? Type { get; set; }
    public string? Description { get; set; }
    public Uri? Link { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}