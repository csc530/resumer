using System.ComponentModel;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.add;

public class AddSkillCommand: Command<AddSkillSettings>
{
    public override int Execute(CommandContext context, AddSkillSettings settings)
    {
        ResumeContext database = new();
        var skillName = settings.Skill;
        var skillType = settings.SkillType;

        if(skillName == null || skillType == null)
        {
            skillName ??= AnsiConsole.Prompt(new TextPrompt<string>("Skill:")
                .Validate(input => database.Skills.Select(skill => skill.Name.ToLower()).Contains(input.ToLower())
                    ? ValidationResult.Error("skill name has already been added")
                    : ValidationResult.Success()));
            skillType ??= AnsiConsole.Prompt(new SelectionPrompt<SkillType>()
                .Title("Skill Type")
                .AddChoices(Enum.GetValues<SkillType>())
                .WrapAround());
        }

        var skill = new Skill(skillName, skillType.Value);
        database.Skills.Add(skill);
        database.SaveChanges();
        return CommandOutput.Success($"""✅ [bold]{skill.Type}[/] Skill "[bold]{skill.Name}[/]" added""");
    }
}

public class AddSkillSettings: CommandSettings
{
    [CommandArgument(0, "[skill]")]
    [Description("The name, abbreviation, or short description of the skill")]
    public string? Skill { get; set; }

    //todo: allows for shortcuts: so you don't have to write out the whole type's name
    //ex. s => soft
    [CommandArgument(1, "[type]")]
    [Description("The type of skill: soft or hard")]
    public SkillType? SkillType { get; set; }
}