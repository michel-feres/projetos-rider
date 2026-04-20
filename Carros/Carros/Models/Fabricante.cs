namespace Carros.Models;

public class Fabricante
{
    public int FabricanteId { get; set; }

    public string Nome { get; set; } = string.Empty; 
    
    public string Endereco { get; set; } = string.Empty;
    
    public string Telefone { get; set; } = string.Empty;
    
    public ICollection<Carro> Carros { get; set; } = new List<Carro>();
}