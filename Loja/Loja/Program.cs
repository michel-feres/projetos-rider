using CrudProdutoFornecedor.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte a controllers e views (MVC) ao container de DI
builder.Services.AddControllersWithViews();

/*
 * Configuração de cultura (pt-BR)
 * - Define a cultura padrão do servidor para pt-BR para que formatações
 *   (por exemplo, números e datas) sejam exibidas corretamente.
 * - Também registramos RequestLocalizationOptions para que o middleware
 *   respeite essa cultura durante o processamento das requisições.
 */
var defaultCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = new[] { defaultCulture };
    options.SupportedUICultures = new[] { defaultCulture };
});

// Registra o DbContext do Entity Framework usando Npgsql (PostgreSQL).
// A string de conexão vem do appsettings.json (DefaultConnection).
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Habilita o middleware de localização das requisições para aplicar a cultura
// configurada acima em cada requisição recebida.
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(locOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
