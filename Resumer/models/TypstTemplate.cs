using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Resumer.models;

/// <summary>
/// Typst PDF template
/// </summary>
public class TypstTemplate
{
    [Key] public string? Name { get; set; }
    public string Content { get; set; }
    public string? Description { get; set; } = string.Empty;

    private const string DefaultContent =
        "#let dateFormat = \"[month repr:short]. [year repr:full sign:automatic]\"\n\n= #fullName\n== #phoneNumber\n== #emailAddress\n\n#location,\n\n#if website != none {\n  underline(link(website))\n}\n#line(length: 100%)\n\n#objective\n\n#if jobs != none {\n  [== Work Experience\n\n    #for job in jobs {\n      [=== #job.title -- #job.company]\n\n      job.startDate.display(dateFormat)\n      \" - \"\n      if job.endDate != none {\n        job.endDate.display(dateFormat)\n      } else { \"Present\" }\n\n      linebreak()\n\n      for desc in job.description {\n        [- #desc]\n      }\n    }\n  ]\n}\n\\\n#if projects != none {\n  [= Projects\n\n    #for proj in projects {\n      link(\n        proj.link.absoluteUri,\n      )[=== #proj.title #if proj.type != none { [-- #proj.type] }]\n      if proj.link.absoluteUri != none {\n        underline(link(proj.link.absoluteUri))\n        linebreak()\n      }\n\n      if proj.startDate != none {\n        proj.startDate.display(dateFormat)\n        \" - \"\n        if proj.endDate != none {\n          proj.endDate.display(dateFormat)\n        } else { \"Present\" }\n        linebreak()\n      }\n\n      proj.description\n\n      for detail in proj.details {\n        [- #detail]\n      }\n    }]\n}\n\n#if skills != none {\n  [= Skills\n\n    #box(height: 15%, width: 100%, outset: 25%)[\n      #columns(calc.ceil(skills.len() / 8))[\n        #for skill in skills {\n          [- #skill.name]\n        }\n      ]\n    ]\n  ]\n}\n\n#if education != none {\n  [= Education\n\n    #for edu in education {\n      [=== #edu.school -- #edu.fieldOfStudy]\n\n      edu.degree\n      linebreak()\n\n      if edu.startDate != none {\n        edu.startDate.display(dateFormat)\n        \" - \"\n        if edu.endDate != none {\n          edu.endDate.display(dateFormat)\n        } else { \"Present\" }\n      }\n\n      linebreak()\n      edu.location\n      linebreak()\n\n      edu.additionalInformation\n    }]\n}\n\n#if languages != none {\n  [= Languages\n\n    #for lang in languages {\n      [- #lang]\n    }]\n}";
        
    public static TypstTemplate Default => new("default", DefaultContent) { Description = "Default template" };

    public TypstTemplate(string? name, string content)
    {
        Name = name;
        Content = content;
    }

    /// <inheritdoc />
    public override string? ToString() => string.IsNullOrWhiteSpace(Description) ? Name : $"{Name} - {Description}";

    /// <summary>
    /// Test if the template is a valid typst file
    /// </summary>
    /// <returns>bool true if valid</returns>
    public bool IsValid(out string error, out string output)
    {
        var tempFile = Path.GetRandomFileName();
        var testTyp = Resume.ExampleResume().ExportToTypst(this).Trim();
        var typst = new Command("typst")
        {
            WorkingDirectory = Program.TempPath,
            RedirectStandardInput = true,
            CommandDisplay = CommandDisplay.Hidden,
        };
        var errorBuilder = new StringBuilder();
        var outputBuilder = new StringBuilder();
        typst.OnStandardError += (s, e) => errorBuilder.AppendLine(e.Data);
        typst.OnStandardOutput += (s, e) => outputBuilder.AppendLine(e.Data);
        var result = typst.Start("compile", "-", tempFile, "--format=pdf")
            .Input(testTyp)
            .Complete();

        error = errorBuilder.ToString();
        output = outputBuilder.ToString();

        if(result.ExitCode != null && result.ExitCode != 0)
            return false;
        File.Delete(Path.Combine(Program.TempPath, tempFile));
        return true;
    }
}