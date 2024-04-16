using System.Data.Common;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;

namespace Resumer;

public static class CommandOutput
{
    public static int Error(Exception exception, CliSettings settings)
    {
        var error = exception switch
        {
            //todo: verbose detailed exception output
            DbException => ExitCode.DbError,
            InvalidOperationException => ExitCode.NoData,
            _ => ExitCode.Unknown
        };
        return Error(error, exception.Message);
    }

    /// <summary>
    ///  Display an error message and return the error code
    /// </summary>
    /// <param name="exitCode"></param>
    /// <param name="msg"></param>
    /// <param name="help"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static int Error(ExitCode exitCode, string msg, string? help = null)
    {
        AnsiConsole.Foreground = Color.Red;
        var errNo = (int)exitCode;
        AnsiConsole.WriteLine($"Error {errNo}: {exitCode}");
        AnsiConsole.MarkupLine(msg);

        if(help != null)
        {
            AnsiConsole.Foreground = Color.Cyan2;
            AnsiConsole.MarkupLine(help);
        }

        AnsiConsole.Reset();
        return errNo;
    }

    /// <summary>
    /// Return the <see cref="ExitCode.Success"/> exit code
    /// </summary>
    /// <param name="msg">optional success message to print to console</param>
    /// <returns>Success exit code</returns>
    public static int Success(string? msg = null)
    {
        if(msg == null)
            AnsiConsole.WriteLine();
        else
            AnsiConsole.MarkupLine(msg);
        return (int)ExitCode.Success;
    }

    /// <summary>
    /// Display a warning message
    /// </summary>
    /// <param name="message">warning message</param>
    public static void Warn(string message) => AnsiConsole.MarkupLine($"[yellow]{message}[/]");
}