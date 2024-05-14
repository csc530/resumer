using System.ComponentModel;
using System.Text;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.config;

public class GetConfigCommand: Command<GetConfigCommandSettings>
{
    public override int Execute(CommandContext context, GetConfigCommandSettings settings)
    {
        ResumeContext resumeContext = new();
        var settingOptions = settings.Settings;

        if(settingOptions == null || settingOptions.Length == 0)
            settingOptions = Enum.GetValues<SettingOptions>().Select(s => s.ToString().ToLower()).ToArray();

        StringBuilder output = new();
        foreach(var setting in settingOptions)
            switch(setting.ToLower())
            {
                case "db":
                case "database":
                    output.AppendLine("Database Location: " + resumeContext.DbPath);
                    break;
                default:
                    return CommandOutput.Error(ExitCode.InvalidArgument, $"Unknown setting: [bold]{setting}[/]");
            }

        output.Remove(output.Length - 1, 1); // remove trailing newline
        return CommandOutput.Success(output.ToString());
    }
}

public class GetConfigCommandSettings: OutputCommandSettings
{
    [CommandArgument(0, "[settings]")]
    [Description("Settings to view. Default is all settings.")]
    public required string[]? Settings { get; set; }
}

public enum SettingOptions
{
    database
}