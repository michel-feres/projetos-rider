using System.ComponentModel.DataAnnotations;
namespace CRUDNN.ViewModels;
public class VendaItemVM
{
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public decimal PrecoAtual { get; set; }

    [Range(0, 999999)]
    public int Quantidade { get; set; } // 0 = não incluir
}