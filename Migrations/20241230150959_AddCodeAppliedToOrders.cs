using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeAppliedToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Discount",
                table: "promocje",
                newName: "znizka");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "promocje",
                newName: "kod");

            migrationBuilder.AddColumn<string>(
                name: "CodeApplied",
                table: "zamowienie",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "znizka",
                table: "promocje",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeApplied",
                table: "zamowienie");

            migrationBuilder.RenameColumn(
                name: "znizka",
                table: "promocje",
                newName: "Discount");

            migrationBuilder.RenameColumn(
                name: "kod",
                table: "promocje",
                newName: "Code");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "promocje",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
