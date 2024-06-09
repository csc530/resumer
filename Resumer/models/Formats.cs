namespace Resumer.models
{
    public enum Formats
    {
        /// <summary>Text file formats</summary>
        Text = 0b100000,
        Txt = 1 + Text,
        Json = 2 + Text,

        // Csv = 3 + Text,
        // Html = 4 + Text,
        Md = 5 + Text,

        /// <summary>Typst file format</summary>
        Typ = 6 + Text,
        Svg = 7 + Text,

        /// <summary>Binary file formats</summary>
        Binary = 0b1000000000,
        Pdf = 1 + Binary,
        Png = 2 + Binary,
        // Docx = 2 + Binary,
        // Rtf = 3 + Binary,
        // Odt = 4 + Binary,
        // Pptx = 5 + Binary,
    }
}

namespace Resumer
{
    using models;

    public static partial class Utility
    {
        public static IEnumerable<string> FormatNames => Enum.GetNames<Formats>();
        public static IEnumerable<Formats> Formats => Enum.GetValues<Formats>();

        public static IEnumerable<Formats> TextFormats =>
            Formats.Where(x => x.HasFlag(models.Formats.Text) && x != models.Formats.Text);

        public static IEnumerable<string> TextFormatNames => TextFormats.Select(x => x.ToString());

        public static IEnumerable<Formats> BinaryFormats =>
            Formats.Where(x => x.HasFlag(models.Formats.Binary) && x != models.Formats.Binary);

        public static IEnumerable<string> BinaryFormatNames => BinaryFormats.Select(x => x.ToString());
    }
}