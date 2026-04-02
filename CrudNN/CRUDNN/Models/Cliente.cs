using System.ComponentModel.DataAnnotations;
namespace CRUDNN.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Nome { get; set; } = string.Empty;

    [EmailAddress, StringLength(120)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Telefone { get; set; }

    public ICollection<Venda>? Vendas { get; set; }
}