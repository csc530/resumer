using System.Collections.Immutable;

namespace Resumer;

public enum Formats
{
    Txt = 0b100000,
    Json = 1 + Txt,
    Csv = 2 + Txt,
    Html = 3 + Txt,


    Binary = 0b1000000000,
    Pdf = +1 + Binary,
    Docx = 2 + Binary,
    Rtf = 3 + Binary,
    Odt = 4 + Binary,
    Pptx = 5 + Binary,
}

public static partial class Globals
{
    public static IEnumerable<string> FormatNames => Enum.GetNames<Formats>();
    public static IEnumerable<Formats> Formats => Enum.GetValues<Formats>();
    public static IEnumerable<Formats> TextFormats => Formats.Where(x => x.HasFlag(Resumer.Formats.Txt));
    public static IEnumerable<string> TextFormatNames => Formats.Where(x => x.HasFlag(Resumer.Formats.Txt)).Select(x=>x.ToString());

    public static IEnumerable<Formats> BinaryFormats => Formats.Where(x => x.HasFlag(Resumer.Formats.Binary) && x != Resumer.Formats.Binary);
    public static IEnumerable<string> BinaryFormatNames => Formats.Where(x => x.HasFlag(Resumer.Formats.Binary)&&x!=Resumer.Formats.Binary).Select(x=>x.ToString())!;
}