using System.Collections;
using System.Collections.Immutable;
using System.Text;
using Spectre.Console;

namespace Resumer;

/// <summary>
/// miscellaneous utility functions collection for <see cref="Resumer"/>
/// </summary>
public static partial class Utility
{
    /// <summary>
    /// simple string of dashes (100)
    /// </summary>
    public const string DashSeparator =
        "-----------------------------------------------------------------------------------------------------";

    /// <summary>
    /// a <see cref="TextPrompt{T}"/> wrapper to create a prompt that allows for empty input and does not display the default value which is set to null (default).
    /// </summary>
    /// <typeparam name="T">the type of the prompt input</typeparam>
    /// <param name="message">the prompt message</param>
    public static TextPrompt<T?> SimplePrompt<T>(string message) =>
        new TextPrompt<T?>(message).AllowEmpty().HideDefaultValue().DefaultValue(default);

    /// <inheritdoc cref="SimplePrompt{T}(string)"/>
    /// <param name="defaultValue">the default value</param>
    public static TextPrompt<T> SimplePrompt<T>(string message, T defaultValue) => new TextPrompt<T>(message)
        .AllowEmpty().DefaultValue(defaultValue).HideDefaultValue();

    /// <summary>
    /// converts a string to camel case
    /// </summary>
    /// <param name="value">string to convert</param>
    /// <remarks>if the string is empty or only contains one character, it is returned in lowercase</remarks>
    /// <returns>camel case string</returns>
    public static string ToCamelCase(this string value) =>
        value.Length switch
        {
            0 => value,
            1 => value[0].ToString().ToLower(),
            _ => char.ToLower(value[0]) + value[1..],
        };

    /// <summary>today's date</summary>
    public static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

    /// <summary>
    /// prints a duration between two dates; no validation is performed
    /// </summary>
    /// <param name="startDate">the first date</param>
    /// <param name="endDate">the second date</param>
    /// <returns>stringified duration of dates in the format "MMM yyyy - MMM yyyy"</returns>
    public static string PrintDuration(DateOnly? startDate, DateOnly? endDate = null) =>
        startDate == null ? string.Empty : $"{startDate:MMM yyyy} - {endDate?.ToString("MMM yyyy") ?? "present"}";
}

public static class Extensions
{
    // todo: inquire about default value being a property - spectre console pr/iss
    // .DefaultValue(textPrompt);


#region strings

    /// <summary>
    /// convert an object to a reasonably human-readable string
    /// </summary>
    /// <param name="value">object to convert</param>
    /// <returns>string representation</returns>
    public static string Print(this object? value) =>
        value switch
        {
            null => string.Empty,
            bool bit => bit ? "true" : "false",
            string txt => txt,
            DictionaryEntry pair => $"{pair.Key.Print()}: {pair.Value.Print()}",
            IDictionary dictionary => string.Join("\n", dictionary.Cast<object>().Select(obj => $"- {obj.Print()}")),
            IEnumerable enumerable => string.Join("\n", enumerable.Cast<object>().Select(obj => $"+ {obj.Print()}")),
            _ => value.ToString() ?? string.Empty,
        };

    public static string Escape(this string value, string escapeValues) =>
        escapeValues.Aggregate(value,
            (current, escapeValue) => current.Replace($"{escapeValue}", $"\\{escapeValue}"));


    /// <summary>
    /// converts an object to its string representation as a typst object
    /// </summary>
    /// <param name="obj">object to convert</param>
    /// <param name="pretty">if the output should be pretty-printed, -1 disables pretty-printing, otherwise it represents the number of tabs to indent: 0 = no indentation</param>
    /// <returns>typst stringified object</returns>
    public static string ToTypstString(this object? obj, int pretty = -1)
    {
        const int notPretty = -1;
        var prettyPrint = pretty != -1;
        if(prettyPrint)
            pretty++;
        var prettyTabs = new StringBuilder();
        for(var i = 0; i < pretty; i++)
            prettyTabs.Append('\t');

        switch(obj)
        {
            case null:
                return "none";
            case string value:
                return $"\"{value.Escape("\"")}\"";
            case int or double or float or long or ulong or short or ushort or byte or sbyte or decimal or char:
                return obj.ToString() ?? "none";
            case bool value:
                return value.ToString().ToLower();
            case DateTime dateTime:
                return prettyPrint
                    ? $"datetime(year: {dateTime.Year}, month: {dateTime.Month}, day: {dateTime.Day}, hour: {dateTime.Hour}, minute: {dateTime.Minute}, second: {dateTime.Second})"
                    : $"datetime(year:{dateTime.Year},month:{dateTime.Month},day:{dateTime.Day},hour:{dateTime.Hour},minute:{dateTime.Minute},second:{dateTime.Second})";
            case DateOnly dateOnly:
                return prettyPrint
                    ? $"datetime(year: {dateOnly.Year}, month: {dateOnly.Month}, day: {dateOnly.Day})"
                    : $"datetime(year:{dateOnly.Year},month:{dateOnly.Month},day:{dateOnly.Day})";
            case TimeOnly timeOnly:
                return prettyPrint
                    ? $"datetime(hour: {timeOnly.Hour}, minute: {timeOnly.Minute}, second: {timeOnly.Second})"
                    : $"datetime(hour:{timeOnly.Hour},minute:{timeOnly.Minute},second:{timeOnly.Second})";
            case IDictionary<object?, object?> dict:
            {
                if(!prettyPrint)
                    return
                        $"({string.Join(",", dict.Keys.Select(key => $"{key.ToTypstString().ToCamelCase()}: {(dict[key].ToTypstString(notPretty))}"))})";

                var sb = new StringBuilder();
                sb.Append('(');


                if(dict.Count > 3)
                {
                    sb.AppendLine();
                    sb.Append(prettyTabs);
                    sb.AppendJoin($",\n{prettyTabs}",
                        dict.Keys.Select(key =>
                            $"{key.ToTypstString().ToCamelCase()}=> {ToTypstString(dict[key], pretty)}"));
                }
                else
                    sb.AppendJoin(", ",
                        dict.Keys.Select(key =>
                            $"{key.ToTypstString().ToCamelCase()}=> {ToTypstString(dict[key], pretty)}"));

                sb.AppendLine(")");
                return sb.ToString();
            }
            case IEnumerable<object?> enumerable:
            {
                var list = enumerable.ToList();
                if(!prettyPrint || list.Count <= 1)
                    return list.Count == 1
                        ? $"({list[0].ToTypstString()},)"
                        : $"({string.Join(',', list.Select(item => item.ToTypstString(pretty)))})";

                var sb = new StringBuilder();
                sb.Append('(');

                if(list.Count > 3)
                {
                    sb.AppendLine();
                    sb.Append(prettyTabs);
                    sb.AppendJoin($",\n{prettyTabs}", list.Select(item => item.ToTypstString(pretty)));
                    sb.AppendLine();
                    sb.Append(prettyTabs.Remove(0, 1));
                }
                else
                    sb.AppendJoin(", ", list.Select(item => item.ToTypstString(pretty)));

                sb.Append(')');
                return sb.ToString();
            }
            default:
            {
                //? get all public readable properties and convert to key-value typst strings
                var properties = obj.GetType()
                    .GetProperties()
                    .Where(prop => prop.CanRead && Array.Exists(
                        prop.GetAccessors(false),
                        accessor => accessor.IsPublic || accessor.IsFamilyOrAssembly)
                    );
                var records = (prettyPrint
                        ? properties
                            .Select(prop => $"{prop.Name.ToCamelCase()}: {prop.GetValue(obj).ToTypstString(pretty)}")
                        : properties.Select(prop =>
                            $"{prop.Name.ToCamelCase()}:{ToTypstString(prop.GetValue(obj), pretty)}"))
                    .ToImmutableArray();
                //? if there are no visible properties, return its string representation
                if(records.Length == 0)
                    return obj.ToString().ToTypstString(pretty);
                if(!prettyPrint)
                    return $"({string.Join(",", records)})";

                var sb = new StringBuilder();
                sb.Append('(');
                if(records.Length > 3)
                {
                    sb.AppendLine();
                    sb.Append(prettyTabs);
                    sb.AppendJoin($",\n{prettyTabs}", records);
                    sb.AppendLine();
                    //? remove the first tab since prettytabs is for child level-indentation: tl;dr bring it back to parent lvl indentation
                    sb.Append(prettyTabs.Remove(0, 1));
                }
                else
                    sb.AppendJoin(", ", records);

                sb.Append(')');
                return sb.ToString();
            }
        }
    }

#endregion

    public static void AddFromPrompt<T>(this List<T> list, string prompt)
    {
        var textPrompt = Utility.SimplePrompt<T>(prompt);
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
                input = AnsiConsole.Prompt(new TextPrompt<string>(prompt + " (enter '-' to delete the entry)")
                    .DefaultValue(description[i]).AllowEmpty());
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
                input = AnsiConsole.Prompt(Utility.SimplePrompt<string>(prompt));
                if(!string.IsNullOrWhiteSpace(input))
                    description.Add(input);
            }
        } while(i < count || !string.IsNullOrWhiteSpace(input));
    }
}