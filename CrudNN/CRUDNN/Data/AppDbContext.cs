namespace CRUDNN.Data;
using CRUDNN.Models;
using Microsoft.EntityFrameworkCore;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<VendaProduto> VendasProdutos => Set<VendaProduto>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Chave composta da tabela de junção
        modelBuilder.Entity<VendaProduto>()
            .HasKey(vp => new { vp.VendaId, vp.ProdutoId });
        modelBuilder.Entity<VendaProduto>()
            .HasOne(vp => vp.Venda)
            .WithMany(v => v.Itens)
            .HasForeignKey(vp => vp.VendaId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<VendaProduto>()
            .HasOne(vp => vp.Produto)
            .WithMany(p => p.Vendas)
            .HasForeignKey(vp => vp.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict); // evitar deletar produto se estiver em vendas
        // Precisão de decimais para preços
        modelBuilder.Entity<Produto>()
            .Property(p => p.Preco)
            .HasPrecision(18, 2);
        modelBuilder.Entity<VendaProduto>()
            .Property(vp => vp.PrecoUnitario)
            .HasPrecision(18, 2);
    }
}