using Microsoft.EntityFrameworkCore.Migrations;

namespace Toeicking2021.Migrations
{
    public partial class AddCheckedTimesColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "CheckedTimes",
                table: "Sentences",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckedTimes",
                table: "Sentences");
        }
    }
}
