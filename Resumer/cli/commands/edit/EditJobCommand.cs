using System.Diagnostics.CodeAnalysis;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.edit;

public class EditJobCommand: Command<EditJobSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] EditJobSettings settings)
    {
        throw new NotImplementedException();
    }
}

public class EditJobSettings: CommandSettings
{
}