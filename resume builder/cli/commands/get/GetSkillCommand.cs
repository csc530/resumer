using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using resume_builder.cli.settings;
using resume_builder.models;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.get;

public class GetSkillCommand : Command<OutputCommandSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] OutputCommandSettings settings)
    {
        DbSet<Skill> skills;
        try
        {
            ResumeContext database = new();
            skills = database.Skills;
        }
        catch(Exception e)
        {
            return Globals.PrintError(settings, e);
        }

        if(skills.Count() == 0)
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