using System.ComponentModel;
using Resumer.cli.settings;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.add;

public class AddCommandSettings: CliSettings
{
    //todo: change to be user first (interactive) and cmd/pipeable second (non interactive)
    //probably make it command level
    [CommandOption("-i|--interactive")]
    [Description("interactive mode")]
    [DefaultValue(false)]
    public bool Interactive { get; set; }
}