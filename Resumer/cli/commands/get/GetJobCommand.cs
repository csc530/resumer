using System.ComponentModel;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetJobCommand : Command<GetJobCommandSettings>
{
    /// <inheritdoc />
    public override int Execute(CommandContext context, GetJobCommandSettings settings)
    {
        ResumeContext database = new();

        if(!database.Jobs.Any())
            return CommandOutput.Success("No jobs found");
        else
        {
            var table = Job.CreateTable(database.Jobs.ToList());

            return CommandOutput.Success(table);
        }
    }
}

public class GetJobCommandSettings : OutputCommandSettings
{
    public bool ShowAll => (Id && JobTitle && Company && StartDate && EndDate && Description && Experience) ||
                           (!Id && !JobTitle && !Company && !StartDate && !EndDate && !Description && !Experience);


    [CommandOption("-i|--id")]
    [Description("display jobs' database id")]
    public bool Id { get; set; }

    [CommandOption("-t|--title")]
    [Description("display jobs' title")]
    public bool JobTitle { get; set; }

    [CommandOption("-s|--start")]
    [Description("display jobs's start date")]
    public bool StartDate { get; set; }

    [CommandOption("-e|--end")]
    [Description("display jobs's last date")]
    public bool EndDate { get; set; }

    [CommandOption("-c|--company")]
    [Description("display jobs'  company/employer name")]
    public bool Company { get; set; }

    [CommandOption("-x|--experience")]
    [Description("display jobs' experience")]
    public bool Experience { get; set; }

    [CommandOption("-d|--description")]
    [Description("display jobs' description")]
    public bool Description { get; set; }
}