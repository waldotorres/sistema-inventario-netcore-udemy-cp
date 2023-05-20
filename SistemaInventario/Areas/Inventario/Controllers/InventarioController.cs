using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaInventario.Areas.Inventario.Controllers
{
	[Area("Inventario")]
	[Authorize( Roles = $"{DS.Role_Admin},{DS.Role_Inventario}" )]
	public class InventarioController : Controller
	{
		private readonly ApplicationDbContext db;
		[BindProperty]
		public InventarioViewModel InventarioVM { get; set; }


		public InventarioController( ApplicationDbContext db )
        {
			this.db = db;
		}
        public IActionResult Index()
		{
			return View();
		}



		public async Task<IActionResult> NuevoInventario(int? inventarioId )
			{
			this.InventarioVM = new InventarioViewModel()
			{
				BodegaLista = await db.Bodegas.Select( x=> new SelectListItem() {  Value = x.Id.ToString(), Text = x.Nombre  } ).ToListAsync(),
				ProductoLista = await db.Productos.Select( x=> new SelectListItem() {  Value= x.Id.ToString(), Text = x.Descripcion } ).ToListAsync(),

			};

			this.InventarioVM.ProductoLista = this.InventarioVM.ProductoLista.Prepend(new SelectListItem(){ Text = "<Seleccione un valor>", Value = "" });

			this.InventarioVM.InventarioDetalles = new List<InventarioDetalle>();
			if ( inventarioId != null )
			{
				this.InventarioVM.Inventario = await db.Inventarios.FirstOrDefaultAsync(x => x.Id == inventarioId);
				this.InventarioVM.InventarioDetalles = await db.InventariosDetalle
														.Include(x=> x.Producto)
														.ThenInclude(x=> x.Marca)
														.Where(x => x.InventarioId == inventarioId   )
														.ToListAsync();
			}

			return View(this.InventarioVM);

		}

		[HttpPost]
		public async Task<IActionResult> AgregarProductoPost(int producto, decimal cantidad, int inventarioId)
		{
			this.InventarioVM.Inventario.Id = inventarioId;
			if (this.InventarioVM.Inventario.Id == 0)
			{
				this.InventarioVM.Inventario.Estado = false;
				this.InventarioVM.Inventario.FechaInicial = DateTime.Now;
				var claim = User.Claims.FirstOrDefault( x=> x.Type ==  ClaimTypes.NameIdentifier );
				this.InventarioVM.Inventario.UsuarioAplicacionId = claim.Value;
				db.Inventarios.Add(this.InventarioVM.Inventario);
				await db.SaveChangesAsync();
			}
			else
			{
				this.InventarioVM.Inventario = await db.Inventarios.FirstOrDefaultAsync(x => x.Id == inventarioId);
			}

			var bodegaProducto = await db.BodegasProductos
										.Include(x => x.Producto)
										.FirstOrDefaultAsync(x =>	x.ProductoId == producto 
																	&& x.BodegaId == this.InventarioVM.Inventario.BodegaId );

			var detalleInventario = await db.InventariosDetalle
									.Include(x => x.Producto)
									.FirstOrDefaultAsync(x =>	x.ProductoId == producto 
																&& x.InventarioId == this.InventarioVM.Inventario.Id);

			if (detalleInventario == null)
			{
				this.InventarioVM.InventarioDetalle = new InventarioDetalle()
				{
					ProductoId = producto,
					InventarioId = this.InventarioVM.Inventario.Id,
					StockAnterior = bodegaProducto!=null?bodegaProducto.Cantidad:0,
					Cantidad = cantidad

				};

				db.InventariosDetalle.Add(this.InventarioVM.InventarioDetalle);
				await db.SaveChangesAsync();
			}
			else
			{
				detalleInventario.Cantidad += cantidad;
				await db.SaveChangesAsync();
			}




			return RedirectToAction("NuevoInventario", new { inventarioId = this.InventarioVM.Inventario.Id });
		}

		public async Task<IActionResult> Mas(int id)
		{
			this.InventarioVM = new InventarioViewModel()
			{

			};
			var detalleInventario = await db.InventariosDetalle.FirstOrDefaultAsync(x => x.Id == id);
			this.InventarioVM.Inventario = await db.Inventarios.FirstOrDefaultAsync(x => x.Id == detalleInventario.InventarioId  );
			detalleInventario.Cantidad += 1;
			await db.SaveChangesAsync();

			return RedirectToAction("NuevoInventario", new { inventarioId = this.InventarioVM.Inventario.Id });
		}
		public async Task<IActionResult> Menos(int id)
		{
			this.InventarioVM = new InventarioViewModel()
			{

			};
			var detalleInventario = await db.InventariosDetalle.FirstOrDefaultAsync(x => x.Id == id);
			this.InventarioVM.Inventario = await db.Inventarios.FirstOrDefaultAsync(x => x.Id == detalleInventario.InventarioId);

			if(detalleInventario.Cantidad == 1)
			{
				db.InventariosDetalle.Remove(detalleInventario);
			}
			else
			{
				detalleInventario.Cantidad -= 1;
			}
			await db.SaveChangesAsync();

			return RedirectToAction("NuevoInventario", new { inventarioId = this.InventarioVM.Inventario.Id });
		}

		//[HttpPost]
		public async Task<IActionResult> GenerarStock(int id)
		{
			var inventario = await db.Inventarios.FirstOrDefaultAsync( x=> x.Id == id);
			var detalleInventario = await db.InventariosDetalle.Where(x => x.InventarioId == id).ToListAsync();

			foreach (var fila in detalleInventario)
			{
				var bodegaProducto = await db.BodegasProductos
										.Include(x => x.Producto)
										.FirstOrDefaultAsync(x => x.ProductoId == fila.ProductoId 
																	&& x.BodegaId == inventario.BodegaId);

				if(bodegaProducto != null)
				{
					bodegaProducto.Cantidad += fila.Cantidad;
				}
				else
				{
					bodegaProducto = new BodegaProducto()
					{
						Cantidad = fila.Cantidad,
						BodegaId = inventario.BodegaId,
						ProductoId = fila.ProductoId
					};

					db.BodegasProductos.Add(bodegaProducto);
				}
			}
			//
			inventario.Estado = true;
			inventario.FechaFinal = DateTime.Now;
			await db.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		public IActionResult Historial()
		{

			return View();
		}
        public async Task<IActionResult> DetalleHistorial(int id)
        {
			var detalleHistorial = await db.InventariosDetalle
										.Include(x => x.Producto)
										.ThenInclude(x => x.Marca)
										.Where(x => x.InventarioId == id)
										.ToListAsync();
            return View(detalleHistorial);
        }

        #region API

        [HttpGet]
		public async Task<IActionResult> ObtenerTodos()
		{
			var entidades = await db.BodegasProductos
							.Include( x=> x.Bodega )
							.Include( x=> x.Producto)
							.ToListAsync();


			return  Json(new {  data = entidades });
			//return Json( data : entidades);
			//return new JsonResult( entidades);
		}

		[HttpGet]
		public async Task<IActionResult> ObtenerHistorial()
		{
			var inventarios = await db.Inventarios
									.Include( x=> x.Bodega)
									.Include( x=> x.UsuarioAplicacion)
									.Where( x=> x.Estado )
									.ToListAsync();

			return Json(new { data = inventarios });
		}



		#endregion

	}
}
