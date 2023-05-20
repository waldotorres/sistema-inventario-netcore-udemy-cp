using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaInventario
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}



//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using SistemaInventario.AccesoDatos.Data;
//using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
//using SistemaInventario.AccesoDatos.Repositorio;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
////builder.Services.AddRazorPages();



//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//				options.UseSqlServer(
//					builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

////services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//IdentityBuilder identityBuilder = builder.Services.AddDefaultIdentity<IdentityUser>()
//	.AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddScoped<IUnidadTrabajo, UnidadTrabajo>();
////services.AddControllersWithViews().AddRazorRuntimeCompilation(); // recomendacion del curso pero no tiene efecto adicional
//builder.Services.AddControllersWithViews();
////services.AddRazorPages(); // esto viene en el video pero VS no lo ha generado


//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//	app.UseExceptionHandler("/Error");
//	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//	app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapRazorPages();

//app.Run();

