using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace resume_builder.Migrations
{
    /// <inheritdoc />
    public partial class ChangeKeyToIdForProfileEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //because all key, column type, etc. operations are performed at the end of a shared transaction
            // so chainging this fk causes a "mismatch" because there's no way to add an index/pk on lastname before the temp table is added...
            // (don't know a fk still exists after SPECIFICALLY DROPPING IT but wtv
            migrationBuilder.Sql("PRAGMA foreign_keys=off;", true); // could done my owwn table create, insert, rearrange, spaghetti bolgnese, but this will work

            migrationBuilder.DropForeignKey(
                name: "FK_Certificate_Profiles_ProfileFirstName_ProfileLastName_ProfileEmailAddress_ProfilePhoneNumber",
                table: "Certificate");

            migrationBuilder.DropForeignKey(
                name: "FK_Education_Profiles_ProfileFirstName_ProfileLastName_ProfileEmailAddress_ProfilePhoneNumber",
                table: "Education");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Education",
                table: "Education");

            migrationBuilder.DropIndex(
                name: "IX_Education_ProfileFirstName_ProfileLastName_ProfileEmailAddress_ProfilePhoneNumber",
                table: "Education");

            migrationBuilder.DropIndex(
                name: "IX_Certificate_ProfileFirstName_ProfileLastName_ProfileEmailAddress_ProfilePhoneNumber",
                table: "Certificate");

            migrationBuilder.DropColumn(
                name: "ProfileEmailAddress",
                table: "Education");

            migrationBuilder.DropColumn(
                name: "ProfileFirstName",
                table: "Education");

            migrationBuilder.DropColumn(
                name: "ProfilePhoneNumber",
                table: "Education");

            migrationBuilder.DropColumn(
                name: "ProfileEmailAddress",
                table: "Certificate");

            migrationBuilder.DropColumn(
                name: "ProfileFirstName",
                table: "Certificate");

            migrationBuilder.DropColumn(
                name: "ProfilePhoneNumber",
                table: "Certificate");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Profiles",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Education",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles",
                column: "LastName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Education",
                table: "Education",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Education_ProfileLastName",
                table: "Education",
                column: "ProfileLastName");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_ProfileLastName",
                table: "Certificate",
                column: "ProfileLastName");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificate_Profiles_ProfileLastName",
                table: "Certificate",
                column: "ProfileLastName",
                principalTable: "Profiles",
                principalColumn: "LastName");

            migrationBuilder.AddForeignKey(
                name: "FK_Education_Profiles_ProfileLastName",
                table: "Education",
                column: "ProfileLastName",
                principalTable: "Profiles",
                principalColumn: "LastName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificate_Profiles_ProfileLastName",
                table: "Certificate");

            migrationBuilder.DropForeignKey(
                name: "FK_Education_Profiles_ProfileLastName",
                table: "Education");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Education",
                table: "Education");

            migrationBuilder.DropIndex(
                name: "IX_Education_ProfileLastName",
                table: "Education");

            migrationBuilder.DropIndex(
                name: "IX_Certificate_ProfileLastName",
                table: "Certificate");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Education");

            migrationBuilder.AddColumn<string>(
                name: "ProfileEmailAddress",
                table: "Education",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileFirstName",
                table: "Education",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhoneNumber",
                table: "Education",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileEmailAddress",
                table: "Certificate",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileFirstName",
                table: "Certificate",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhoneNumber",
                table: "Certificate",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles",
                columns: new[] { "FirstName", "LastName", "EmailAddress", "PhoneNumber" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Education",
                table: "Education",
                columns: new[] { "School", "Degree", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Education_ProfileFirstName_ProfileLastName_ProfileEmailAddress_ProfilePhoneNumber",
                table: "Education",
                columns: new[] { "ProfileFirstName", "ProfileLastName", "ProfileEmailAddress", "ProfilePhoneNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_ProfileFirstName_ProfileLastName_ProfileEmailAddress_ProfilePhoneNumber",
                table: "Certificate",
                columns: new[] { "ProfileFirstName", "ProfileLastName", "ProfileEmailAddress", "ProfilePhoneNumber" });

            migrationBuilder.AddForeignKey(
                name: "FK_Certificate_Profiles_ProfileFirstName_ProfileLastName_ProfileEmailAddress_ProfilePhoneNumber",
                table: "Certificate",
                columns: new[] { "ProfileFirstName", "ProfileLastName", "ProfileEmailAddress", "ProfilePhoneNumber" },
                principalTable: "Profiles",
                principalColumns: new[] { "FirstName", "LastName", "EmailAddress", "PhoneNumber" });

            migrationBuilder.AddForeignKey(
                name: "FK_Education_Profiles_ProfileFirstName_ProfileLastName_ProfileEmailAddress_ProfilePhoneNumber",
                table: "Education",
                columns: new[] { "ProfileFirstName", "ProfileLastName", "ProfileEmailAddress", "ProfilePhoneNumber" },
                principalTable: "Profiles",
                principalColumns: new[] { "FirstName", "LastName", "EmailAddress", "PhoneNumber" });
        }
    }
}
