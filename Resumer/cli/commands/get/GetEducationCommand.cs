using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetEducationCommand: Command<GetEducationCommandSettings>
{
    public override int Execute(CommandContext context, GetEducationCommandSettings settings)
    {
        ResumeContext database = new();

        var education = database.Education.ToList();
        if(education.Count == 0)
            return CommandOutput.Success("No education found");
        else
        {
            var table = settings.CreateTable();
            if(table == null)
                education.ForEach(edu => AnsiConsole.WriteLine(edu.ToString()));
            else
            {
                education.ForEach(edu => settings.AddEducationToTable(table, edu));
                AnsiConsole.Write(table);
            }

            return CommandOutput.Success();
        }
    }
}

public class GetEducationCommandSettings: OutputCommandSettings
{
    public override Table? CreateTable()
    {
        var table = CreateTable("Education");
        if(table == null)
            return table;
        table.AddColumns("School", "Location", "Degree", "Field of Study", "Start Date", "End Date",
            "Grade Point Average", "Courses", "Additional Information");
        return table;
    }

    public void AddEducationToTable(Table table, Education education)
    {
        table.AddRow(
            education.School.Print(),
            education.Location.Print(),
            education.Degree.Print(),
            education.FieldOfStudy,
            education.StartDate.Print(),
            education.EndDate == null ? "present" : education.EndDate.Value.Print(),
            education.GradePointAverage.Print(),
            education.Courses.Print(),
            education.AdditionalInformation.Print()
        );
    }
}