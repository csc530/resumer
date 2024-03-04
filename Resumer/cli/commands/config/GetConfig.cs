using Resumer.models;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.config;

public class GetConfigCommand: Command<GetConfigCommandSettings>
{
    public override int Execute(CommandContext context, GetConfigCommandSettings settings)
    {
        ResumeContext ResumeContext = new();
        if(settings.Db)
        {
            System.Console.WriteLine("DB Location: " + ResumeContext.DbPath);
        }

        if(settings.Profile)
        {
            System.Console.WriteLine("Profile Location: " + ResumeContext.DbPath);
        }

        if(settings.Jobs)
        {
            System.Console.WriteLine("Jobs Location: " + ResumeContext.DbPath);
        }

        if(settings.Projects)
        {
            System.Console.WriteLine("Projects Location: " + ResumeContext.DbPath);
        }

        if(settings.Companies)
        {
            System.Console.WriteLine("Companies Location: " + ResumeContext.DbPath);
        }

        if(settings.Skills)
        {
            System.Console.WriteLine("Skills Location: " + ResumeContext.DbPath);
        }

        return 0;
    }
}

public class GetConfigCommandSettings: CommandSettings
{
    [CommandOption("-d|--db")] public bool Db { get; set; }
    [CommandOption("--profile")] public bool Profile { get; set; }
    [CommandOption("-j|--jobs")] public bool Jobs { get; set; }
    [CommandOption("-p|--projects")] public bool Projects { get; set; }
    [CommandOption("-c|--companies")] public bool Companies { get; set; }
    [CommandOption("-s|--skills")] public bool Skills { get; set; }
}