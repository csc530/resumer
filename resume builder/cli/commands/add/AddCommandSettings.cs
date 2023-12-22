using System.ComponentModel;
using resume_builder.cli.settings;
using Spectre.Console.Cli;

namespace resume_builder.cli.commands.add;

public class AddCommandSettings: CLISettings
{
    //todo: change to be user first (interactive) and cmd/pipeable second (non interactive)
    //probably make it command level
    [CommandOption("-i|--interactive")]
    [Description("interactive mode")]
    [DefaultValue(false)]
    public bool Interactive { get; set; }
}