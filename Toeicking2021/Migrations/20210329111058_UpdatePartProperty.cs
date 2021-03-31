using Microsoft.EntityFrameworkCore.Migrations;

namespace Toeicking2021.Migrations
{
    public partial class UpdatePartProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Part",
                table: "Sentences",
                type: "nvarchar(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Part",
                table: "Sentences",
                type: "varchar(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)");
        }
    }
}
