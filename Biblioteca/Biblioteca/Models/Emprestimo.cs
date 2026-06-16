using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models;

public class Emprestimo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Selecione o usuário.")]
    [Display(Name = "Usuário")]
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "Selecione o livro.")]
    [Display(Name = "Livro")]
    public int LivroId { get; set; }

    [Required(ErrorMessage = "A data do empréstimo é obrigatória.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data do empréstimo")]
    public DateOnly DataEmprestimo { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "A data prevista de devolução é obrigatória.")]
    [DataType(DataType.Date)]
    [Display(Name = "Devolução prevista")]
    public DateOnly DataPrevistaDevolucao { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(7));

    [DataType(DataType.Date)]
    [Display(Name = "Data da devolução")]
    public DateOnly? DataDevolucao { get; set; }

    [Range(0, 99999, ErrorMessage = "A multa não pode ser negativa.")]
    [DataType(DataType.Currency)]
    public decimal Multa { get; set; }

    public Usuario? Usuario { get; set; }
    public Livro? Livro { get; set; }
    
    //Função para verificar se o livro foi devolvido
    [Display(Name = "Status")]
    public string Status
    {
        get
        {
            if (DataDevolucao.HasValue)
            {
                return "Devolvido";
            }

            return DataPrevistaDevolucao < DateOnly.FromDateTime(DateTime.Today) ? "Atrasado" : "Emprestado";
        }
    }

    [Display(Name = "Dias de atraso")]
    public int DiasAtraso
    {
        get
        {
            var dataReferencia = DataDevolucao ?? DateOnly.FromDateTime(DateTime.Today);
            return dataReferencia > DataPrevistaDevolucao ? dataReferencia.DayNumber - DataPrevistaDevolucao.DayNumber : 0;
        }
    }

    [Display(Name = "Multa atual")]
    public decimal MultaAtual => DataDevolucao.HasValue ? Multa : DiasAtraso * 2m;
}
