using Microsoft.EntityFrameworkCore;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.delete;

public class DeleteSkillCommand: DeleteCommand<Skill>
{
    protected override DbSet<Skill> DbSet => Db.Skills;

}