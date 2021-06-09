using Microsoft.EntityFrameworkCore.Migrations;

namespace Toeicking2021.Migrations
{
    public partial class ModifyUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "rating",
                table: "Users",
                type: "varchar(2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rating",
                table: "Users");

            migrationBuilder.AddColumn<byte>(
                name: "Level",
                table: "Users",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
