using System.ComponentModel.DataAnnotations;
namespace CRUDNN.Models;

public class Cliente
{
    public int Id { get; set; }
    [Display(Name = "Nome completo")]
    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(120)]
    public string Nome { get; set; } = string.Empty;
    [Display(Name = "E-mail")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [StringLength(120)]
    public string? Email { get; set; }
    [Display(Name = "Telefone")]
    [StringLength(20)]
    [RegularExpression(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$",
        ErrorMessage = "Use o formato (99) 99999-9999")]
    public string? Telefone { get; set; }
    public ICollection<Venda>? Vendas { get; set; }
}