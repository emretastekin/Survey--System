using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyApp.Migrations
{
    public partial class Test2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Surveys",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "YesNoAnswer",
                table: "Surveys",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "YesNoAnswer",
                table: "Surveys");
        }
    }
}
