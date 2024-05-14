using System.ComponentModel;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.add;

public class AddSkillCommand : Command<AddSkillSettings>
{
    public override int Execute(CommandContext context, AddSkillSettings settings)
    {
        var skillName = AnsiConsole.Ask<string>("Skill: ");
        var skillType = AnsiConsole.Prompt(new SelectionPrompt<SkillType>()
            .Title("Skill Type")
            .AddChoices(Enum.GetValues<SkillType>())
            .MoreChoicesText("[grey](Move up and down to reveal more skill types)[/]")
            .WrapAround());

        var skill = new Skill(skillName, skillType);
        ResumeContext database = new();
        database.Skills.Add(skill);
        database.SaveChanges();
        return CommandOutput.Success($"""✅ [bold]{skill.Type}[/] Skill "[bold]{skill.Name}[/]" added""");
    }
}

public class AddSkillSettings : CommandSettings
{
    [CommandArgument(0, "[skill]")]
    [Description("The name, abbreviation, or short description of the skill")]
    public string? Skill { get; set; }

    //todo: allows for shortcuts: so you don't have to write out the whole type's name
    //ex. s => soft
    [CommandArgument(1, "[type]")]
    [Description("The type of skill: soft, hard, etc.")]
    public SkillType? SkillType { get; set; }
}