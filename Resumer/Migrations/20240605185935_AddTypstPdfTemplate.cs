using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Resumer.Migrations
{
    /// <inheritdoc />
    public partial class AddTypstPdfTemplate : Migration
    {
        private const string content = $"#let dateFormat = \"[month repr:short]. [year repr:full sign:automatic] \"\n\n= #fullName\n== #phoneNumber\n== #emailAddress\n\n#location,\n\n#website\n\n#objective\n\n#if jobs != none {{\n  [== Work Experience\n\n  #for job in jobs {{\n    [=== #job.title -- #job.company]\n\n    job.startDate.display(dateFormat)\n    \" - \"\n    if job.endDate != none {{\n        job.endDate.display(dateFormat)\n      }} else {{ \"Present\" }}\n\n    linebreak()\n\n\n    for desc in job.description {{\n      [- #desc]\n    }}\n  }}\n]}}\n\n#if projects != none {{\n  [= Projects\n\n    #for proj in projects {{\n      [=== #proj.title #if proj.type != none {{[-- #proj.type]}}]\n\n      if proj.startDate != none {{ proj.startDate.display(dateFormat)\n      \" - \"\n        if proj.endDate != none {{\n          proj.endDate.display(dateFormat)\n        }} else {{ \"Present\" }}\n      }}\n\n      linebreak()\n\n      proj.description\n\n      for detail in proj.details {{\n        [- #detail]\n      }}\n    }}]\n}}\n\n#if skills != none {{\n  [= Skills\n\n  #for skill in skills {{\n    [- #skill.name]\n  }}]\n}}\n\n#if education != none {{\n  [= Education\n\n  #for edu in education {{\n    [=== #edu.school -- #edu.fieldOfStudy]\n\n    if edu.startDate != none {{ edu.startDate.display(dateFormat)\n    \" - \"\n      if edu.endDate != none {{\n        edu.endDate.display(dateFormat)\n      }} else {{ \"Present\" }}\n    }}\n\n    linebreak()\n    edu.location\n    linebreak()\n\n    edu.additionalInformation\n  }}]\n}}\n\n#if languages != none {{\n  [= Languages\n\n  #for lang in languages {{\n    [- #lang]\n  }}]\n}}\n\n#if interests != none {{\n  [= Interests\n\n  #for interest in interests {{\n    [- #interest]\n  }}]\n}}";
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Id);
                });

            migrationBuilder.InsertData(
                "Templates",
                new[] { "Id", "Content", "Name" },
            new object[] { Guid.Empty, content, "Default" }
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Templates");
        }
    }
}
