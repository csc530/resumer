using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetJobCommand: Command<GetJobCommandSettings>
{
    /// <inheritdoc />
    public override int Execute(CommandContext context, GetJobCommandSettings settings)
    {
        ResumeContext database = new();

        var jobs = database.Jobs.ToList();
        if(jobs.Count == 0)
            return CommandOutput.Success("No jobs found");
        else
        {
            var table = settings.CreateTable();
            if(table == null)
                jobs.ForEach(job => AnsiConsole.WriteLine(job.ToString()));
            else
            {
                jobs.ForEach(job => GetJobCommandSettings.AddJobToTable(table, job));
                AnsiConsole.Write(table);
            }

            return CommandOutput.Success();
        }
    }
}

public class GetJobCommandSettings: OutputCommandSettings
{
    public override Table? CreateTable()
    {
        var table = CreateTable("Jobs");
        if(table == null) return table;

        table.AddColumn("Title");
        table.AddColumn("Company");
        table.AddColumn("Start Date");
        table.AddColumn("End Date");
        table.AddColumn("Description");
        return table;
    }

    /// <summary>
    /// adds a <see cref="Job"/> to <see cref="Table"/>
    /// </summary>
    /// <param name="table">table created by <see cref="CreateTable"/></param>
    /// <returns>table with the newly added job</returns>
    public static void AddJobToTable(Table table, Job job) =>
        table.AddRow(
            job.Title,
            job.Company,
            job.StartDate.ToString(),
            job.EndDate?.ToString() ?? "present",
            job.Description.Print()
        );
}