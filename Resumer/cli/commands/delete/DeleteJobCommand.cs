using Microsoft.EntityFrameworkCore;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.delete;

public class DeleteJobCommand: DeleteCommand<Job>
{
    protected override DbSet<Job> DbSet  => Db.Jobs;
}