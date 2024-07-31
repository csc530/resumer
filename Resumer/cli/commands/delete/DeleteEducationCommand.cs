using Microsoft.EntityFrameworkCore;
using Resumer.models;

namespace Resumer.cli.commands.delete;

public class DeleteEducationCommand: DeleteCommand<Education>
{
    protected override DbSet<Education> DbSet => Db.Education;
}