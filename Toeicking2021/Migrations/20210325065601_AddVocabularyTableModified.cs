using Microsoft.EntityFrameworkCore.Migrations;

namespace Toeicking2021.Migrations
{
    public partial class AddVocabularyTableModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vocabulary_Sentences_SentenceId",
                table: "Vocabulary");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vocabulary",
                table: "Vocabulary");

            migrationBuilder.RenameTable(
                name: "Vocabulary",
                newName: "Vocabularies");

            migrationBuilder.RenameIndex(
                name: "IX_Vocabulary_SentenceId",
                table: "Vocabularies",
                newName: "IX_Vocabularies_SentenceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vocabularies",
                table: "Vocabularies",
                column: "VocabularyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vocabularies_Sentences_SentenceId",
                table: "Vocabularies",
                column: "SentenceId",
                principalTable: "Sentences",
                principalColumn: "SentenceId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vocabularies_Sentences_SentenceId",
                table: "Vocabularies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vocabularies",
                table: "Vocabularies");

            migrationBuilder.RenameTable(
                name: "Vocabularies",
                newName: "Vocabulary");

            migrationBuilder.RenameIndex(
                name: "IX_Vocabularies_SentenceId",
                table: "Vocabulary",
                newName: "IX_Vocabulary_SentenceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vocabulary",
                table: "Vocabulary",
                column: "VocabularyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vocabulary_Sentences_SentenceId",
                table: "Vocabulary",
                column: "SentenceId",
                principalTable: "Sentences",
                principalColumn: "SentenceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
