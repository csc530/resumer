using System.Collections;
using System.Reflection;
using Resumer.models;
using Spectre.Console;

namespace Resumer;

public static partial class Utility
{
    public static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

    public static string PrintDuration(DateOnly? startDate, DateOnly? endDate = null) =>
        startDate == null ? string.Empty : $"{startDate:MMM yyyy} - {endDate?.ToString("MMM yyyy") ?? "present"}";
}

public static class Extensions
{
    // todo: inquire about default value being a property - spectre console pr/iss
    // .DefaultValue(textPrompt);

    public static IEnumerable<T> Clone<T>(this IEnumerable<T> list) where T : ICloneable =>
        list.Select(item => (T)item.Clone()).ToList();

    public static string Print(this object? value)
    {
        if(value == null || value is string || value.GetType().GetInterface(nameof(IEnumerable)) == null) //? check if value is a string or not a collection
            return value?.ToString() ?? string.Empty;

        var list = new List<object?>((IEnumerable<object?>)value);
        return list.Count == 0 ? string.Empty : string.Join("\n", list.Select(obj => $"+ {obj.Print()}"));
    }


    #region strings

    public static string Prefix(this string s, string txt) => $"{txt}{s}";

    public static List<string> Prefix(this IEnumerable<string> strings, string txt) =>
        strings.Select(s => s.Prefix(txt)).ToList();

    public static string Surround(this string? s, string txt) => $"{txt}{s}{txt}";


    /// <summary> surrounds each string within a list with another string.</summary>
    ///
    /// <param name="strings"> the list strings to be surrounded.
    /// </param>
    /// <param name="txt"> The text to surround the strings with.</param>
    ///
    /// <returns> A list of strings with the parameter txt surrounding each string in the original list.</returns>
    public static List<string> Surround(this IEnumerable<string> strings, string txt) =>
        strings.Select(s => s.Surround(txt)).ToList();

    #endregion

    public static void AddFromPrompt<T>(this List<T> list, string prompt)
    {
        var textPrompt = new SimplePrompt<T>(prompt);
        T input;

        do
        {
            input = AnsiConsole.Prompt(textPrompt);
            if(input != null)
                list.Add(input);
        } while(input != null && !string.IsNullOrWhiteSpace(input.ToString()));
    }

    public static List<string> AddFromPrompt(IEnumerable<string> list, string prompt)
    {
        var textPrompt = new TextPrompt<string?>(prompt).HideDefaultValue().AllowEmpty();
        string? input;
        var newlist = list.ToList();

        do
        {
            input = AnsiConsole.Prompt(textPrompt);
            if(!string.IsNullOrWhiteSpace(input))
                newlist.Add(input);
        } while(!string.IsNullOrWhiteSpace(input));

        return newlist;
    }

    public static void EditFromPrompt(this List<string> description, string prompt)
    {
        string? input;
        var i = 0;
        var count = description.Count; //? count is not updated when description is modified;

        do
        {
            if(i < count)
            {
                input = AnsiConsole.Prompt(new TextPrompt<string>(prompt).DefaultValue(description[i]).AllowEmpty());
                if(string.IsNullOrWhiteSpace(input) || input == "-")
                {
                    description.RemoveAt(i);
                    count--;
                    i--;
                }
                else
                    description[i] = input;

                i++;
            }
            else
            {
                input = AnsiConsole.Prompt(new SimplePrompt<string>(prompt));
                if(!string.IsNullOrWhiteSpace(input))
                    description.Add(input);
            }
        } while(i < count || !string.IsNullOrWhiteSpace(input));
    }

    public static string GetMessage(this SqlResultCode code) => code switch
    {
        SqlResultCode.Success => "Success",
        SqlResultCode.Error => "Error",
        SqlResultCode.Readonly => "Database is readonly",
        SqlResultCode.IoErr => "disk I/O error occurred",
        SqlResultCode.NotNull => "not null constraint violated",
        SqlResultCode.Abort => "Operation terminated by interrupt (sqlite3_interrupt)",
        SqlResultCode.Constraint => "constraint violation",
        _ => "Unknown error"
    };
}

/// <summary>
/// a <see cref="TextPrompt{T}"/> wrapper to create a prompt that allows for empty input and does not display the default value which is set to null (default).
/// </summary>
/// <typeparam name="T">the type of the prompt input</typeparam>
public class SimplePrompt<T> : IPrompt<T?>
{
    private readonly TextPrompt<T?> _textPrompt;

    public SimplePrompt(string message, T? defaultValue = default)
    {
        _textPrompt = new TextPrompt<T?>(message).AllowEmpty().DefaultValue(defaultValue).HideDefaultValue();
    }


    public T? Show(IAnsiConsole console) => _textPrompt.Show(console);

    public Task<T?> ShowAsync(IAnsiConsole console, CancellationToken cancellationToken) =>
        _textPrompt.ShowAsync(console, cancellationToken);
}