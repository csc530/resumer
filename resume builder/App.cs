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
using System.Diagnostics;

namespace resume_builder
{
	public sealed partial class App
	{
		private static string GetAPPDATAPATH()
		{
#if DEBUG
			var path =".";
#else
			var path = Path.GetFullPath("resume_builder", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create));
#endif
			if(!Path.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}

		private static SqliteConnection SQLDBConnection = InitSqliteConnection();
		private static SqliteConnection BackupSQLDBConnection = InitSqliteConnection(backup: true);
		private static int ReturnCode(ExitCode exitcode) => (int)exitcode;


		public App(IAnsiConsole? console = null)
		{
			if(console != null)
				AnsiConsole.Console = console;
		}

		public int Run(string[] args)
		{


			var app = new CommandApp();
			app.Configure(config =>
			{
#if DEBUG
				config.PropagateExceptions();
				config.ValidateExamples();
#endif
				config.SetApplicationName("resume builder");
				config.SetApplicationVersion("1.0.0");
				config.CaseSensitivity(CaseSensitivity.None);

				config.AddBranch<AddSetting>("add", add =>
				{
					add.SetDescription("add new information to job database/bank");
					add.AddCommand<AddJobCommand>("job")
						.WithDescription("add a new job")
						.WithExample("add", "job", "-s", "2022-01-01", "-e", "2026-11-01", "-t", "foreman");
				});
				config.AddCommand<InitCommand>("init")
				.WithDescription("initializes resume database")
				.WithAlias("start");
			});

			int exitCode = app.Run(args);
			SQLDBConnection.Close();
			return exitCode;
		}

		private static SqliteConnection InitSqliteConnection(bool backup = false)
		{
			string dbName = (backup ? "backup_" : string.Empty) + "resume.sqlite";

			SqliteConnectionStringBuilder sqliteConnectionStringBuilder = new()
			{
				DataSource =Path.Combine(GetAPPDATAPATH(),dbName),
				Mode = SqliteOpenMode.ReadWriteCreate
			};

			var sqldb = new SqliteConnection(sqliteConnectionStringBuilder.ConnectionString);
			sqldb.Open();
			return sqldb;
		}
	}
}