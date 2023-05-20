using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using SistemaInventario.Modelos;


namespace SistemaInventario.AccesoDatos.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bodega> Bodegas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<UsuarioAplicacion> UsuarioAplicacion { get; set;  }

        public DbSet<Inventario> Inventarios { get; set; }
		public DbSet<InventarioDetalle>InventariosDetalle { get; set; }
		public DbSet<BodegaProducto> BodegasProductos { get; set; }
        public DbSet<Compañia> Compañias { get; set; }

		public DbSet<CarroCompras> CarroCompras { get; set; }
		public DbSet<Orden> Ordenes { get; set; }
		public DbSet<OrdenDetalle> OrdenesDetalle { get; set; }
	}
}
