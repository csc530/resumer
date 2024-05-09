using System.Text;
using Resumer.models;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.config;

public class GetConfigCommand: Command<GetConfigCommandSettings>
{
    public override int Execute(CommandContext context, GetConfigCommandSettings settings)
    {
        ResumeContext resumeContext = new();
        if(settings.Settings.Length == 0)
            return CommandOutput.Success();

        StringBuilder output = new();

        foreach(var setting in settings.Settings)
            switch(setting.ToLower())
            {
                case "db":
                case "database":
                    output.AppendLine("DB Location: " + resumeContext.DbPath);
                    break;
                default:
                    return CommandOutput.Error(ExitCode.InvalidArgument, "Unknown setting: " + setting);
            }

        return CommandOutput.Success(output.ToString());
    }
}

public class GetConfigCommandSettings: CommandSettings
{
    [CommandArgument(0, "[settings]")] public string[] Settings { get; set; }
}