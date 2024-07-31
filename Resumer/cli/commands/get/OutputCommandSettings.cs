using System.ComponentModel;
using System.Globalization;
using System.Text;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Resumer.cli.commands.get;

public abstract class OutputCommandSettings: CommandSettings
{
    [CommandOption("-o|--format")]
    [Description("output format")]
    [DefaultValue(OutputFormats.Table)]
    public OutputFormats Format { get; set; } = OutputFormats.Table;

    [CommandOption("-b|--border")]
    [Description("table border style")]
    [TypeConverter(typeof(TableBorderEnumConverter))]
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


    /// <summary>
    ///
    /// </summary>
    /// <param name="title"></param>
    /// <param name="caption"></param>
    /// <returns>if null raw/plain output was requested</returns>
    public Table? CreateTable(string? title, string? caption = null)
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

    public virtual Table? CreateTable() => throw new NotImplementedException("must be implemented in derived classes");

    public string Output(Table table)
    {
        var output = string.Empty;
        switch(Format)
        {
            case OutputFormats.Table:
                AnsiConsole.Decoration = Decoration.Conceal;
                AnsiConsole.Record();
                AnsiConsole.Write(table);
                output = AnsiConsole.ExportText();
                AnsiConsole.ResetDecoration();
                break;
            case OutputFormats.json:
                // JsonSerializer.SerializeToDocument();
                break;
            case OutputFormats.csv:
                break;
            case OutputFormats.yml:
                break;
            case OutputFormats.yaml:
                break;
            case OutputFormats.xml:
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(Format), (int)Format, typeof(OutputFormats));
        }

        return output;
    }

    public string Output<T>(IEnumerable<T> objs) where T : notnull
    {
        var sb = new StringBuilder();
        foreach(var obj in objs)
            sb.AppendLine(Output(CreateTable()));
        return sb.ToString();
    }
}

public enum OutputFormats
{
    Table,
    json,
    csv,
    yml,
    yaml,
    xml,
}

public class TableBorderEnumConverter: TypeConverter
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