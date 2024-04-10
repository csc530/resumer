using System.ComponentModel.DataAnnotations;

namespace Resumer.models;

public class Resume
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime DateCreated { get; set; }

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