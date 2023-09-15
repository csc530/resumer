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
        public DateOnly? StartDate { get; }

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
            cmd.CommandType = CommandType.StoredProcedure;
			Microsoft.Data.Sqlite.SqliteParameter sqliteParameter = cmd.CreateParameter();
            sqliteParameter.ParameterName = "title";
            sqliteParameter.Value = settings.JobTitle;
            sqliteParameter.DbType = DbType.String;
            cmd.CommandText = "INSERT INTO jobs VALUES($title,);";
            SQLDBConnection.CreateFunction<int>("add job", pop => Console.WriteLine());
            SQLDBConnection.
            SQLDBConnection.BackupDatabase(BackupSQLDBConnection)
            AnsiConsole.WriteLine($"{context}\ntitle: {settings.JobTitle}\n start: {settings.StartDate}\n end: {settings.EndDate}");
            return (int)ExitCode.Success;
        }
    }
}