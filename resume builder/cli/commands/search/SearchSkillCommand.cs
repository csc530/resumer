using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using resume_builder.cli.settings;
using resume_builder.models;
using resume_builder.models.database;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.search;

public class SearchSkillCommand : Command<SearchSkillCommandSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] SearchSkillCommandSettings settings)
	{
		Database database = new();
		var skills = database.GetSkillsLike(settings.Name, settings.Type);
		if(skills.Count == 0)
			AnsiConsole.MarkupLine("No skills found");
		else
		{
			var table = settings.GetTable();
			if(table == null)
				foreach(var skill in skills)
					AnsiConsole.WriteLine(skill.ToString());
			else
			{
				table.AddTableColumn("Skill Name", "Type");
				foreach(var skill in skills)
					table.AddRow(skill.Name, skill.Type.ToString());
				AnsiConsole.Write(table);
			}
		}

		return ExitCode.Success.ToInt();
	}
}

public class SearchSkillCommandSettings : OutputCommandSettings
{
	[CommandArgument(0, "[name]")]
	[Description("The name of the skill")]
	[DisplayName("Skill Name")]
	public string? Name { get; set; }

	[CommandOption("-t|--type")]
	[Description("The type of skill: soft, hard, etc.")]
	public SkillType? Type { get; set; }
}