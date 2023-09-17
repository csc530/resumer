using Spectre.Console;
using Spectre.Console.Cli;
using System.Data;
using Microsoft.Data.Sqlite;

namespace resume_builder
{
    partial class App
    {
        internal class InitCommand : Command
        {
            //todo: edit user table to entries and not columns
            private static readonly List<string> RequiredTables = new()
                { "companies", "jobs", "skills", "job_skills", "user" };

            public override int Execute(CommandContext context)
            {
                if(CheckForRequiredTables(SQLDBConnection) == ExitCode.Success)
                {
                    AnsiConsole.WriteLine("Database already initialized");
                    //todo: provide help to clear, reset, or edit
                    return ReturnCode(ExitCode.Success);
                }
                else if(CheckForRequiredTables(BackupSQLDBConnection) == ExitCode.Success)
                {
                    //prompt to copy to main
                    //indicate backup found
                    return ReturnCode(ExitCode.Success);
                }
                else
                {
                    InitDatabase();
                    if(CheckForRequiredTables(SQLDBConnection) != ExitCode.Success &&
                       CheckForRequiredTables(BackupSQLDBConnection) != ExitCode.Success)
                        return ReturnCode(ExitCode.Error);
                    return ReturnCode(ExitCode.Success);
                }
            }

            private ExitCode CheckForRequiredTables(SqliteConnection sqldbConnection)
            {
                var cmd = sqldbConnection.CreateCommand();
                cmd.CommandText = "SELECT DISTINCT name FROM sqlite_master WHERE type='table' AND name NOT IN ('sqlite_sequence');";
                cmd.Prepare();
                var result = cmd.ExecuteReader();
                List<string> tables = new();
                if(!result.HasRows)
                {
                    result.Close();
                    return ExitCode.NotFound;
                }

                //check if all required tables exist, perf?
                while(result.Read())
                    tables.Add((string)result["name"]);
                result.Close();
                return tables.TrueForAll(name => RequiredTables.Contains(name)) ? ExitCode.Success : ExitCode.NotFound;
            }

            private void InitDatabase()
            {
                var cmd = SQLDBConnection.CreateCommand();
                cmd.CommandText = File.ReadAllText("tables.sql");
                //cmd.Prepare();
                cmd.ExecuteNonQuery();
                SQLDBConnection.BackupDatabase(BackupSQLDBConnection);
                AnsiConsole.WriteLine("Database initialized");
                //add prompt for basic information populaation
            }
        }
    }
}