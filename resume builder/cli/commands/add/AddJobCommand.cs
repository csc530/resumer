using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.add;

public class AddSetting : CommandSettings
{
}

public class AddJobSettings : AddSetting
{
    [Description("start date at the job")]
    [CommandOption("-s|--start <StartDate>")]
    public DateOnly? StartDate { get; init; }

    [Description("last date at the job")]
    [CommandOption("-e|--end")]
    public DateOnly? EndDate { get; init; }

    [Description("job title")]
    [CommandOption("-t|--title")]
    public string? JobTitle { get; init; }

    [Description("posted/official job description by the employer")]
    [CommandOption("-d|--description")]
    public string? JobDescription { get; init; }

    [Description("your (personal) experience at the job")]
    [CommandOption("-x|--experience")]
    public string? Experience { get; init; }

    [Description("The company or employer name")]
    [CommandOption("-c|--company")]
    public string? Company { get; }

    public override ValidationResult Validate()
    {
        if(string.IsNullOrWhiteSpace(JobTitle))
            return ValidationResult.Error("Job title is required");
        if(StartDate == null || EndDate > StartDate)
            return ValidationResult.Error("Start date is required");
        return ValidationResult.Success();
    }

    public void Deconstruct(out DateOnly? startDate, out DateOnly? endDate, out string? jobTitle,
        out string? jobDescription, out string? experience, out string? company)
    {
        startDate = StartDate;
        endDate = EndDate;
        jobTitle = JobTitle;
        jobDescription = JobDescription;
        experience = Experience;
        company = Company;
    }

    public void Deconstruct(out string? title, out DateOnly? start)
    {
        start = StartDate;
        title = JobTitle;
    }
}

internal sealed class AddJobCommand : Command<AddJobSettings>
{

    public override int Execute([NotNull] CommandContext context, [NotNull] AddJobSettings settings)
    {
    Database database = new();
        var (startDate, endDate, jobTitle, jobDescription, experience, company) = settings;
        if(string.IsNullOrWhiteSpace(jobTitle))
            return ExitCode.InvalidArgument.ToInt();
        if(startDate == null)
            return ExitCode.InvalidArgument.ToInt();

        var job = new Job(jobTitle, startDate, endDate, company, jobDescription, experience);
        database.AddJob(job);
        AnsiConsole.WriteLine($"{context}\ntitle: {jobTitle}\n start: {startDate}\n end: {endDate}");
        //todo add nice output and what went in
        return (ExitCode.Success).ToInt();
    }
}