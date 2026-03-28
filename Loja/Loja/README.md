# Loja - Projeto de Exemplo

Este repositório contém uma aplicação ASP.NET Core MVC simples para gerenciar Produtos e Fornecedores.
O objetivo deste README é explicar como executar o projeto, onde foram feitos ajustes importantes
(relacionados à cultura e validação) e como usar a aplicação localmente.

---

Pré-requisitos
- .NET 10 SDK
- PostgreSQL (se estiver usando a connection string configurada)
- Microsoft Visual Studio 2026 (opcional) ou dotnet CLI

Executando a aplicação
1. Abra a solução no Visual Studio ou use o terminal:

   cd C:\Users\PC\source\repos\Loja\Loja
   dotnet run

2. Acesse no navegador: https://localhost:7070 (porta pode variar)

Principais ajustes realizados

1) Cultura (pt-BR)
- A aplicação foi configurada para usar a cultura pt-BR por padrão (Program.cs):
  isso garante que formatações de números e datas sejam exibidas com vírgula/aspas
  conforme a convenção brasileira.

2) Validação client-side para decimais com vírgula
- O projeto sobrescreve o método `$.validator.methods.number` do jQuery Validate
  em `wwwroot/js/site.js` para aceitar vírgula como separador decimal (ex.: `10,50`).
- Os scripts de validação (jquery.validate e jquery.validate.unobtrusive) são carregados
  antes do `site.js` no `_Layout.cshtml` para que o override funcione.

3) Campos Valor
- As views de criação/edição (`Views/Produtoes/Create/Edit`) usam `<input type="text">`
  para o campo Valor. Isso evita a validação nativa do navegador que exige ponto.
- O model binder do ASP.NET, com a cultura pt-BR configurada, é responsável por converter
  a string com vírgula em decimal no servidor.

Arquivos importantes
- Program.cs: configuração de serviços, cultura e DbContext
- Data/ApplicationDbContext.cs: DbContext e configuração das entidades
- Models/Produto.cs, Models/Fornecedor.cs: modelos do domínio
- Views/Produtoes/*, Views/Fornecedors/*: views geradas pelo scaffolding
- wwwroot/js/site.js: customização da validação client-side

Notas finais
- Se desejar suportar múltiplas culturas e detectar a cultura do navegador,
  ajuste `RequestLocalizationOptions` em Program.cs para listar várias culturas
  e habilite a detecção com `RequestCultureProviders`.

- Se preferir usar Globalize ou outro pacote para validação cultural no client,
  posso ajustar o projeto para usar esses scripts.


---

Se quiser que eu gere comentários XML (///) para os models ou adicione instruções
para migração de banco (EF Core migrations), diga qual opção prefere.