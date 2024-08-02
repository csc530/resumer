using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.add;

public abstract class AddCommand: AddCommand<AddCommandSettings>;

public abstract class AddCommand<T>: Command<T> where T :  notnull, AddCommandSettings
{
    /** prompt displayed to continue adding items */

    protected virtual string ContinuePrompt { get; } = "Add another item?";
    protected abstract int AddItem(CommandContext context, T settings);
    public sealed override int Execute(CommandContext context, T settings)
    {
        int result;
        do
        {
            result = AddItem(context, settings);
        } while(!settings.NoContinue && result == CommandOutput.Success() && AnsiConsole.Confirm(ContinuePrompt));

        return result;
    }
};

public class AddCommandSettings: CommandSettings
{
    [CommandOption("-n|--no-continue|--no")]
    [Description("do not prompt to continue adding items")]
    public bool NoContinue { get; set; }
}