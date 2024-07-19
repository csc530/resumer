using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.add;

public abstract class AddCommand: Command<AddCommandSettings>
{
    /** prompt displayed to continue adding items */
    protected virtual string ContinuePrompt { get; } = "Add another item?";

    public sealed override int Execute(CommandContext context, AddCommandSettings settings)
    {
        int result;
        do
        {
            result = AddItem(context, settings);
        } while(!settings.NoContinue && result == CommandOutput.Success() && AnsiConsole.Confirm(ContinuePrompt));

        return result;
    }

    protected abstract int AddItem(CommandContext context, AddCommandSettings settings);
}

public abstract class AddCommand<T>: AddCommand where T : AddCommandSettings
{
    protected abstract int AddItem(CommandContext context, T settings);
    protected sealed override int AddItem(CommandContext context, AddCommandSettings settings) => AddItem(context, (T)settings);
};

public class AddCommandSettings: CommandSettings
{
    [CommandOption("-n|--no-continue|--no")]
    [Description("do not prompt to continue adding items")]
    public bool NoContinue { get; set; }
}