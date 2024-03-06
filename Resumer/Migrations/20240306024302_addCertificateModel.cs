using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace resume_builder.Migrations
{
    /// <inheritdoc />
    public partial class addCertificateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Certifications",
                table: "Profiles");

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
                    ProfileEmailAddress = table.Column<string>(type: "TEXT", nullable: true),
                    ProfileFirstName = table.Column<string>(type: "TEXT", nullable: true),
                    ProfileLastName = table.Column<string>(type: "TEXT", nullable: true),
                    ProfilePhoneNumber = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificate_Profiles_ProfileFirstName_ProfileLastName_ProfileEmailAddress_ProfilePhoneNumber",
                        columns: x => new { x.ProfileFirstName, x.ProfileLastName, x.ProfileEmailAddress, x.ProfilePhoneNumber },
                        principalTable: "Profiles",
                        principalColumns: new[] { "FirstName", "LastName", "EmailAddress", "PhoneNumber" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_ProfileFirstName_ProfileLastName_ProfileEmailAddress_ProfilePhoneNumber",
                table: "Certificate",
                columns: new[] { "ProfileFirstName", "ProfileLastName", "ProfileEmailAddress", "ProfilePhoneNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificate");

            migrationBuilder.AddColumn<string>(
                name: "Certifications",
                table: "Profiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
