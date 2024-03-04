using System.ComponentModel.DataAnnotations;

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
    public string Name
    {
        get => _name;
        set
        {
            if(value.IsBlank())
                throw new ArgumentException("Skill name cannot be empty");
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