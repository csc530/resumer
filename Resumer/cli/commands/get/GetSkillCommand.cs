using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetSkillCommand: Command<OutputCommandSettings>
{
    public override int Execute(CommandContext context, OutputCommandSettings settings)
    {
        ResumeContext database = new();
        var skills = database.Skills;

        if(!skills.Any())
            return CommandOutput.Success("No skills found");
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

            return CommandOutput.Success();
        }
    }
}