
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Inicializador;
using SistemaInventario.AccesoDatos.Repositorio;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Utilidades;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaInventario
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();



            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));

            services.AddScoped<IUnidadTrabajo, UnidadTrabajo>();
            //services.AddControllersWithViews().AddRazorRuntimeCompilation(); // recomendacion del curso pero no tiene efecto adicional
            services.AddControllersWithViews( ops =>
            {
                ops.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));
            } );
            //services.AddRazorPages(); // esto viene en el video pero VS no lo ha generado
            services.AddRazorPages();

            services.ConfigureApplicationCookie(opciones =>
            {
                opciones.LoginPath = "/Identity/Account/Login";
                opciones.LogoutPath = "/Identity/Account/Logout";
                opciones.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            //services.PostConfigure<CookieAuthenticationOptions>( IdentityConstants.ApplicationScheme, opciones  =>
            //{
            //    opciones.LoginPath = "/Identity/Account/Login";
            //    opciones.LogoutPath = "/Identity/Account/Logout";
            //    opciones.AccessDeniedPath = "/Identity/Account/AccessDenied";
            //});

            services.AddSession(opciones =>
            {
                opciones.IdleTimeout = TimeSpan.FromMinutes(30);
                opciones.Cookie.HttpOnly = true;
                opciones.Cookie.IsEssential = true;

            });

            services.AddScoped<IDBInicializador, DBInicializador>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDBInicializador dbInicializador )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();   


            app.UseAuthentication();
            app.UseAuthorization();
            //dbInicializador.Inicializar();

            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Inventario}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            Rotativa.AspNetCore.RotativaConfiguration.Setup(env.WebRootPath, "..\\Rotativa\\Windows\\");

            

        }
    }
}
