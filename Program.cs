using IS_161_Proyecto_Grupo2.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICategoriaRepositorio, RepositorioCategoria>();
builder.Services.AddScoped<ISubcategoriaRepositorio, RepositorioSubcategoria>();
builder.Services.AddScoped<IProductoRepositorio, RepositorioProducto>();
builder.Services.AddScoped<ILoteRepositorio, RepositorioLote>();
builder.Services.AddScoped<IMovimientoRepositorio, RepositorioMovimiento>();
builder.Services.AddScoped<IFacturaRepositorio, RepositorioFactura>();
builder.Services.AddScoped<IDetalleFacturaRepositorio, RepositorioDetalleFactura>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
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

app.Run();