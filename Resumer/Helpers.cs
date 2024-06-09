using System.Collections;
using System.Text;
using Resumer.models;
using Spectre.Console;

namespace Resumer;

public static partial class Utility
{
    /// <summary>
    /// simple string of dashes (100)
    /// </summary>
    public const string DashSeparator = "-----------------------------------------------------------------------------------------------------";

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

    public static string ToCamelCase(this string value) =>
        value.Length switch
        {
            0 => value,
            1 => value[0].ToString().ToLower(),
            _ => char.ToLower(value[0]) + value[1..],
        };

    public static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

    public static string PrintDuration(DateOnly? startDate, DateOnly? endDate = null) =>
        startDate == null ? string.Empty : $"{startDate:MMM yyyy} - {endDate?.ToString("MMM yyyy") ?? "present"}";

    public static string PrintAsTypstVariables(Resume resume)
    {
        var profile = resume.Profile;
        var jobs = resume.Jobs;
        var skills = resume.Skills;
        var projects = resume.Projects;
        var builder = new StringBuilder();
        var var = TypstVariableDeclaration;
        builder.AppendLine(var(nameof(profile.FirstName), profile.FirstName.ToTypstString()));
        builder.AppendLine(var(nameof(profile.MiddleName), profile.MiddleName.ToTypstString()));
        builder.AppendLine(var(nameof(profile.LastName), profile.LastName.ToTypstString()));
        builder.AppendLine(var(nameof(profile.FullName), profile.FullName.ToTypstString()));
        builder.AppendLine(var(nameof(profile.WholeName), profile.WholeName.ToTypstString()));
        builder.AppendLine(var(nameof(profile.EmailAddress), profile.EmailAddress.ToTypstString()));
        builder.AppendLine(var(nameof(profile.PhoneNumber), profile.PhoneNumber.ToTypstString()));
        builder.AppendLine(var(nameof(profile.Location), profile.Location.ToTypstString()));
        builder.AppendLine(var(nameof(profile.Website), profile.Website.ToTypstString()));
        builder.AppendLine(var(nameof(profile.Objective), profile.Objective.ToTypstString()));

        builder.AppendLine(var(nameof(profile.Education), profile.Education.ToTypstString()));
        builder.AppendLine(var(nameof(profile.Languages), profile.Languages.ToTypstString()));

        builder.AppendLine(var(nameof(profile.Interests), profile.Interests.ToTypstString()));

        builder.AppendLine(var(nameof(jobs), jobs.ToTypstString()));
        builder.AppendLine(var(nameof(skills), skills.ToTypstString()));
        builder.AppendLine(var(nameof(projects), projects.ToTypstString()));

        return builder.ToString();
    }

    public static string TypstVariableDeclaration(string name, string value)
    {
        return $"#let {name.ToCamelCase()}={value};";
    }
}

public static class Extensions
{
    // todo: inquire about default value being a property - spectre console pr/iss
    // .DefaultValue(textPrompt);

    public static IEnumerable<T> Clone<T>(this IEnumerable<T> list) where T : ICloneable =>
        list.Select(item => (T)item.Clone()).ToList();

    public static string Print(this object? value)
    {
        if(value == null || value is string ||
           value.GetType().GetInterface(nameof(IEnumerable)) == null) //? check if value is a string or not a collection
            return value?.ToString() ?? string.Empty;

        var list = new List<object?>((IEnumerable<object?>)value);
        return list.Count == 0 ? string.Empty : string.Join("\n", list.Select(obj => $"+ {obj.Print()}"));
    }


    #region strings

    public static string Escape(this string value, string escapeValues) =>
        escapeValues.Aggregate(value,
            (current, escapeValue) => current.Replace($"{escapeValue}", $"\\{escapeValue}"));

    #endregion

    /// <summary>
    /// converts an object to its string representation as a typst object
    /// </summary>
    /// <param name="obj">object to convert</param>
    /// <returns>typst stringified object</returns>
    public static string ToTypstString(this object? obj)
    {
        string typstObj;
        switch(obj)
        {
            case null:
                typstObj = "none";
                break;
            case string value:
                typstObj = $"\"{value.Escape("\"")}\"";
                break;
            case double:
            case float:
            case int:
            case long:
            case short:
            case byte:
            case sbyte:
            case uint:
            case ulong:
            case ushort:
            case decimal:
                typstObj = obj.ToString() ?? "none";
                break;
            case bool:
                typstObj = (obj.ToString() ?? "none");
                typstObj = typstObj.ToLower();
                break;
            case DateTime dateTime:
                typstObj =
                    $"datetime(year: {dateTime.Year}, month: {dateTime.Month}, day: {dateTime.Day}, hour: {dateTime.Hour}, minute: {dateTime.Minute}, second: {dateTime.Second})";
                break;
            case DateOnly dateOnly:
                typstObj = $"datetime(year: {dateOnly.Year}, month: {dateOnly.Month}, day: {dateOnly.Day})";
                break;
            case TimeOnly timeOnly:
                typstObj = $"datetime(hour: {timeOnly.Hour}, minute: {timeOnly.Minute}, second: {timeOnly.Second})";
                break;
            case IDictionary dict:
            {
                var keys = dict.Keys.Cast<string>();
                var items = keys.Select(key => $"{key.ToCamelCase()}: {ToTypstString(dict[key])}");
                typstObj = $"({string.Join(",", items)})";
                break;
            }
            case IEnumerable list:
            {
                var items = list.Cast<object>().Select(ToTypstString).ToArray();
                typstObj = items.Length == 1 ? $"({items[0]},)" : $"({string.Join(",", items)})";
                break;
            }
            default:
            {
                var properties = obj.GetType().GetProperties()
                    .Where(prop => prop.CanRead && Array.Exists(
                        prop.GetAccessors(false),
                        accessor => accessor.IsPublic || accessor.IsFamilyOrAssembly)
                    )
                    .ToList();
                if(properties.Count == 0)
                    typstObj = obj.ToString() == null ? "none" : $"\"{obj.ToString()!.Escape("\"")}\"";
                else
                {
                    var items = properties.Select(prop =>
                        $"{prop.Name.ToCamelCase()}: {ToTypstString(prop.GetValue(obj))}");
                    typstObj = $"({string.Join(",", items)})";
                }

                break;
            }
        }

        return typstObj;
    }



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