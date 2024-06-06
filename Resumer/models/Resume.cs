using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Resumer.models;

public class Resume
{
    private string _name;
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name
    {
        get => _name;
        [MemberNotNull(nameof(_name))] set => _name = value.Trim();
    }

    /// <summary>
    /// The date and time the resume was created
    /// </summary>
    public DateTime DateCreated { get; init; } = DateTime.Now;

    public required Profile Profile { get; set; }
    public List<Job> Jobs { get; set; } = [];
    public List<Skill> Skills { get; set; } = [];
    public List<Project> Projects { get; set; } = [];

    public Resume(string name)
    {
        Name = name;
    }

    public static Resume ExampleResume()
    {
        var faker = new Faker();

        var profileFaker = new Faker<models.Profile>()
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName())
            .RuleFor(p => p.EmailAddress, (f, p) => f.Internet.Email(p.FirstName, p.LastName))
            .RuleFor(p => p.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(p => p.Location, f => f.Address.FullAddress())
            .RuleFor(p => p.Objective, f => f.WaffleText(includeHeading: false))
            .RuleFor(p => p.Languages, f => f.Make(f.Random.Int(1, 10), () => RandomLanguage()))
            .RuleFor(p => p.Interests, f => f.Make(f.Random.Int(1, 10), () => f.Random.Words()))
            .RuleFor(p => p.Certifications, f => f.Make(f.Random.Int(1, 10),
                () => new Certificate()
                {
                    Name = f.WaffleTitle(),
                    Description = f.WaffleText(includeHeading: false),
                    CredentialId = f.Random.Bool() ? f.Random.Guid().ToString() : f.Random.Int().ToString("G9"),
                    IssueDate = f.Date.PastDateOnly(f.Random.Number(10)),
                    ExpirationDate = f.Date.FutureDateOnly(f.Random.Number(10)),
                    Issuer = f.Company.CompanyName(),
                    Url = new Uri(f.Internet.Url()),
                })
            )
            .RuleFor(p => p.Education, f => f.Make(f.Random.Int(1, 10), () => new Education()
            {
                School = f.Company.CompanyName() + (f.Random.Bool() ? " University" : " College"),
                Degree = f.PickRandom("Associate", "Bachelor", "Master", "Doctorate"),
                StartDate = f.Date.PastDateOnly(f.Random.Number(10)),
                EndDate = f.Random.Bool() ? null : f.Date.FutureDateOnly(f.Random.Number(10)),
                FieldOfStudy = f.WaffleTitle(),
                GradePointAverage = f.Random.Double(0d, 4d),
                Location = f.Address.City() + ", " + f.Address.Country(),
                AdditionalInformation = f.Random.Bool() ? f.WaffleText(includeHeading: false) : null,
            }));

        var jobFaker = new Faker<Job>()
            .CustomInstantiator(f => new Job(f.Name.JobTitle(), f.Company.CompanyName(f.Random.Number(0, 2).OrNull(f))))
            .RuleFor(j => j.StartDate, f => f.Date.PastDateOnly(f.Random.Number(10)))
            .RuleFor(j => j.EndDate, f => f.Random.Bool() ? null : f.Date.FutureDateOnly(f.Random.Number(10)))
            .RuleFor(j => j.Description, f => f.Make(f.Random.Int(1, 5), () => f.WaffleText(includeHeading: false)));

        var projectFaker = new Faker<Project>()
            .CustomInstantiator(f => new Project(f.WaffleTitle()))
            .RuleFor(p => p.StartDate, f => f.Date.PastDateOnly(f.Random.Number(10)))
            .RuleFor(p => p.EndDate, f => f.Random.Bool() ? null : f.Date.FutureDateOnly(f.Random.Number(10)))
            .RuleFor(p => p.Type, f => f.WaffleTitle())
            .RuleFor(p => p.Description, f => f.WaffleText(includeHeading: false))
            .RuleFor(p => p.Details, f => f.Make(f.Random.Int(1, 3), () => f.WaffleText(includeHeading: false)))
            .RuleFor(p => p.Link, f => new Uri(f.Internet.Url()));

        var skillFaker = new Faker<Skill>().CustomInstantiator(f =>
            new Skill(f.Random.Bool() ? f.Name.JobArea() : f.WaffleTitle(), f.PickRandom<SkillType>()));

        var resume = new Resume(@"test")
        {
            Profile = profileFaker.Generate(),
            Jobs = jobFaker.Generate(3),
            Skills = skillFaker.Generate(faker.Random.Int(1, 20)),
            Projects = projectFaker.Generate(3),
        };

        return resume;

        string RandomLanguage() => faker.PickRandom(CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Select(c => c.DisplayName.Split("[")[0].Trim()));
    }

    /// <summary>
    /// Export resume to text format
    /// </summary>
    /// <returns>returns resume as text</returns>
    public string ExportToTxt()
    {
        const string sectionBreak = "==================================================";
        var sb = new StringBuilder();
        sb.AppendLine(Profile.FullName);
        if(Profile.Location != null)
            sb.AppendLine(Profile.Location);
        sb.AppendLine(Profile.EmailAddress);
        sb.AppendLine(Profile.PhoneNumber);
        sb.AppendLine();

        if(Profile.Objective != null)
            sb.AppendLine("PROFESSIONAL SUMMARY")
                .AppendLine(sectionBreak)
                .AppendLine(Profile.Objective);

        sb.AppendLine("WORK EXPERIENCE")
            .AppendLine(sectionBreak);
        foreach(var job in Jobs.OrderByDescending(j => j.StartDate))
        {
            sb.AppendLine(job.Title)
                .AppendLine(job.Company);
            // if(job.Location != null)
            //     sb.AppendLine($" - {job.Location}");
            sb.AppendLine(Utility.PrintDuration(job.StartDate, job.EndDate));
            foreach(var description in job.Description)
                sb.AppendLine($"+ {description}");
            sb.AppendLine();
        }

        sb.AppendLine("SKILLS")
            .AppendLine(sectionBreak);
        foreach(var skill in Skills.OrderBy(skill => skill.Name))
            sb.AppendLine(skill.Name);

        sb.AppendLine("PROJECTS")
            .AppendLine(sectionBreak);
        foreach(var project in Projects)
            sb.AppendLine(project.Title)
                .AppendLine(Utility.PrintDuration(project.StartDate, project.EndDate))
                .AppendLine(project.Description);

        sb.AppendLine("REFERENCES")
            .AppendLine(sectionBreak)
            .AppendLine("Available upon request");

        return sb.ToString();
    }

    public string ExportToCsv()
    {
        var sb = new StringBuilder();
        throw new NotImplementedException("Export to CSV not implemented yet🤷🏿‍♂️");
    }

    public string ExportToJson()
    {
        // https://jsonresume.org/schema
        var obj = new JsonResume(Profile, Jobs, Skills, Projects);
        var typeInfo = SourceGenerationContext.Default.JsonResume;

        return JsonSerializer.Serialize(obj, typeInfo);
    }

    public string? ExportToMarkdown()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# {Profile.FullName}");
        sb.AppendLine(Profile.EmailAddress);
        sb.AppendLine(Profile.PhoneNumber);
        if(Profile.Location != null)
            sb.AppendLine(Profile.Location);
        sb.AppendLine();

        if(Profile.Objective != null)
        {
            sb.AppendLine("## Professional Summary")
                .AppendLine()
                .AppendLine(Profile.Objective)
                .AppendLine();
        }

        if(Jobs.Count != 0)
        {
            sb.AppendLine("## Work Experience")
                .AppendLine();
            foreach(var job in Jobs.OrderByDescending(j => j.StartDate))
            {
                sb.AppendLine($"### {job.Title}")
                    .AppendLine()
                    .AppendLine(job.Company);
                sb.AppendLine(Utility.PrintDuration(job.StartDate, job.EndDate));
                foreach(var description in job.Description)
                    sb.AppendLine($"- {description}");
                sb.AppendLine();
            }
        }

        if(Skills.Count != 0)
        {
            sb.AppendLine("## Skills")
                .AppendLine();
            foreach(var skill in Skills.OrderBy(skill => skill.Name))
                sb.AppendLine($"- {skill.Name}");
            sb.AppendLine();
        }

        if(Projects.Count != 0)
        {
            sb.AppendLine("## Projects")
                .AppendLine();
            foreach(var project in Projects)
                sb.AppendLine($"### {project.Title}")
                    .AppendLine()
                    .AppendLine(Utility.PrintDuration(project.StartDate, project.EndDate))
                    .AppendLine(project.Description)
                    .AppendLine();
        }

        return sb.ToString();
    }
}

[JsonSerializable(typeof(JsonResume))]
internal partial class SourceGenerationContext: JsonSerializerContext { }

internal class JsonResume
{
    public JsonBasics basics { get; set; }
    public List<JsonWork> work { get; set; } = [];
    public List<JsonVolunteer> volunteer { get; set; } = [];
    public List<JsonEducation> education { get; set; } = [];
    public List<JsonAwards> awards { get; set; } = [];
    public List<JsonCertificates> certificates { get; set; } = [];
    public List<JsonPublications> publications { get; set; } = [];
    public List<JsonSkills> skills { get; set; } = [];
    public List<JsonLanguages> languages { get; set; } = [];
    public List<JsonInterests> interests { get; set; } = [];
    public List<JsonReferences> references { get; set; } = [];
    public List<JsonProjects> projects { get; set; } = [];

    private const string DateFormat = "yyyy-MM-dd";

    public JsonResume(Profile profile, IEnumerable<Job> jobs, IEnumerable<Skill> skills,
        IEnumerable<Project> projects)
    {
        basics = new JsonBasics
        {
            name = profile.FullName,
            email = profile.EmailAddress,
            phone = profile.PhoneNumber,
            summary = profile.Objective,
            location = new JsonBasics.JsonLocation { address = profile.Location },
        };

        work = jobs.Select(job => new JsonWork
        {
            name = job.Company,
            position = job.Title,
            startDate = job.StartDate.ToString(DateFormat),
            endDate = job.EndDate?.ToString(DateFormat),
            highlights = job.Description.ToList<string?>(),
        }).ToList();

        this.skills = skills.Select(skill => new JsonSkills { name = skill.Name, }).ToList();

        this.projects = projects.Select(project => new JsonProjects
        {
            name = project.Title,
            startDate = project.StartDate?.ToString(DateFormat),
            endDate = project.EndDate?.ToString(DateFormat),
            description = project.Description,
            highlights = project.Details.ToList<string?>(),
            url = project.Link?.ToString(),
        }).ToList();

        interests = profile.Interests.Select(interest => new JsonInterests { name = interest }).ToList();

        languages = profile.Languages.Select(lang => new JsonLanguages { language = lang }).ToList();

        education = profile.Education.Select(edu => new JsonEducation
        {
            institution = edu.School,
            area = edu.FieldOfStudy,
            studyType = edu.Degree,
            startDate = edu.StartDate.ToString(DateFormat),
            endDate = edu.EndDate?.ToString(DateFormat),
            score = edu.GradePointAverage.ToString(),
        }).ToList();

        certificates = profile.Certifications.Select(cert => new JsonCertificates
        {
            name = cert.Name,
            date = cert.IssueDate?.ToString(DateFormat),
            issuer = cert.Issuer,
            url = cert.Url?.ToString(),
        }).ToList();
    }

    public class JsonBasics
    {
        /// <summary>
        /// full name
        /// </summary>
        public string? name { get; set; }

        /// <summary>
        /// (job) title
        /// </summary>
        public string? label { get; set; }

        public string? image { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public JsonLocation? location { get; set; }
        public string? summary { get; set; }
        public List<JsonProfiles>? profiles { get; set; } = [];

        public class JsonLocation
        {
            public string? address { get; set; }
            public string? postalCode { get; set; }
            public string? city { get; set; }
            public string? countryCode { get; set; }
            public string? region { get; set; }
        }

        public class JsonProfiles
        {
            /// <summary>
            /// profile network/host name
            /// </summary>
            /// <example>Twitter</example>
            public string? network { get; set; }

            public string? username { get; set; }
            public string? url { get; set; }
        }
    }

    public class JsonWork
    {
        public string? name { get; set; }
        public string? position { get; set; }
        public string? url { get; set; }
        public string? startDate { get; set; }
        public string? endDate { get; set; }
        public List<string?>? highlights { get; set; }
    }

    public class JsonVolunteer
    {
        public string? organization { get; set; }
        public string? position { get; set; }
        public string? url { get; set; }
        public string? startDate { get; set; }
        public string? endDate { get; set; }
        public string? summary { get; set; }
        public List<string?>? highlights { get; set; }
    }

    public class JsonEducation
    {
        public string? institution { get; set; }
        public string? url { get; set; }

        /// <summary>
        /// area/field of study
        /// </summary>
        public string? area { get; set; }

        /// <summary>
        /// what level of study was it
        /// </summary>
        /// <example>Master's, high school</example>
        public string? studyType { get; set; }

        public string? startDate { get; set; }
        public string? endDate { get; set; }

        /// <summary>
        /// grade point average or equivalent
        /// </summary>
        public string? score { get; set; }

        public List<string?>? courses { get; set; }
    }

    public class JsonAwards
    {
        /// <summary>
        /// award name
        /// </summary>
        public string? title { get; set; }

        public string? date { get; set; }
        public string? awarder { get; set; }
        public string? summary { get; set; }
    }

    public class JsonCertificates
    {
        public string? name { get; set; }
        public string? date { get; set; }
        public string? issuer { get; set; }
        public string? url { get; set; }
    }

    public class JsonPublications
    {
        public string? name { get; set; }
        public string? publisher { get; set; }
        public string? releaseDate { get; set; }
        public string? url { get; set; }
        public string? summary { get; set; }
    }

    public class JsonSkills
    {
        public string? name { get; set; }
        public string? level { get; set; }
        public List<string?>? keywords { get; set; }
    }

    public class JsonLanguages
    {
        public string? language { get; set; }
        public string? fluency { get; set; }
    }

    public class JsonInterests
    {
        public string? name { get; set; }
        public List<string?>? keywords { get; set; }
    }

    public class JsonReferences
    {
        public string? name { get; set; }
        public string? reference { get; set; }
    }

    public class JsonProjects
    {
        public string? name { get; set; }
        public string? startDate { get; set; }
        public string? endDate { get; set; }
        public string? description { get; set; }
        public List<string?>? highlights { get; set; }
        public string? url { get; set; }
    }
}