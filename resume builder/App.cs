using Microsoft.Data.Sqlite;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace resume_builder
{
	public sealed class App
	{
		private SqliteConnection _connection;
		private IAnsiConsole Console = AnsiConsole.Console;

		public App(IAnsiConsole? console = null)
		{
			if(console != null)
				AnsiConsole.Console = console;
			_connection = new SqliteConnection();
		}

		public int Run(string[] args)
		{
			SqliteConnectionStringBuilder sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder();
			sqliteConnectionStringBuilder.DataSource = "resume.db";

			sqliteConnectionStringBuilder.Mode = SqliteOpenMode.ReadWriteCreate;

			Console.WriteLine($"sqlite connection string: {sqliteConnectionStringBuilder.ConnectionString}");

			var sqldb = new SqliteConnection(sqliteConnectionStringBuilder.ConnectionString);
			sqldb.Open();

			var app = new CommandApp();
			app.Configure(config =>
			{
#if DEBUG
				config.PropagateExceptions();
				config.ValidateExamples();
#endif
				config.AddBranch<>("add")


				AddCommand<AddJob>("add job")
				.WithDescription("add a new job");
			});

			int exitcode = app.Run(args);
			sqldb.Close();
			return exitcode;
		}

		public class AddSetting : CommandSettings { }
		public class AddJobSettings: AddSetting
		{
			[Description("start date at the job")]
			[CommandOption("-s|--start")]
			public DateOnly? StartDate { get; init; }

			[Description("last date at the job")]
			[CommandOption("-e|--end")]
			public DateOnly? EndDate { get; init; }

			[Description("job title")]
			[CommandOption("-t|--title")]
			public string? JobTitle { get; init; }
		}
	

		internal sealed class AddJobCommand : Command<AddJobSettings>
		{

			public override int Execute([NotNull] CommandContext context, [NotNull] AddJobSettings settings)
			{
				throw new NotImplementedException();
			}
		}
	}
}
