using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Toeicking2021.Migrations
{
    public partial class AddSenetenceTableClassModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sentences",
                columns: table => new
                {
                    SentenceId = table.Column<int>(type: "int", nullable: false),
                    Sen = table.Column<string>(type: "nvarchar(600)", nullable: false),
                    Chinesese = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Context = table.Column<string>(type: "varchar(2)", nullable: false),
                    WordOrigin = table.Column<bool>(type: "bit", nullable: true),
                    Synonym = table.Column<bool>(type: "bit", nullable: true),
                    GrammarCategory = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sentences", x => x.SentenceId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sentences");
        }
    }
}
