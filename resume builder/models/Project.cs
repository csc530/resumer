using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_builder.models;

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
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value;
        }
    }

    public string? Type { get; set; }
    public string? Description { get; set; }
    public Uri? Link { get; set; }
}