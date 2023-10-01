using Spectre.Console.Cli;

namespace resume_builder.cli.settings;

public class CLISettings : CommandSettings
{
	[CommandOption("-v|--verbose")] public bool Verbose { get; set; }
}