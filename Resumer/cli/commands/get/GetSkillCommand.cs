using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetSkillCommand: Command<GetSkillCommandSettings>
{
    public override int Execute(CommandContext context, GetSkillCommandSettings settings)
    {
        ResumeContext database = new();
        var skills = database.Skills.ToList();

        if(skills.Count == 0)
            return CommandOutput.Success("No skills found");
        else
        {
            var table = settings.CreateTable();
            if(table == null)
                skills.ForEach(skill => AnsiConsole.WriteLine(skill.ToString()));
            else
            {
                skills.ForEach(skill => settings.AddSkillToTable(table, skill));
                AnsiConsole.Write(table);
            }

            return CommandOutput.Success();
        }
    }
}

public class GetSkillCommandSettings: OutputCommandSettings
{
    public Table? CreateTable()
    {
        var table = CreateTable("Skills");
        if(table == null) return table;

        table.AddColumns("Name", "Type");

        return table;
    }

    public void AddSkillToTable(Table table, Skill skill) => table.AddRow(skill.Name, skill.Type.Print());
}