using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using resume_builder.models;
using Spectre.Console;
using Spectre.Console.Cli;
using static resume_builder.Globals;

namespace resume_builder.cli.commands.get;

public class GetJobCommand : Command<GetJobCommandSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] GetJobCommandSettings settings)
	{
		Database database = new();
		var (plain, expand, id, title, startDate, endDate, company, description, experience, ids) = settings;
		var rows = database.GetJob(ids ?? Array.Empty<long>());
		var jobs = rows.Values;
		if(jobs.Count == 0)
			AnsiConsole.MarkupLine("No jobs found");
		else
		{
			var table = new Table().Centered().Border(TableBorder.Rounded).Expand();
			if(!id && !title && !company && !startDate && !endDate && !description && !experience)
			{
				table.Centered().Border(TableBorder.Rounded).Expand().AddColumn(new TableColumn("ID"))
				     .AddColumn(new TableColumn("Job Title"))
				     .AddColumn(new TableColumn("Company"))
				     .AddColumn(new("Start Date"))
				     .AddColumn(new("End Date"))
				     .AddColumn(new("Description"))
				     .AddColumn(new("Experience"));
				foreach(var (tblId, job) in rows)
					table.AddRow(tblId.ToString(), job.Title, job.Company ?? "", job.StartDate.ToString(),
						job.EndDate.ToString() ?? "", job.Description ?? "", job.Experience ?? "");
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


				foreach(var (tblId, job) in rows)
				{
					var row = new List<string>();
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
			}

			AnsiConsole.Write(table);
		}

		return ExitCode.Success.ToInt();
	}
}

public class GetJobCommandSettings : GetCommandSettings
{
	public void Deconstruct(out bool plain, out bool expand, out bool id, out bool title, out bool startDate,
	                        out bool endDate, out bool company, out bool description, out bool experience,
	                        out long[]? ids)
	{
		plain = Plain;
		expand = Expand;
		id = Id;
		title = Title;
		startDate = StartDate;
		endDate = EndDate;
		company = Company;
		description = Description;
		experience = Experience;
		ids = Ids;
	}

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

	[CommandOption("-p|--experience")]
	[Description("output job experience")]
	public bool Experience { get; set; }

	[CommandArgument(0, "[id]")]
	[Description("id(s) of jobs to retrieve")]
	public long[]? Ids { get; set; }
	//todo: add discriminators/options for each field indicating what to return

	public override ValidationResult Validate()
	{
		return Ids != null && Ids.Any(id => id == null || id < 0)
			? ValidationResult.Error("id must be zero (0) or a positive number")
			: ValidationResult.Success();
	}
}