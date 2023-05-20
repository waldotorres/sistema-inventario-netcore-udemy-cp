using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnidadTrabajo _unidadTrabajo;
		private readonly ApplicationDbContext db;
         

        [BindProperty]
        public CarroComprasVM carroComprasVM { get; set; }


		public HomeController(ILogger<HomeController> logger, IUnidadTrabajo unidadTrabajo, 
                               ApplicationDbContext db )
        {
            _logger = logger;
            _unidadTrabajo = unidadTrabajo;
			this.db = db;
          
        }


        public async Task<IActionResult> Detalle(int id)
        {
            this.carroComprasVM = new CarroComprasVM();
            this.carroComprasVM.Compañia = await db.Compañias.OrderBy(x => x.Id).FirstOrDefaultAsync();
            this.carroComprasVM.BodegaProducto = await db.BodegasProductos
                                                .Include(x => x.Producto)
                                                .ThenInclude(x=> x.Categoria)
												//.Include(x => x.Producto).ThenInclude(x=> x.Marca)
												.Include( x=> x.Producto.Marca )
                                                .FirstOrDefaultAsync( x=> x.ProductoId == id 
                                                                        && x.BodegaId == this.carroComprasVM.Compañia.BodegaVentaId );
            if(this.carroComprasVM.BodegaProducto== null)
            {
                return RedirectToAction("Index");
            }

            carroComprasVM.CarroCompras = new CarroCompras()
            {
                Producto = this.carroComprasVM.BodegaProducto.Producto,
                ProductoId = this.carroComprasVM.BodegaProducto.ProductoId
			};

            return View(carroComprasVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult  Detalle( CarroComprasVM carroComprasCreacion )
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            carroComprasCreacion.CarroCompras.UsuarioAplicacionId = userId;
            var carro = _unidadTrabajo.CarroCompras
                                .ObtenerPrimero(x => x.UsuarioAplicacionId == userId
                                && x.ProductoId == carroComprasCreacion.CarroCompras.ProductoId, incluirPropiedades:"Producto");
            if(carro == null)
            {
                _unidadTrabajo.CarroCompras.Agregar(carroComprasCreacion.CarroCompras);
            }
            else
            {
                carro.Cantidad += carroComprasCreacion.CarroCompras.Cantidad;
                _unidadTrabajo.CarroCompras.Actualizar(carro);
            }

            _unidadTrabajo.guardar();
            //agregar valor a la sesion
            var numeroProductos = _unidadTrabajo.CarroCompras
                                .ObtenerTodos(x => x.UsuarioAplicacionId == carroComprasCreacion.CarroCompras.UsuarioAplicacionId)
                                .ToList()
                                .Count();

            HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos );

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            IEnumerable<Producto> productoLista = _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades: "Categoria,Marca");
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value?? null;
            if(userId != null)
            {
                var numeroProductos = _unidadTrabajo.CarroCompras
                                .ObtenerTodos(x => x.UsuarioAplicacionId == userId)
                                .ToList()
                                .Count();

                HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos);
            }
            return View(productoLista);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
