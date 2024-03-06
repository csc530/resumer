using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace resume_builder.Migrations
{
    /// <inheritdoc />
    public partial class addResumeModelAndUpdateProfileProjectModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "Profiles",
                newName: "Objective");

            migrationBuilder.AddColumn<string>(
                name: "Certifications",
                table: "Profiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Interests",
                table: "Profiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                table: "Profiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Profiles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs",
                columns: new[] { "Title", "Company", "StartDate" });

            migrationBuilder.CreateTable(
                name: "Education",
                columns: table => new
                {
                    School = table.Column<string>(type: "TEXT", nullable: false),
                    Degree = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    FieldOfStudy = table.Column<string>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    GradePointAverage = table.Column<float>(type: "REAL", nullable: true),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    AdditionalInformation = table.Column<string>(type: "TEXT", nullable: true),
                    ProfileEmailAddress = table.Column<string>(type: "TEXT", nullable: true),
                    ProfileFirstName = table.Column<string>(type: "TEXT", nullable: true),
                    ProfileLastName = table.Column<string>(type: "TEXT", nullable: true),
                    ProfilePhoneNumber = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Education", x => new { x.School, x.Degree, x.StartDate });
                    table.ForeignKey(
                        name: "FK_Education_Profiles_ProfileFirstName_ProfileLastName_ProfileEmailAddress_ProfilePhoneNumber",
                        columns: x => new { x.ProfileFirstName, x.ProfileLastName, x.ProfileEmailAddress, x.ProfilePhoneNumber },
                        principalTable: "Profiles",
                        principalColumns: new[] { "FirstName", "LastName", "EmailAddress", "PhoneNumber" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Education_ProfileFirstName_ProfileLastName_ProfileEmailAddress_ProfilePhoneNumber",
                table: "Education",
                columns: new[] { "ProfileFirstName", "ProfileLastName", "ProfileEmailAddress", "ProfilePhoneNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Education");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Certifications",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Interests",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Languages",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Profiles");

            migrationBuilder.RenameColumn(
                name: "Objective",
                table: "Profiles",
                newName: "Summary");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Jobs",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs",
                column: "Id");
        }
    }
}
