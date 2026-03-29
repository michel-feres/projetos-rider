using CrudFornecedorProduto.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudFornecedorProduto.Data;

/*
 * ApplicationDbContext
 * Clase que representa o contexto do Entity Framework Core usado para
 * acesso ao banco de dados. Cada DbSet corresponde a uma tabela/entidade
 * no banco. Aqui definimos também configurações de relacionamento.
 */
public class ApplicationDbContext : DbContext
{
    // O construtor recebe opções (conexão, provedor, etc.) que são
    // configuradas em Program.cs e injetadas pelo container.
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSet representa a coleção de fornecedores (tabela Fornecedores)
    public DbSet<Fornecedor> Fornecedores { get; set; }
    // DbSet representa a coleção de produtos (tabela Produtos)
    public DbSet<Produto> Produtos { get; set; }

    // OnModelCreating permite configurar relacionamentos, chaves e comportamento
    // de exclusão (cascade, restrict, etc.) usando a API fluente.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuramos aqui a relação: 1 Fornecedor tem muitos Produtos.
        // - HasMany: fornecedor.Produtos
        // - WithOne: cada produto tem uma referência a Fornecedor
        // - HasForeignKey: chave estrangeira na entidade Produto
        // - OnDelete(DeleteBehavior.Cascade): ao excluir um fornecedor,
        //   seus produtos relacionados também serão removidos (cascade).
        modelBuilder.Entity<Fornecedor>()
            .HasMany(f => f.Produtos)
            .WithOne(p => p.Fornecedor)
            .HasForeignKey(p => p.FornecedorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
