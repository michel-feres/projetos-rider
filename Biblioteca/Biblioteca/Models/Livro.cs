using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models;

public class Livro
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(160, MinimumLength = 2, ErrorMessage = "Informe entre 2 e 160 caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O autor é obrigatório.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Informe entre 2 e 120 caracteres.")]
    public string Autor { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O ISBN deve ter no máximo 20 caracteres.")]
    [Display(Name = "ISBN")]
    public string? Isbn { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Informe entre 2 e 80 caracteres.")]
    public string Categoria { get; set; } = string.Empty;

    [Range(0, 18, ErrorMessage = "Informe uma faixa etária entre 0 e 18 anos.")]
    [Display(Name = "Faixa etária permitida")]
    public int FaixaEtariaPermitida { get; set; }

    [Range(1000, 2100, ErrorMessage = "Informe um ano válido.")]
    [Display(Name = "Ano de publicação")]
    public int AnoPublicacao { get; set; } = DateTime.Today.Year;

    [Range(0, 999, ErrorMessage = "A quantidade deve ser maior ou igual a zero.")]
    [Display(Name = "Quantidade total")]
    public int QuantidadeTotal { get; set; } = 1;

    [Range(0, 999, ErrorMessage = "A quantidade disponível deve ser maior ou igual a zero.")]
    [Display(Name = "Quantidade disponível")]
    public int QuantidadeDisponivel { get; set; } = 1;

    public ICollection<Emprestimo> Emprestimos { get; set; } = new List<Emprestimo>();

    [Display(Name = "Disponibilidade")]
    public string Disponibilidade => QuantidadeDisponivel > 0 ? "Disponível" : "Indisponível";
}
