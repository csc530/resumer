using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.Sqlite;
using Resumer.cli.settings;
using Resumer.models;
using Spectre.Console;

namespace Resumer;

public static partial class Globals
{
    public const string NullString = "<null/>";
    public static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);
}

public static partial class Extensions
{
    // todo: inquire about default value being a property - spectre console pr/iss
    // .DefaultValue(textPrompt);


    public static string GetPrintValue(this string? value, bool allowBlank = false)
    {
        if(allowBlank)
            return value ?? "";
        return string.IsNullOrWhiteSpace(value) ? Globals.NullString : value;
    }

    public static string GetPrintValue(this object? value, bool allowBlank = false) =>
        GetPrintValue(value?.ToString(), allowBlank);

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

    public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);
    public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);

    /// <summary>
    /// check if a string is null, empty, or whitespace
    /// </summary>
    /// <param name="s">string to check</param>
    /// <returns>true if the string is null, empty, or whitespace; otherwise false</returns>
    public static bool IsBlank([NotNullWhen(false)] this string? s) =>
        s == null || string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);

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
                         .Select(col => col.Value.GetPrintValue(allowBlank: true))
                         .ToArray();
        table.AddRow(row);
        return table;
    }

    public static Table AddTableColumn(this Table table, string name, bool nowrap = false) =>
        table.AddColumn(RenderableFactory.CreateTableColumn(name, nowrap));

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
}

public static class RenderableFactory
{
    public static TableColumn CreateTableColumn(string name, bool nowrap = false) => new TableColumn(name)
    {
        Footer = new Text(name),
        Header = new Text(name),
        NoWrap = nowrap
    };


    /// <inheritdoc cref="CreateTextPrompt{T}(string,bool,System.Func{T,Spectre.Console.ValidationResult}?)"/>
    /// <param name="defaultValue">The default value of the prompt.</param>
    /// <remarks>If the default value is null and allowEmpty is false, the default value will not be displayed (empty brackets) nor set.</remarks>
    public static TextPrompt<T> CreateTextPrompt<T>(string prompt, T defaultValue, bool allowEmpty = false,
        Func<T, ValidationResult>? validator = null)
    {
        if(defaultValue == null && !allowEmpty)
            return CreateTextPrompt(prompt, allowEmpty, validator);
        return CreateTextPrompt(prompt, allowEmpty, validator).DefaultValue(defaultValue).ShowDefaultValue();
    }

    ///<summary>
    /// Creates a text prompt with the specified prompt message, default value, and optional parameters.
    /// </summary>
    ///
    /// <typeparam name="T">The type of the prompt result.</typeparam>
    /// <param name="prompt">The prompt message to display to the user.</param>
    /// <param name="allowEmpty">Optional. Specifies whether empty input is allowed. Defaults to false.</param>
    /// <param name="validator">Optional. A function that validates the input value. Defaults to null.</param>
    ///
    /// <returns>A <see cref="TextPrompt{T}"/> object representing the created text prompt.</returns>
    public static TextPrompt<T> CreateTextPrompt<T>(string prompt, bool allowEmpty = false,
        Func<T, ValidationResult>? validator = null) =>
        new(prompt)
        {
            AllowEmpty = allowEmpty,
            Validator = validator,
            ShowDefaultValue = false
        };

    public static T Show<T>(this TextPrompt<T> textPrompt) => textPrompt.Show(AnsiConsole.Console);
}