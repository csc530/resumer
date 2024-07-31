using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;

namespace Resumer;

public static partial class CommandOutput
{
    public static int Error(Exception exception, CommandSettings settings)
    {
        var error = exception switch
        {
            //todo: verbose detailed exception output
            DbException => ExitCode.DbError,
            InvalidOperationException => ExitCode.NoData,
            _ => ExitCode.Unknown,
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

    /// <summary>
    /// print a verbose message
    /// </summary>
    /// <param name="label">message label/header</param>
    /// <param name="text">message</param>
    /// <param name="settingsVerbose">helper to determine if verbose output should be printed</param>
    public static void Verbose(string label, string text, bool settingsVerbose = true)
    {
        if(!settingsVerbose)
            return;
        var txtLabel = new Text(label, new Style(Color.Aqua))
        {
            Justification = Justify.Left,
            Overflow = Overflow.Fold,
        };
        var txtText = new Text(text) { Justification = Justify.Left, Overflow = Overflow.Fold };
        AnsiConsole.Write(txtLabel);
        AnsiConsole.Markup("[aqua]:[/] ");
        AnsiConsole.Write(txtText);
        AnsiConsole.WriteLine();
    }

    public static Table AddObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        this Table table, T obj)
    {
        if(obj is null)
            return table;


        var values = typeof(T).GetProperties()
            .Where(prop => prop.CanRead)
            .Select(prop => prop.GetValue(obj).Print())
            .ToArray();

        table.AddRow(values);


        return table;
    }

    public static void Json(object obj)
    {
        var json = JsonSerializer.SerializeToDocument(obj, new JsonSerializerOptions(JsonSerializerDefaults.General) { WriteIndented = true });
        if(json == null)
            throw new InvalidOperationException("could not serialize object to json");
        var output = new Spectre.Console.Json.JsonText(json.ToString());
        AnsiConsole.Write(output);
    }

    public static Table AddObjects<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        this Table table, IEnumerable<T> objs) where T : class
    {
        foreach(var item in objs)
            AddObject(table, item);
        return table;
    }
}