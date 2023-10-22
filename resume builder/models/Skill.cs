using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.Sqlite;

namespace resume_builder.models;

[Table("skill")]
public class Skill
{
	public Skill(string name, SkillType type)
	{
		SetName(name);
		Type = type;
	}

	[Column("name")]
	[Display(Name = "Skill", Description = "Name or definition of the name", Prompt = "what is your name")]
	public string Name { get; protected set; }

	[Column("type")] public SkillType Type { get; set; }

	public void SetName(string name)
	{
		if(string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Name cannot be null or empty", nameof(name));
		Name = name;
	}

	public static Skill ParseSkillsFromQuery(SqliteDataReader reader)
	{
		var name = reader.GetString(0);
		var type = reader.GetString(1);
		return new Skill(name, Enum.Parse<SkillType>(type));
	}

	public override string ToString() => $"{Type} Skill - {Name}";
}

public enum SkillType
{
	Hard,
	Soft,
	Technical,
}