using System.Collections;
using System.Reflection;
using Spectre.Console;

namespace Resumer;

public static partial class Utility
{
    public static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

    public static string PrintDuration(DateOnly? startDate, DateOnly? endDate = null) =>
        startDate == null ? string.Empty : $"{startDate:MMM yyyy} - {endDate?.ToString("MMM yyyy") ?? "present"}";
}

public static partial class Extensions
{
    // todo: inquire about default value being a property - spectre console pr/iss
    // .DefaultValue(textPrompt);

    public static IEnumerable<T> Clone<T>(this IEnumerable<T> list) where T : ICloneable =>
        list.Select(item => (T)item.Clone()).ToList();

    public static void AddObj<T>(this Table table, T obj)
    {
        if(obj == null)
            return;

        var row = obj.GetType().GetProperties()
            .Where(prop => prop.CanRead && prop.MemberType == MemberTypes.Property)
            .Select(prop => Markup.Escape(prop.GetValue(obj).Print()))
            .ToArray();
        table.AddRow(row);
    }

    public static string Print(this object? value)
    {
        if(value == null)
            return string.Empty;

        if(value is not string && value.GetType().GetInterface(nameof(IEnumerable)) != null)
            return string.Join("\n+ ", (IEnumerable<object?>)value);

        return value.ToString() ?? string.Empty;
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

    #region Table

    /// <summary>
    /// add column values to the table based on conditions
    /// </summary>
    /// <param name="table">the table to append column values</param>
    /// <param name="columns">a key-value pair of column condition and column values; if the column condition is true, the column value will be added to the table.
    /// if the column condition is false, the column (value) will be skipped</param>
    public static Table AddTableRow(this Table table, params KeyValuePair<bool, object?>[] columns)
    {
        var row = columns.Where(col => col.Key)
            .Select(col => col.Value?.ToString() ?? string.Empty) //todo: handle null values; i.e. print overload
            .ToArray();
        table.AddRow(row);
        return table;
    }

    public static Table AddTableColumn(this Table table, string name, bool nowrap = false) =>
        table.AddColumn(name, c => c.NoWrap = nowrap);

    public static Table AddTableColumn(this Table table, bool nowrap = false, params string[] columns)
    {
        foreach(var column in columns)
            table.AddTableColumn(column, nowrap);
        return table;
    }

    public static Table AddTableColumn(this Table table, params string[] columns)
    {
        foreach(var column in columns)
            table.AddTableColumn(column);
        return table;
    }

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