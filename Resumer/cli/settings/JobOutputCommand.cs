using System.ComponentModel;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.settings;

public abstract class JobOutputCommand<T>: Command<T> where T : JobOutputSettings
{
    protected static void PrintJobsPlain(T settings, Dictionary<int, Job> rows)
    {
        InitializeSettings(settings);
        foreach(var (tblId, job) in rows)
        {
            var row = new List<string>();
            if(settings.Id)
                row.Add((tblId.ToString()));
            if(settings.JobTitle)
                row.Add((job.Title));
            if(settings.Company)
                row.Add((job.Company.GetPrintValue()));
            if(settings.StartDate)
                row.Add((job.StartDate.ToString()));
            if(settings.EndDate)
                row.Add((job.EndDate.GetPrintValue()));
            if(settings.Description)
                row.Add((job.Description.GetPrintValue()));
            if(settings.Experience)
                row.Add(job.Experience.GetPrintValue());
            var expandColumns = (settings.Expand && !settings.Minimize) || (!settings.Minimize && !settings.Expand);
            AnsiConsole.Write(new Columns(row) { Expand = expandColumns });
        }
    }

    protected static void PrintJobsTable(T settings, Table table, Dictionary<int, Job> rows)
    {
        InitializeSettings(settings);
        if(settings.Id)
            table.AddTableColumn("ID");
        if(settings.JobTitle)
            table.AddTableColumn("Title");
        if(settings.Company)
            table.AddTableColumn("Company");
        if(settings.StartDate)
            table.AddTableColumn("Start Date");
        if(settings.EndDate)
            table.AddTableColumn("End Date");
        if(settings.Description)
            table.AddTableColumn("Description");
        if(settings.Experience)
            table.AddTableColumn("Experience");

        foreach(var (id, job) in rows)
        {
            var columnConditionValuePairs = new List<KeyValuePair<bool, object?>>
            {
                new(settings.Id, id),
                new(settings.JobTitle, job.Title),
                new(settings.Company, job.Company),
                new(settings.StartDate, job.StartDate),
                new(settings.EndDate, job.EndDate),
                new(settings.Description, job.Description),
                new(settings.Experience, job.Experience),
            };
            table.AddTableRow(columnConditionValuePairs.ToArray());
        }

        Spectre.Console.AnsiConsole.Write(table);
    }

    /// <summary>
    /// Sets all setting properties to reflect their value of <see cref="JobOutputSettings.ShowAll"/>
    ///
    /// **Changes** setting properties to reflect it's <see cref="JobOutputSettings.ShowAll"/> property
    /// either setting all properties to true or leaving them unchanged if false
    /// </summary>
    ///
    ///<remarks>
    /// Used to accurately print columns and values in <see cref="PrintJobsPlain"/> and <see cref="PrintJobsTable"/>
    /// </remarks>
    /// <param name="settings">the given <see cref="JobOutputSettings"/> </param>
    private static void InitializeSettings(T settings)
    {
        if(!settings.ShowAll)
            return;
        settings.Id = true;
        settings.JobTitle = true;
        settings.Company = true;
        settings.StartDate = true;
        settings.EndDate = true;
        settings.Description = true;
        settings.Experience = true;
    }
}

public class JobOutputSettings: OutputCommandSettings
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