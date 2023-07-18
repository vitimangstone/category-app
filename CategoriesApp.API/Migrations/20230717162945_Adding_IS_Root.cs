using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CategoriesApp.API.Migrations
{
    public partial class Adding_IS_Root : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRoot",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRoot",
                table: "Categories");
        }
    }
}
