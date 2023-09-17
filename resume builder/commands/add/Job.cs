using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace resume_builder;

public partial class App
{
    public class AddSetting : CommandSettings
    {
    }

    public class AddJobSettings : AddSetting
    {
        [Description("start date at the job")]
        [CommandOption("-s|--start <StartDate>")]
        public DateOnly? StartDate { get; init; }

        [Description("last date at the job")]
        [CommandOption("-e|--end")]
        public DateOnly? EndDate { get; init; }

        [Description("job title")]
        [CommandOption("-t|--title")]
        public string? JobTitle { get; init; }

        [Description("posted/official job description by the employer")]
        [CommandOption("-d|--description")]
        public string? JobDescription { get; init; }

        [Description("your (personal) experience at the job")]
        [CommandOption("-x|--experience")]
        public string? Experience { get; init; }

        public override ValidationResult Validate()
        {
            if(string.IsNullOrWhiteSpace(JobTitle))
                return ValidationResult.Error("Job title is required");
            if(StartDate == null || EndDate > StartDate)
                return ValidationResult.Error("Start date is required");
            return ValidationResult.Success();
        }
    }

    internal sealed class AddJobCommand : Command<AddJobSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] AddJobSettings settings)
        {
            var cmd = SQLDBConnection.CreateCommand();

            cmd.CommandText = "INSERT INTO jobs(title,'start date', 'end date') VALUES(@title, $start, :end);";
            cmd.Parameters.AddWithValue("title",settings.JobTitle);
            cmd.Parameters.AddWithValue("start", settings.StartDate);
            cmd.Parameters.AddWithValue("end", settings.EndDate);

            AnsiConsole.WriteLine(cmd.CommandText);
            cmd.Prepare();
            AnsiConsole.WriteLine(cmd.CommandText);
            cmd.ExecuteReader();
            cmd.ExecuteNonQuery();

            SQLDBConnection.BackupDatabase(BackupSQLDBConnection);

            AnsiConsole.WriteLine(
                $"{context}\ntitle: {settings.JobTitle}\n start: {settings.StartDate}\n end: {settings.EndDate}");
            return ReturnCode(ExitCode.Success);
        }
    }
}