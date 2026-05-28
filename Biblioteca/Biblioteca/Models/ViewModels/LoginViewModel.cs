using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models.ViewModels;

/// <summary>
/// Dados necessários para o login simples por e-mail e senha.
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;
}