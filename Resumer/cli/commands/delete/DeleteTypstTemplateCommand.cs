using Microsoft.EntityFrameworkCore;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.delete;

public class DeleteTypstTemplateCommand: DeleteCommand<TypstTemplate>
{
    protected override DbSet<TypstTemplate> DbSet => Db.Templates;
}