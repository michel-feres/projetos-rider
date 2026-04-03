namespace CRUDNN.ViewModels;
public class ProdutoVendaReportRow
{
    public int ProdutoId { get; set; }
    public string Produto { get; set; } = "";
    public int Quantidade { get; set; }
    public decimal Receita { get; set; }
    public decimal TicketMedio => Quantidade == 0 ? 0 : Receita / Quantidade;
}

public class RelatorioVendasVM
{
    public DateTime? DataIni { get; set; }
    public DateTime? DataFim { get; set; }

    public List<ProdutoVendaReportRow> Linhas { get; set; } = new();
    public int TotalItens => Linhas.Sum(l => l.Quantidade);
    public decimal ReceitaTotal => Linhas.Sum(l => l.Receita);

    public string PeriodoFmt =>
        (DataIni, DataFim) switch
        {
            (DateTime a, DateTime b) => $"{a:dd/MM/yyyy} a {b:dd/MM/yyyy}",
            (DateTime a, null)       => $"desde {a:dd/MM/yyyy}",
            (null, DateTime b)       => $"até {b:dd/MM/yyyy}",
            _                        => "todos os períodos"
        };
}