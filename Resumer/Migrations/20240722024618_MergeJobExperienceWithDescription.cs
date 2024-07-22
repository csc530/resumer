using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Resumer.Migrations
{
    /// <inheritdoc />
    public partial class MergeJobExperienceWithDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(
                "UPDATE Jobs SET Description = " +
                // instr and sunstr are 1-indexed inclusive
                "CASE  WHEN Description != '[]' THEN substr(Description, 1, instr(Description, '\"]')) || ',' || substr(Experience,instr(Experience,'[\"') + 1) "+
                 "ELSE Experience END " +
                 "WHERE Experience != '[]'"
                );

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "Jobs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "Jobs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
