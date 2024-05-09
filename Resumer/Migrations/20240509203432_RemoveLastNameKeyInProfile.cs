using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace resume_builder.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLastNameKeyInProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //more again of the same thing as before becuase the parent primary key is changing
            // the "cascade" down needs to happen in the end but the temp fk doesn't refer tot the changed column name in the generaated sql
            migrationBuilder.Sql("PRAGMA foreign_keys=off;", true);

            migrationBuilder.DropForeignKey(
                name: "FK_Certificate_Profiles_ProfileLastName",
                table: "Certificate");

            migrationBuilder.DropForeignKey(
                name: "FK_Education_Profiles_ProfileLastName",
                table: "Education");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles");

            migrationBuilder.RenameColumn(
                name: "ProfileLastName",
                table: "Education",
                newName: "ProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Education_ProfileLastName",
                table: "Education",
                newName: "IX_Education_ProfileId");

            migrationBuilder.RenameColumn(
                name: "ProfileLastName",
                table: "Certificate",
                newName: "ProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Certificate_ProfileLastName",
                table: "Certificate",
                newName: "IX_Certificate_ProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificate_Profiles_ProfileId",
                table: "Certificate",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Education_Profiles_ProfileId",
                table: "Education",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificate_Profiles_ProfileId",
                table: "Certificate");

            migrationBuilder.DropForeignKey(
                name: "FK_Education_Profiles_ProfileId",
                table: "Education");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles");

            migrationBuilder.RenameColumn(
                name: "ProfileId",
                table: "Education",
                newName: "ProfileLastName");

            migrationBuilder.RenameIndex(
                name: "IX_Education_ProfileId",
                table: "Education",
                newName: "IX_Education_ProfileLastName");

            migrationBuilder.RenameColumn(
                name: "ProfileId",
                table: "Certificate",
                newName: "ProfileLastName");

            migrationBuilder.RenameIndex(
                name: "IX_Certificate_ProfileId",
                table: "Certificate",
                newName: "IX_Certificate_ProfileLastName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles",
                column: "LastName");

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
    }
}
