using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserIdColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Koszyk_Uzytkownik_Userid",
                table: "Koszyk");

            migrationBuilder.DropIndex(
                name: "IX_Koszyk_Userid",
                table: "Koszyk");

            migrationBuilder.DropColumn(
                name: "Userid",
                table: "Koszyk");

            migrationBuilder.RenameColumn(
                name: "User_id",
                table: "Koszyk",
                newName: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Koszyk",
                newName: "User_id");

            migrationBuilder.AddColumn<int>(
                name: "Userid",
                table: "Koszyk",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Koszyk_Userid",
                table: "Koszyk",
                column: "Userid");

            migrationBuilder.AddForeignKey(
                name: "FK_Koszyk_Uzytkownik_Userid",
                table: "Koszyk",
                column: "Userid",
                principalTable: "Uzytkownik",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
