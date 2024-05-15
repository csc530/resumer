namespace Resumer;

public enum Formats
{
    Text = 0b100000,
    Txt = 1 + Text,
    Json = 2 + Text,
    Csv = 3 + Text,
    Html = 4 + Text,
    Md = 5 + Text,


    Binary = 0b1000000000,
    Pdf = 1 + Binary,
    Docx = 2 + Binary,
    Rtf = 3 + Binary,
    Odt = 4 + Binary,
    Pptx = 5 + Binary,
}

public static partial class Utility
{
    public static IEnumerable<string> FormatNames => Enum.GetNames<Formats>();
    public static IEnumerable<Formats> Formats => Enum.GetValues<Formats>();
    public static IEnumerable<Formats> TextFormats => Formats.Where(x => x.HasFlag(Resumer.Formats.Text));

    public static IEnumerable<string> TextFormatNames =>
        Formats.Where(x => x.HasFlag(Resumer.Formats.Txt)).Select(x => x.ToString());

    public static IEnumerable<Formats> BinaryFormats =>
        Formats.Where(x => x.HasFlag(Resumer.Formats.Binary) && x != Resumer.Formats.Binary);

    public static IEnumerable<string> BinaryFormatNames => Formats
        .Where(x => x.HasFlag(Resumer.Formats.Binary) && x != Resumer.Formats.Binary).Select(x => x.ToString());
}