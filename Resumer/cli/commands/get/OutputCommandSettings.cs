using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Resumer.models;
using Spectre.Console;
using Spectre.Console.Cli;
using Profile = Resumer.models.Profile;

namespace Resumer.cli.commands.get;

public class OutputCommandSettings: CommandSettings
{
    [CommandOption("-b|--border")]
    [Description("table border style")]
    [TypeConverter(typeof(EnumConverter))]
    public TableBorder Border { get; set; } = TableBorder.Rounded;

    [CommandOption("-r|--raw")]
    [Description("output in plain text")]
    public bool Raw { get; set; }

    [CommandOption("-e|--expand")]
    [Description("output in expanded format: maximum width of each column (default)")]
    public bool Expand { get; set; }

    [CommandOption("-f|--footer")]
    [Description("show table footer")]
    public bool Footer { get; set; }

    public Table? CreateTable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        string? title = null, string? caption = null) where T : notnull
    {
        var table = CreateTable(title, caption);
        if(table == null)
            return null;

        var type = typeof(T);
        if(type == typeof(Job))
        {
            table.AddColumn("Title");
            table.AddColumn("Company");
            table.AddColumn("Start Date");
            table.AddColumn("End Date");
            table.AddColumn("Description");
            table.Title = new TableTitle("Jobs");
        }
        else if(type == typeof(Project))
        {
            table.AddColumn("Title");
            table.AddColumn("Type");
            table.AddColumn("Description");
            table.AddColumn("Details");
            table.AddColumn("Link");
            table.AddColumn("Start Date");
            table.AddColumn("End Date");
            table.Title = new TableTitle("Projects");
        }
        else if(type == typeof(Skill))
        {
            table.AddColumns("Name", "Type");
            table.Title = new TableTitle("Skills");
        }
        else if(type == typeof(Profile))
        {
            table.AddColumns("Name", "Email", "Phone", "Location", "Interests", "Objective", "Languages", "Website");
            table.Title = new TableTitle("Profiles");
        }
        else if(type == typeof(TypstTemplate))
        {
            table.AddColumns("Name", "Description", "Content");
            table.Title = new TableTitle("Templates");
        }
        else
        {
            typeof(T).GetProperties()
                .Where(prop => prop.CanRead)
                .Select(prop => prop.Name)
                .ToImmutableList()
                .ForEach(name => table.AddColumn(new TableColumn(name)));

            table.Title = new TableTitle($"{type.Name}s");
        }

        return table;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="title"></param>
    /// <param name="caption"></param>
    /// <returns>if null raw/plain output was requested</returns>
    public Table? CreateTable(string? title = null, string? caption = null)
    {
        if(Raw)
            return null;

        var table = new Table()
        {
            Title = string.IsNullOrWhiteSpace(title) ? null : new TableTitle($"[BOLD]{title}[/]"),
            Caption = string.IsNullOrWhiteSpace(caption) ? null : new TableTitle(caption),
            Expand = Expand,
            Border = Border,
            ShowFooters = Footer,
            ShowHeaders = true,
            UseSafeBorder = true,
            ShowRowSeparators = true,
        };
        return table;
    }
}

public class EnumConverter: TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        HashSet<TableBorder> tableBorders =
        [
            TableBorder.Ascii,
            TableBorder.Ascii2,
            TableBorder.AsciiDoubleHead,
            TableBorder.Double,
            TableBorder.DoubleEdge,
            TableBorder.Heavy,
            TableBorder.HeavyEdge,
            TableBorder.HeavyHead,
            TableBorder.Horizontal,
            TableBorder.Markdown,
            TableBorder.Minimal,
            TableBorder.MinimalDoubleHead,
            TableBorder.MinimalHeavyHead,
            TableBorder.None,
            TableBorder.Rounded,
            TableBorder.Simple,
            TableBorder.SimpleHeavy,
            TableBorder.Square,
        ];
        Console.WriteLine(TableBorder.Ascii.ToString());
        var tableBorderMap = tableBorders.Where(tb => tb.ToString() != null)
            // tostring of table border looks like "Spectre.Console.Rendering.AsciiTableBorder"; ^11 takes off the "tableborder" suffix
            .ToDictionary(key => key.ToString()!.Split(".")[^1].ToLower()[0..^11], tableBorder => tableBorder);
        return value switch
        {
            string s => tableBorderMap.ContainsKey(s.ToLower()) ? tableBorderMap[s.ToLower()] : TableBorder.Simple,
            _ => base.ConvertFrom(context, culture, value),
        };
    }
}