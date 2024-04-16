using Spectre.Console.Cli;

namespace Resumer.cli.commands.edit;

public class EditJobCommand: Command<EditJobSettings>
{
    public override int Execute(CommandContext context, EditJobSettings settings)
    {
        throw new NotImplementedException();
    }
}

public class EditJobSettings: CommandSettings
{
}