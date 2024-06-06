using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Resumer.models;

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
    [Required(AllowEmptyStrings = false)]
    public string Name
    {
        get => _name;
        [MemberNotNull(nameof(_name))]
        set
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Skill name cannot be empty");
            _name = value;
        }
    }

    public SkillType Type { get; set; }

    public override string ToString() => $"{Name} - {Type} Skill";
}

public enum SkillType
{
    Hard,
    Soft,
    // Technical,
}