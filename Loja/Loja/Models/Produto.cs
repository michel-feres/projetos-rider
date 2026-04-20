namespace CrudFornecedorProduto.Models;

public class Produto
{
    // Identificador único do produto (PK)
    public int ProdutoId { get; set; }

    // Nome do produto (não nulo)
    public string Nome { get; set; } = string.Empty;

    // Valor do produto. Decimal é apropriado para dinheiro.
    public decimal Valor { get; set; }

    // Chave estrangeira para o fornecedor. Define a relação 1 (Fornecedor) -> N (Produtos).
    public int FornecedorId { get; set; }

    // Propriedade de navegação para o fornecedor. Pode ser nula se não carregada.
    public Fornecedor? Fornecedor { get; set; }
}
