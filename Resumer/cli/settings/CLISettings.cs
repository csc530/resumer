using Spectre.Console.Cli;

namespace Resumer.cli.settings;

public class CLISettings : CommandSettings
{
	[CommandOption("-v|--verbose")] public bool Verbose { get; set; }
}