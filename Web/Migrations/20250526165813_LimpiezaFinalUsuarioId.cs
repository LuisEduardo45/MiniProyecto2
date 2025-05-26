using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcTemplate.Migrations
{
    /// <inheritdoc />
    public partial class LimpiezaFinalUsuarioId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TopeMaximo",
                table: "Categorias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TopeMaximo",
                table: "Categorias",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
