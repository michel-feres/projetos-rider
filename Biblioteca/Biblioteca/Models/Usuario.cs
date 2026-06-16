using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models;

/// <summary>
/// Representa a pessoa cadastrada no sistema da biblioteca.
/// Também é usada no login simples exigido pela atividade.
/// </summary>
public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome completo é obrigatório.")]
    [Display(Name = "Nome completo")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "Informe entre 3 e 120 caracteres.")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de nascimento")]
    public DateOnly DataNascimento { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(160, ErrorMessage = "O e-mail deve ter no máximo 160 caracteres.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    [StringLength(60, MinimumLength = 4, ErrorMessage = "A senha deve ter entre 4 e 60 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    [Display(Name = "Status")]
    public string Status => Ativo ? "Ativo" : "Inativo";

    public ICollection<Emprestimo> Emprestimos { get; set; } = new List<Emprestimo>();
    //Função para verificar a idade do usuário cadastrado e ver se ele pode pegar um livro +18
    public int Idade
    {
        get
        {
            var hoje = DateOnly.FromDateTime(DateTime.Today);
            var idade = hoje.Year - DataNascimento.Year;
            if (DataNascimento > hoje.AddYears(-idade))
            {
                idade--;
            }

            return idade;
        }
    }
}
