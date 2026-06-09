using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Data;

/// <summary>
/// Contexto principal do Entity Framework Core para acesso ao PostgreSQL.
/// </summary>
public class BibliotecaContext : DbContext
{
    public BibliotecaContext(DbContextOptions<BibliotecaContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Livro> Livros => Set<Livro>();
    public DbSet<Emprestimo> Emprestimos => Set<Emprestimo>();

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

        modelBuilder.Entity<Livro>(entity =>
        {
            entity.ToTable("livros");
            entity.HasKey(livro => livro.Id);
            entity.HasIndex(livro => livro.Isbn).IsUnique();

            entity.Property(livro => livro.Titulo)
                .HasColumnName("titulo")
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(livro => livro.Autor)
                .HasColumnName("autor")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(livro => livro.Isbn)
                .HasColumnName("isbn")
                .HasMaxLength(20);

            entity.Property(livro => livro.Categoria)
                .HasColumnName("categoria")
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(livro => livro.FaixaEtariaPermitida)
                .HasColumnName("faixa_etaria_permitida")
                .IsRequired();

            entity.Property(livro => livro.AnoPublicacao)
                .HasColumnName("ano_publicacao")
                .IsRequired();

            entity.Property(livro => livro.QuantidadeTotal)
                .HasColumnName("quantidade_total")
                .IsRequired();

            entity.Property(livro => livro.QuantidadeDisponivel)
                .HasColumnName("quantidade_disponivel")
                .IsRequired();
        });

        modelBuilder.Entity<Emprestimo>(entity =>
        {
            entity.ToTable("emprestimos");
            entity.HasKey(emprestimo => emprestimo.Id);

            entity.Property(emprestimo => emprestimo.DataEmprestimo)
                .HasColumnName("data_emprestimo")
                .IsRequired();

            entity.Property(emprestimo => emprestimo.UsuarioId)
                .HasColumnName("usuario_id")
                .IsRequired();

            entity.Property(emprestimo => emprestimo.LivroId)
                .HasColumnName("livro_id")
                .IsRequired();

            entity.Property(emprestimo => emprestimo.DataPrevistaDevolucao)
                .HasColumnName("data_prevista_devolucao")
                .IsRequired();

            entity.Property(emprestimo => emprestimo.DataDevolucao)
                .HasColumnName("data_devolucao");

            entity.Property(emprestimo => emprestimo.Multa)
                .HasColumnName("multa")
                .HasPrecision(10, 2)
                .IsRequired();

            entity.HasOne(emprestimo => emprestimo.Usuario)
                .WithMany(usuario => usuario.Emprestimos)
                .HasForeignKey(emprestimo => emprestimo.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(emprestimo => emprestimo.Livro)
                .WithMany(livro => livro.Emprestimos)
                .HasForeignKey(emprestimo => emprestimo.LivroId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
