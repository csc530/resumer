using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.get;

public class GetCommandSettings : CommandSettings
{
	public virtual Table? GetTable(string? title = null)
	{
		if(Plain)
			return null;

		return new Table
		{
			Expand = (Expand && !Minimize) || (!Minimize && !Expand),
			Border = Border,
			ShowFooters = Footer,
			ShowHeaders = true,
			Title = string.IsNullOrWhiteSpace(title) ? null : new($"[BOLD]{title}[/]")
		};
	}

	[CommandOption("-b|--border")]
	[Description("table border style")]
	public TableBorder Border { get; set; } = TableBorder.Rounded;

	[CommandOption("-p|--plain")]
	[Description("output in plain text")]
	public bool Plain { get; set; }

	[CommandOption("-g|--expand")]
	[Description("output in expanded format: maximum width of each column (default)")]
	[DefaultValue(false)]
	public bool Expand { get; set; }

	[CommandOption("-m|--minimize")]
	[Description("output in condensed format: minimize the tables size")]
	[DefaultValue(false)]
	public bool Minimize { get; set; }

	[CommandOption("-f|--footer")]
	[Description("show table footer")]
	public bool Footer { get; set; }
}