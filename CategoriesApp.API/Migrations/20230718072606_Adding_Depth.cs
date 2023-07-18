using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CategoriesApp.API.Migrations
{
    public partial class Adding_Depth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HierarchySize",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HierarchySize",
                table: "Categories");
        }
    }
}
