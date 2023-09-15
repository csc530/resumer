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
using resume_builder;

namespace resume_builder
{
    public sealed partial class App
    {
		static readonly string APPDATAPATH = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create)}/resume_builder";
		static private SqliteConnection SQLDBConnection = InitSqliteConnection();
		static private SqliteConnection BackupSQLDBConnection = InitSqliteConnection(backup: true);

		public App(IAnsiConsole? console = null)
        {
            if (console != null)
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

                config.AddBranch<AddSetting>("add", add =>
                {
                    add.SetDescription("add new information to job database/bank");
                    add.AddCommand<AddJobCommand>("job")
                        .WithDescription("add a new job")
                        .WithExample("add", "job", "-s", "2022-01-01", "-e", "2026-11-01", "-t", "foreman");
                });
            });

            int exitCode = app.Run(args);
            SQLDBConnection.Close();
            return exitCode;
        }

        private static SqliteConnection InitSqliteConnection(bool backup = false)
        {
            SqliteConnectionStringBuilder sqliteConnectionStringBuilder = new()
            {
                DataSource = backup ? $"{APPDATAPATH}/resume.sqlite" : $"{APPDATAPATH}/backup_resume.sqlite",
                Mode = SqliteOpenMode.ReadWriteCreate
            };

            Console.WriteLine($"sqlite connection string: {sqliteConnectionStringBuilder.ConnectionString}");

            var sqldb = new SqliteConnection(sqliteConnectionStringBuilder.ConnectionString);
            sqldb.Open();
            return sqldb;
        }
    }
}