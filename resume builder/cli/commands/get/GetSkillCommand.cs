using System.Diagnostics.CodeAnalysis;
using resume_builder.cli.settings;
using resume_builder.models;
using resume_builder.models.database;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.get;

public class GetSkillCommand : Command<OutputCommandSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] OutputCommandSettings settings)
	{
		List<Skill> skills;
		try
		{
			Database database = new();
			skills = database.GetSkills();
		}
		catch(Exception e)
		{
			return Globals.PrintError(settings, e);
		}

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