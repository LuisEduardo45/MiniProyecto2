using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcTemplate.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCategoriaIdFromEntradas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entradas_Categorias_CategoriaId",
                table: "Entradas");

            migrationBuilder.DropIndex(
                name: "IX_Entradas_CategoriaId",
                table: "Entradas");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Entradas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Entradas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_CategoriaId",
                table: "Entradas",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entradas_Categorias_CategoriaId",
                table: "Entradas",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
