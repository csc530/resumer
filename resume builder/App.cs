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
using resume_builder.commands.add;

namespace resume_builder
{
    public sealed class App
    {
        public App(IAnsiConsole? console = null)
        {
            if (console != null)
                AnsiConsole.Console = console;
        }

        public int Run(string[] args)
        {
            var sqldb = InitSqliteConnection();

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
            sqldb.Close();
            return exitCode;
        }

        private static SqliteConnection InitSqliteConnection()
        {
            SqliteConnectionStringBuilder sqliteConnectionStringBuilder = new()
            {
                DataSource = "resume.sqlite",
                Mode = SqliteOpenMode.ReadWriteCreate
            };

            Console.WriteLine($"sqlite connection string: {sqliteConnectionStringBuilder.ConnectionString}");

            var sqldb = new SqliteConnection(sqliteConnectionStringBuilder.ConnectionString);
            sqldb.Open();
            return sqldb;
        }
    }
}