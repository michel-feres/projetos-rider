using System.ComponentModel.DataAnnotations;
namespace CRUDNN.Models;

public class Venda
{
    public int Id { get; set; }

    // UTC por padrão
    public DateTime Data { get; set; } = DateTime.UtcNow;

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public ICollection<VendaProduto> Itens { get; set; } = new List<VendaProduto>();
    public decimal Total => Itens?.Sum(i => i.PrecoUnitario * i.Quantidade) ?? 0m;
}