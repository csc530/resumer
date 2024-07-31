using Microsoft.EntityFrameworkCore;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.delete;

public class DeleteProjectCommand: DeleteCommand<Project>
{
    protected override DbSet<Project> DbSet => Db.Projects;

}