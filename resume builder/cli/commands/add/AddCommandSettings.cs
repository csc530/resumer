using System.ComponentModel;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.add;

public class AddCommandSettings : CommandSettings
{
	[CommandOption("-i|--interactive")]
	[Description("interactive mode")]
	[DefaultValue(false)]
	public bool Interactive { get; set; }
}