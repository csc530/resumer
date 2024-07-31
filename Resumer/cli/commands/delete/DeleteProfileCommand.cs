using Microsoft.EntityFrameworkCore;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Profile = Resumer.models.Profile;

namespace Resumer.cli.commands.delete;

public class DeleteProfileCommand : DeleteCommand<Profile>
{
    protected override DbSet<Profile> DbSet => Db.Profiles;

}