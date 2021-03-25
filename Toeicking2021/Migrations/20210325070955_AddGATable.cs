using Microsoft.EntityFrameworkCore.Migrations;

namespace Toeicking2021.Migrations
{
    public partial class AddGATable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GAs",
                columns: table => new
                {
                    AnalysisId = table.Column<int>(type: "int", nullable: false),
                    SentenceId = table.Column<int>(type: "int", nullable: false),
                    Analysis = table.Column<string>(type: "nvarchar(400)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GAs", x => x.AnalysisId);
                    table.ForeignKey(
                        name: "FK_GAs_Sentences_SentenceId",
                        column: x => x.SentenceId,
                        principalTable: "Sentences",
                        principalColumn: "SentenceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GAs_SentenceId",
                table: "GAs",
                column: "SentenceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GAs");
        }
    }
}
