using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Migrations
{
    /// <inheritdoc />
    public partial class RegrasRelatoriosBonus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "categoria",
                table: "livros",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "Geral");

            migrationBuilder.AddColumn<int>(
                name: "faixa_etaria_permitida",
                table: "livros",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "multa",
                table: "emprestimos",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "categoria",
                table: "livros");

            migrationBuilder.DropColumn(
                name: "faixa_etaria_permitida",
                table: "livros");

            migrationBuilder.DropColumn(
                name: "multa",
                table: "emprestimos");
        }
    }
}
