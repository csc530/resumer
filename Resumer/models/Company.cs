using System.ComponentModel.DataAnnotations;

namespace Resumer.models;

public class Company
{
    [Key]
    public string Name { get; set; } = string.Empty;
}