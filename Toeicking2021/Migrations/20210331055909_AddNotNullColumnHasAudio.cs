using Microsoft.EntityFrameworkCore.Migrations;

namespace Toeicking2021.Migrations
{
    public partial class AddNotNullColumnHasAudio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAudio",
                table: "Sentences",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAudio",
                table: "Sentences");
        }
    }
}
