using Microsoft.EntityFrameworkCore;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetSkillCommand: Command<GetSkillCommandSettings>
{
    public override int Execute(CommandContext context, GetSkillCommandSettings settings)
    {
        ResumeContext database = new();
        var skills = database.Skills;

        if(!skills.Any())
            return CommandOutput.Success("No skills found");
        else
        {
            var table = settings.CreateTable<Skill>("Skills")?.AddObjects(database.Skills);
            if(table == null)
                skills.ForEachAsync(skill => AnsiConsole.WriteLine(skill.ToString())).Wait();
            else
                AnsiConsole.Write(table);

            return CommandOutput.Success();
        }
    }
}

public class GetSkillCommandSettings : OutputCommandSettings;