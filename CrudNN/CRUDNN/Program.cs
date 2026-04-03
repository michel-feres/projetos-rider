using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using CRUDNN.Data;
using CRUDNN.Infrastructure;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(options =>
{
    // nosso binder vem primeiro
    options.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
});
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connStr));
// (Opcional) mapear DateTime antigo do Npgsql se você migrar projetos antigos:
// AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var supportedCultures = new[] { new CultureInfo("pt-BR") };
builder.Services.Configure<RequestLocalizationOptions>(opts =>
{
    opts.DefaultRequestCulture = new RequestCulture("pt-BR");
    opts.SupportedCultures = supportedCultures;
    opts.SupportedUICultures = supportedCultures;
});
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();