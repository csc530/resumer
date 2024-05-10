using System.ComponentModel;
using Spectre.Console.Cli;

namespace Resumer.cli.settings;

public class DeleteCommandSettings:CommandSettings
{
    [CommandOption("-c|--no-confirm")]
    [Description("Do not ask for confirmation before deleting")]
    public bool NoConfirm { get; set; }

    [CommandOption("-a|--all")]
    [Description("Delete all entries")]
    public bool DeleteAll { get; set; }

}