using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.search;

public class SearchSkillCommand : Command<SearchSkillCommandSettings>
{
    public override int Execute(CommandContext context, SearchSkillCommandSettings settings)
    {
        ResumeContext resumeContext = new();
        var skills = resumeContext.Skills
            .Where(skill =>
                skill.Name.Contains(settings.Name) || skill.Type == settings.Type)
            .Select((skill, i) => new KeyValuePair<int, Skill>(i, skill))
            .ToList();
        if (skills.Count == 0)
            AnsiConsole.MarkupLine("No skills found");
        else
        {
            var table = settings.GetTable();
            if (table == null)
                foreach (var skill in skills)
                    AnsiConsole.WriteLine(skill.ToString());
            else
            {
                table.AddTableColumn("#","Skill Name", "Type");
                foreach (var skill in skills)
                    table.AddRow(skill.Key.ToString(), skill.Value.Name, skill.Value.Type.ToString());
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