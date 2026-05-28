using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Data;

/// <summary>
/// Contexto principal do Entity Framework Core para acesso ao PostgreSQL.
/// As próximas entidades do projeto (Livro e Empréstimo) serão adicionadas aqui.
/// </summary>
public class BibliotecaContext : DbContext
{
    public BibliotecaContext(DbContextOptions<BibliotecaContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(usuario => usuario.Id);
            entity.HasIndex(usuario => usuario.Email).IsUnique();

            entity.Property(usuario => usuario.NomeCompleto)
                .HasColumnName("nome_completo")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(usuario => usuario.DataNascimento)
                .HasColumnName("data_nascimento")
                .IsRequired();

            entity.Property(usuario => usuario.Email)
                .HasColumnName("email")
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(usuario => usuario.Senha)
                .HasColumnName("senha")
                .HasMaxLength(60)
                .IsRequired();

            entity.Property(usuario => usuario.Ativo)
                .HasColumnName("ativo")
                .HasDefaultValue(true)
                .IsRequired();
        });
    }
}