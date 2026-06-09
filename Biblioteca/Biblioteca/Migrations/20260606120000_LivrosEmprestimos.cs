using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Biblioteca.Migrations
{
    /// <inheritdoc />
    public partial class LivrosEmprestimos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "livros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    titulo = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    autor = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    isbn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ano_publicacao = table.Column<int>(type: "integer", nullable: false),
                    quantidade_total = table.Column<int>(type: "integer", nullable: false),
                    quantidade_disponivel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_livros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "emprestimos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    livro_id = table.Column<int>(type: "integer", nullable: false),
                    data_emprestimo = table.Column<DateOnly>(type: "date", nullable: false),
                    data_prevista_devolucao = table.Column<DateOnly>(type: "date", nullable: false),
                    data_devolucao = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emprestimos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_emprestimos_livros_livro_id",
                        column: x => x.livro_id,
                        principalTable: "livros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_emprestimos_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_emprestimos_livro_id",
                table: "emprestimos",
                column: "livro_id");

            migrationBuilder.CreateIndex(
                name: "IX_emprestimos_usuario_id",
                table: "emprestimos",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_livros_isbn",
                table: "livros",
                column: "isbn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "emprestimos");

            migrationBuilder.DropTable(
                name: "livros");
        }
    }
}
