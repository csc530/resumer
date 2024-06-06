using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Resumer.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToTypstTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Templates",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Templates");
        }
    }
}
