using System.Text;

namespace Resumer.models;

/// <summary>
/// Typst PDF template
/// </summary>
public class TypstTemplate
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Content { get; set; }
    public string Description { get; set; } = string.Empty;

    public TypstTemplate(string name, string content)
    {
        Name = name;
        Content = content;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var builder = new StringBuilder(100);
        builder.Append(Name);
        if(!string.IsNullOrWhiteSpace(Description))
            builder.Append(" - ").Append(Description);
        return builder.Length > 100 ? $"{builder.ToString(0, 100)}..." : builder.ToString();
    }
}