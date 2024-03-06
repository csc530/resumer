using System.ComponentModel.DataAnnotations;
using Resumer.cli.commands.export;

namespace Resumer.models;

public class Resume
{
    [Key]
    public string Name { get; set; }
    public Profile Profile { get; set; }
    public List<Job> Jobs { get; set; }
    public List<Skill> Skills { get; set; }
    public List<Project> Projects { get; set; }

    public string Export(Formats format, dynamic template)
    {
        if(!format.HasFlag(Formats.Txt))
            return "Exporting to txt format";

        return "Exporting to " + format + " format";
    }

}