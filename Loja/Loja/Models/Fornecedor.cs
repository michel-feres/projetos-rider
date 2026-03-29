namespace CrudFornecedorProduto.Models;

/*
 * Classe Fornecedor
 * Representa um fornecedor (empresa/pessoa) que fornece produtos.
 * - FornecedorId: chave primária
 * - Nome: nome do fornecedor
 * - Cidade: cidade do fornecedor
 * - Produtos: coleção de produtos fornecidos (relação 1 -> N)
 */
public class Fornecedor
{
    // Identificador do fornecedor (PK)
    public int FornecedorId { get; set; }

    // Nome do fornecedor
    public string Nome { get; set; } = string.Empty;

    // Cidade do fornecedor (opcional de acordo com o domínio)
    public string Cidade { get; set; } = string.Empty;

    // Coleção de produtos associados a este fornecedor. Inicializada
    // para evitar null checks ao manipular a lista.
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}
