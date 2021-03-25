using Microsoft.EntityFrameworkCore.Migrations;

namespace Toeicking2021.Migrations
{
    public partial class AddVocabularyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vocabulary",
                columns: table => new
                {
                    VocabularyId = table.Column<int>(type: "int", nullable: false),
                    SentenceId = table.Column<int>(type: "int", nullable: false),
                    Voc = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Category = table.Column<string>(type: "varchar(4)", nullable: true),
                    Chinese = table.Column<string>(type: "nvarchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vocabulary", x => x.VocabularyId);
                    table.ForeignKey(
                        name: "FK_Vocabulary_Sentences_SentenceId",
                        column: x => x.SentenceId,
                        principalTable: "Sentences",
                        principalColumn: "SentenceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vocabulary_SentenceId",
                table: "Vocabulary",
                column: "SentenceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vocabulary");
        }
    }
}
