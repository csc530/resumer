using System.Data.Common;
using System.Text.RegularExpressions;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Resumer;

public static partial class CommandOutput
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
    ///  Display the name of the exit code with an optional custom error message and return the error code
    /// </summary>
    /// <param name="exitCode">exit code</param>
    /// <param name="msg">optional error message</param>
    /// <param name="help">hint for how to fix the error</param>
    /// <returns>the exit code</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static int Error(ExitCode exitCode, string? msg = null, string? help = null)
    {
        var errNo = (int)exitCode;
        var error = CapitalLettersRegex().Replace(exitCode.ToString(), " $0").Trim();
        AnsiConsole.MarkupLine($"[red]Error 0x{errNo:X4}: {error}[/]");

        if(msg != null)
            AnsiConsole.MarkupLine(msg);
        if(help != null)
            AnsiConsole.MarkupLine($"[italic]{help}[/]");

        AnsiConsole.Reset();
        return errNo;
    }

    /// <summary>
    /// Return the <see cref="ExitCode.Success"/> exit code
    /// </summary>
    /// <param name="msg">optional success message to print to console</param>
    /// <returns>Success exit code</returns>
    public static int Success(string msg)
    {
        AnsiConsole.MarkupLine(msg);
        return Success();
    }

    public static int Success(IRenderable msg)
    {
        AnsiConsole.Write(msg);
        return Success();
    }

    public static int Success() => (int)ExitCode.Success;

    /// <summary>
    /// Display a warning message
    /// </summary>
    /// <param name="message">warning message</param>
    public static void Warn(string message) => AnsiConsole.MarkupLine($"[yellow]{message}[/]");

    [GeneratedRegex("[A-Z]")]
    private static partial Regex CapitalLettersRegex();
}