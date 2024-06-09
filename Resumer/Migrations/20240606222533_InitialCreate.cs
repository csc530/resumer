using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Resumer.models;

#nullable disable

namespace Resumer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Company = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Experience = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    MiddleName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    Interests = table.Column<string>(type: "TEXT", nullable: false),
                    Languages = table.Column<string>(type: "TEXT", nullable: false),
                    Website = table.Column<string>(type: "TEXT", nullable: true),
                    Objective = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Details = table.Column<string>(type: "TEXT", nullable: false),
                    Link = table.Column<string>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Name);
                });

            const string defaultContent = "#let dateFormat = \"[month repr:short]. [year repr:full sign:automatic]\"\n\n= #fullName\n== #phoneNumber\n== #emailAddress\n\n#location,\n\n#website\n\n#objective\n\n#if jobs != none {\n  [== Work Experience\n\n  #for job in jobs {\n    [=== #job.title -- #job.company]\n\n    job.startDate.display(dateFormat)\n    \" - \"\n    if job.endDate != none {\n        job.endDate.display(dateFormat)\n      } else { \"Present\" }\n\n    linebreak()\n\n\n    for desc in job.description {\n      [- #desc]\n    }\n  }\n]}\n\n#if projects != none {\n  [= Projects\n\n    #for proj in projects {\n      [=== #proj.title #if proj.type != none {[-- #proj.type]}]\n\n      if proj.startDate != none { proj.startDate.display(dateFormat)\n      \" - \"\n        if proj.endDate != none {\n          proj.endDate.display(dateFormat)\n        } else { \"Present\" }\n      }\n\n      linebreak()\n\n      proj.description\n\n      for detail in proj.details {\n        [- #detail]\n      }\n    }]\n}\n\n#if skills != none {\n  [= Skills\n\n  #for skill in skills {\n    [- #skill.name]\n  }]\n}\n\n#if education != none {\n  [= Education\n\n  #for edu in education {\n    [=== #edu.school -- #edu.fieldOfStudy]\n\n    if edu.startDate != none { edu.startDate.display(dateFormat)\n    \" - \"\n      if edu.endDate != none {\n        edu.endDate.display(dateFormat)\n      } else { \"Present\" }\n    }\n\n    linebreak()\n    edu.location\n    linebreak()\n\n    edu.additionalInformation\n  }]\n}\n\n#if languages != none {\n  [= Languages\n\n  #for lang in languages {\n    [- #lang]\n  }]\n}\n\n#if interests != none {\n  [= Interests\n\n  #for interest in interests {\n    [- #interest]\n  }]\n}";
            migrationBuilder.InsertData(
                "Templates",
                new[] { "Name" , "Content", "Description" },
                new object[] { "Default", defaultContent, "default template"}
            );

            migrationBuilder.CreateTable(
                name: "Certificate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Issuer = table.Column<string>(type: "TEXT", nullable: true),
                    IssueDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    ExpirationDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    CredentialId = table.Column<string>(type: "TEXT", nullable: true),
                    ProfileId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificate_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Education",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    School = table.Column<string>(type: "TEXT", nullable: false),
                    Degree = table.Column<string>(type: "TEXT", nullable: false),
                    FieldOfStudy = table.Column<string>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    GradePointAverage = table.Column<double>(type: "REAL", nullable: true),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    AdditionalInformation = table.Column<string>(type: "TEXT", nullable: true),
                    ProfileId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Education", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Education_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_ProfileId",
                table: "Certificate",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Education_ProfileId",
                table: "Education",
                column: "ProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificate");

            migrationBuilder.DropTable(
                name: "Education");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
