using System.ComponentModel.DataAnnotations;

namespace resume_builder.models;

public class Company
{
    [Key]
    public string Name { get; set; } = string.Empty;
}