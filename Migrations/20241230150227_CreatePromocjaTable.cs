using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class CreatePromocjaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "zamowienie",
                newName: "uzytkownik_id");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "zamowienie",
                newName: "cena");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "zamowienie",
                newName: "opis");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "zamowienie",
                newName: "data");

            migrationBuilder.RenameColumn(
                name: "Approved",
                table: "zamowienie",
                newName: "zatwierdzone");

            migrationBuilder.CreateTable(
                name: "promocje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Discount = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promocje", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "promocje");

            migrationBuilder.RenameColumn(
                name: "zatwierdzone",
                table: "zamowienie",
                newName: "Approved");

            migrationBuilder.RenameColumn(
                name: "uzytkownik_id",
                table: "zamowienie",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "opis",
                table: "zamowienie",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "data",
                table: "zamowienie",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "cena",
                table: "zamowienie",
                newName: "TotalPrice");
        }
    }
}
