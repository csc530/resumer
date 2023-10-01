using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.Sqlite;
using resume_builder.models;
using resume_builder.models.database;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.get.job;

public class GetJobCommand : Command<GetJobCommandSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] GetJobCommandSettings settings)
	{
		Database database = new();
		var plain = settings.Plain;
		var ids = settings.Ids;
		Dictionary<long, Job> rows;
		try
		{
			rows = database.GetJob(ids ?? Array.Empty<long>());
		}
		catch(Exception e)
		{
			return Globals.PrintError(settings, e);
		}

		var jobs = rows.Values;
		if(jobs.Count == 0)
		{
			AnsiConsole.MarkupLine("No jobs found");
			return ExitCode.Success.ToInt();
		}

		var table = settings.GetTable();
		Console.Error.WriteLine("plain: " + plain);
		if((plain && !settings.Table) || table == null)
			PlainOutput(settings, rows);
		else
			TableOutput(table, settings, rows);
		return ExitCode.Success.ToInt();
	}

	private static string GetStringValue(string? value) =>
		string.IsNullOrWhiteSpace(value) ? Globals.NullString : value;

	private static string GetStringValue(object? value) =>
		value == null ? Globals.NullString : GetStringValue(value.ToString());

	private void PlainOutput(GetJobCommandSettings settings, Dictionary<long, Job> rows)
	{
		var id = settings.Id;
		var experience = settings.Experience;
		var title = settings.Title;
		var description = settings.Description;
		var company = settings.Company;
		var startDate = settings.StartDate;
		var endDate = settings.EndDate;
		bool allNull = !id && !title && !company && !startDate && !endDate && !description && !experience;
		foreach(var (tblId, job) in rows)
		{
			var row = new List<string>();
			if(allNull)
			{
				row.Add(tblId.ToString());
				row.Add(job.Title);
				row.Add(GetStringValue(job.Company));
				row.Add(job.StartDate.ToString());
				row.Add(GetStringValue(job.EndDate.ToString()));
				row.Add(GetStringValue(job.Description));
				row.Add(GetStringValue(job.Experience));
			}
			else
			{
				if(id)
					row.Add((tblId.ToString()));
				if(title)
					row.Add((job.Title));
				if(company)
					row.Add((GetStringValue(job.Company)));
				if(startDate)
					row.Add((job.StartDate.ToString()));
				if(endDate)
					row.Add((GetStringValue(job.EndDate)));
				if(description)
					row.Add((GetStringValue(job.Description)));
				if(experience)
					row.Add(GetStringValue(job.Experience));
			}

			AnsiConsole.Write(new Columns(row) { Expand = false });
		}
		//AnsiConsole.WriteLine(tblId);
	}

	private void TableOutput(Table table, GetJobCommandSettings settings, Dictionary<long, Job> rows)
	{
		var id = settings.Id;
		var title = settings.Title;
		var company = settings.Company;
		var startDate = settings.StartDate;
		var endDate = settings.EndDate;
		var description = settings.Description;
		var experience = settings.Experience;

		bool allNull = !id && !title && !company && !startDate && !endDate && !description && !experience;
		if(allNull)
		{
			table.AddColumn(new TableColumn("ID"))
			     .AddColumn(new TableColumn("Job Title"))
			     .AddColumn(new TableColumn("Company"))
			     .AddColumn(new("Start Date"))
			     .AddColumn(new("End Date"))
			     .AddColumn(new("Description"))
			     .AddColumn(new("Experience"));
		}
		else
		{
			if(id)
				table.AddColumn(new TableColumn("ID"));
			if(title)
				table.AddColumn(new TableColumn("Job Title"));
			if(company)
				table.AddColumn(new TableColumn("Company"));
			if(startDate)
				table.AddColumn(new("Start Date"));
			if(endDate)
				table.AddColumn(new("End Date"));
			if(description)
				table.AddColumn(new("Description"));
			if(experience)
				table.AddColumn(new("Experience"));
		}

		foreach(var (tblId, job) in rows)
		{
			var row = new List<string>();
			if(allNull)
			{
				row.Add(tblId.ToString());
				row.Add(job.Title);
				row.Add(job.Company ?? "");
				row.Add(job.StartDate.ToString());
				row.Add(job.EndDate.ToString() ?? "");
				row.Add(job.Description ?? "");
				row.Add(job.Experience ?? "");
				table.AddRow(row.ToArray());
				continue;
			}

			if(id)
				row.Add(tblId.ToString());
			if(title)
				row.Add(job.Title);
			if(company)
				row.Add(job.Company ?? "");
			if(startDate)
				row.Add(job.StartDate.ToString());
			if(endDate)
				row.Add(job.EndDate.ToString() ?? "");
			if(description)
				row.Add(job.Description ?? "");
			if(experience)
				row.Add(job.Experience ?? "");
			table.AddRow(row.ToArray());
		}

		AnsiConsole.Write(table);
	}
}

public class GetJobCommandSettings : GetCommandSettings
{
	[CommandOption("-i|--id")]
	[Description("output job id")]
	public bool Id { get; set; }

	[CommandOption("-t|--title")]
	[Description("output job title")]
	public bool Title { get; set; }

	[CommandOption("-s|--start")]
	[Description("output job start date")]
	public bool StartDate { get; set; }

	[CommandOption("-e|--end")]
	[Description("output job end date")]
	public bool EndDate { get; set; }

	[CommandOption("-c|--company")]
	[Description("output job company")]
	public bool Company { get; set; }

	[CommandOption("-d|--description")]
	[Description("output job description")]
	public bool Description { get; set; }

	[CommandOption("-x|--experience")]
	[Description("output job experience")]
	public bool Experience { get; set; }

	[CommandArgument(0, "[id]")]
	[Description("id(s) of jobs to retrieve")]
	public long[]? Ids { get; set; }

	public override ValidationResult Validate()
	{
		return Ids != null && Array.Exists(Ids, id => id == null || id < 0)
			? ValidationResult.Error("id must be zero (0) or a positive number")
			: ValidationResult.Success();
	}
}