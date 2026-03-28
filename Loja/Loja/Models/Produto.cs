namespace CrudProdutoFornecedor.Models;

/*
 * Classe Produto
 * Representa um produto do sistema com propriedades essenciais:
 * - ProdutoId: chave primária (identificador do produto)
 * - Nome: nome do produto
 * - Valor: preço/valor do produto (usar decimal para valores monetários)
 * - FornecedorId: chave estrangeira para o fornecedor (inteiro)
 * - Fornecedor: propriedade de navegação para carregar o fornecedor relacionado
 *
 * Comentário: usamos decimal para o Valor porque é o tipo recomendado
 * para valores monetários (precisão, evitar imprecisão de ponto flutuante).
 */
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
