using System.Text.RegularExpressions;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using static Resumer.Utility;

namespace Resumer.cli.commands.add;

public class AddEducationCommand: AddCommand
{
    protected override string ContinuePrompt => "Add another education?";

    protected override int AddItem(CommandContext context, AddCommandSettings settings)
    {
        var school = AnsiConsole.Ask<string>("School (name):");
        var degree =
            AnsiConsole.Prompt(new SelectionPrompt<Certification>()
                .AddChoices(Enum.GetValues<Certification>())
                .EnableSearch()
                .UseConverter(certification => certification.Print())
                .WrapAround()
            );
        AnsiConsole.WriteLine($"Degree: {degree}");
        var fieldOfStudy = AnsiConsole.Ask<string>("Field (or level) of study:");
        var location = AnsiConsole.Prompt(SimplePrompt<string?>("Location:"));
        var gpa = AnsiConsole.Prompt(SimplePrompt<double?>("Grade point average:"));
        var startDate = AnsiConsole.Ask<DateOnly>("Start date:");
        var endDate = AnsiConsole.Prompt(SimplePrompt<DateOnly?>("End date or expected graduation date (optional):"));

        AnsiConsole.WriteLine("\nHighlight certain courses, classes, subjects, projects, etc.");
        AnsiConsole.MarkupLine("Press [bold]Enter[/] to skip.");
        var courses = new List<string>();
        courses.AddFromPrompt("Course:");

        var additionalInformation = AnsiConsole.Prompt(SimplePrompt<string?>("Additional information:"));

        var education = new Education()
        {
            School = school,
            Degree = degree,
            Courses = courses,

            Location = location,
            FieldOfStudy = fieldOfStudy,
            GradePointAverage = gpa,

            StartDate = startDate,
            EndDate = endDate,
            AdditionalInformation = additionalInformation,
        };

        var db = new ResumeContext();
        db.Education.Add(education);
        db.SaveChanges();
        return CommandOutput.Success();
    }
}