using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_builder.models;

[Table("skill")]
public class Skill
{
	[Column("name")]
	[Display(Name = "Skill", Description = "Name or definition of the skill", Prompt = "what is your skill")]
	public string Name { get; protected set; }

	[Column("type")] public SkillType? Type { get; set; }

	public Skill(string name, SkillType? type = null)
	{
		SetName(name);
		Type = type;
	}

	public void SetName(string name)
	{
		if(string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Name cannot be null or empty", nameof(name));
		Name = name;
	}
}

public enum SkillType
{
	Hard,
	Soft,
	Technical,
}