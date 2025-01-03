using CleanArch.Infra.Data.Context;
using CleanArch.Infra.IoC;
using CleanArch.MVC.MappingConfig;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços de infraestrutura e outras dependências
builder.Services.AddInfrastructure(builder.Configuration); // Supondo que você tenha uma extensão para isso
builder.Services.AddAutoMapperConfiguration(); // Supondo que você tenha uma extensão para isso

// Adicionando suporte a controladores e Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configuração do pipeline de requisição (middleware)
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro na migração ou alimentação dos dados.");
    }
}

app.Run();
