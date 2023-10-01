using System.ComponentModel;
using resume_builder.cli.settings;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.add;

public class AddCommandSettings : CLISettings
{
	[CommandOption("-i|--interactive")]
	[Description("interactive mode")]
	[DefaultValue(false)]
	public bool Interactive { get; set; }
}