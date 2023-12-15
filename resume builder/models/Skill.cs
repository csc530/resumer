using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.Sqlite;

namespace resume_builder.models;

public class Skill
{
    private string _name;

    public Skill(string name, SkillType type)
    {
        Name = name;
        Type = type;
    }

    [Key]
    [Display(Name = "Skill", Description = "Name or definition of the name", Prompt = "what is your name")]
    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value;
        }
    }

    public SkillType Type { get; set; }

    public override string ToString() => $"{Type} Skill - {Name}";
}

public enum SkillType
{
    Hard,
    Soft,
    Technical,
}