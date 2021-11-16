using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TriviaDotNetApi.Infrastructure.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "TriviaDotNetApi");

            migrationBuilder.CreateTable(
                name: "TriviaItem",
                schema: "TriviaDotNetApi",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    category = table.Column<string>(nullable: true),
                    type = table.Column<string>(nullable: true),
                    difficulty = table.Column<string>(nullable: true),
                    question = table.Column<string>(nullable: true),
                    correct_answer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriviaItem", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TriviaItem",
                schema: "TriviaDotNetApi");
        }
    }
}
