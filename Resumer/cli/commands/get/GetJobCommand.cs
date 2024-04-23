using System.ComponentModel;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public class GetJobCommand: JobOutputCommand<GetJobCommandSettings>
{
    public override int Execute(CommandContext context, GetJobCommandSettings settings)
    {
        ResumeContext database = new();

        if(!database.Jobs.Any())
            return CommandOutput.Success("No jobs found");
        else
        {
            var table = settings.GetTable(typeof(Job), "Jobs");

            if(table == null)
                return CommandOutput.Error(ExitCode.Error,"Invalid table settings");

            foreach (var job in database.Jobs)
                table.AddObj(job);

            return CommandOutput.Success(table);
        }
    }
}

public class GetJobCommandSettings: JobOutputSettings;