using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Inicializador
{
    public class DBInicializador : IDBInicializador
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DBInicializador( ApplicationDbContext db, 
                                UserManager<IdentityUser> userManager, 
                                RoleManager<IdentityRole> roleManager )
        {
            this.db = db;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public void Inicializar()
        {


            try
            {
                if (db.Database.GetPendingMigrations().Count() > 0)
                {
                    db.Database.Migrate();
                }
            }
            catch (Exception)
            {

                throw;
            }

            if ( db.Roles.Any(x=> x.Name == DS.Role_Admin ))
            {
                return;
            }

            roleManager.CreateAsync(new IdentityRole() { Name = DS.Role_Admin }).GetAwaiter().GetResult();
            roleManager.CreateAsync(new IdentityRole() { Name = DS.Role_Ventas }).GetAwaiter().GetResult();
            roleManager.CreateAsync(new IdentityRole() { Name = DS.Role_Inventario }).GetAwaiter().GetResult();
            roleManager.CreateAsync(new IdentityRole() { Name = DS.Role_Cliente }).GetAwaiter().GetResult();


            userManager.CreateAsync(new UsuarioAplicacion()
            {
                UserName = "administrador",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                Nombres = "Admin",
                Apellidos = "Admin"
            }, "aA123456!").GetAwaiter().GetResult();

            var user = db.UsuarioAplicacion.FirstOrDefault(x => x.UserName == "administrador");
            userManager.AddToRoleAsync(user, DS.Role_Admin).GetAwaiter().GetResult();

        }
    }
}
