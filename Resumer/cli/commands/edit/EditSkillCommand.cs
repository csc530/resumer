using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Command = Spectre.Console.Cli.Command;

namespace Resumer.cli.commands.edit;

public class EditSkillCommand: Command
{
    public override int Execute(CommandContext context)
    {
        var db = new ResumeContext();

        var skill = AnsiConsole.Prompt(new SelectionPrompt<Skill>()
            .Title("Select skill to edit")
            .AddChoices(db.Skills.OrderBy(s => s.Name))
            .WrapAround()
            .PageSize(10)
        );
        db.Skills.Update(skill);
        AnsiConsole.WriteLine($"Editing: {skill}");

        var name = AnsiConsole.Prompt(new TextPrompt<string>("Name:").DefaultValue(skill.Name));
        var type = AnsiConsole.Prompt(new TextPrompt<SkillType>("Type (hard/soft):").DefaultValue(skill.Type));


        // because skill.name is the PK and it's not a [foreign key assoc.](https://stackoverflow.com/questions/5281974/code-first-independent-associations-vs-foreign-key-associations/5282275#5282275)
        // tl;dr remove and insert to update 🙄
        if(!skill.Name.Equals(name, StringComparison.CurrentCulture))
        {
            db.Skills.Remove(skill);
            db.Skills.Add(new Skill(name, type));
        }
        else
            skill.Type = type;

        db.SaveChanges();

        return CommandOutput.Success("Skill updated successfully");
    }
}