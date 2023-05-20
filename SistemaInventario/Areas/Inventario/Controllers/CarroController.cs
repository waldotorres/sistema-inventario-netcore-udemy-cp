//using Castle.Core.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using Stripe;
using System;
using System.Linq;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventario.Controllers
{
	[Area("Inventario")]
	public class CarroController : Controller
	{
		private readonly IUnidadTrabajo unidadTrabajo;
		private readonly IEmailSender emailSender;
		private readonly UserManager<IdentityUser> userManager;

		[BindProperty]
        public CarroComprasVM carroComprasVM { get; set; }

        public CarroController( IUnidadTrabajo unidadTrabajo, IEmailSender email, 
								UserManager<IdentityUser> userManager )
        {
			this.unidadTrabajo = unidadTrabajo;
			this.emailSender = email;
			this.userManager = userManager;
		}

        public IActionResult Index()
		{
			var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
			if (user == null)
			{

			}
			carroComprasVM = new CarroComprasVM()
			{
				Orden = new Orden(),
				CarroComprasLista = unidadTrabajo.CarroCompras
									.ObtenerTodos( x=> x.UsuarioAplicacionId == user.Value, incluirPropiedades:"Producto" )
			};

			carroComprasVM.Orden.TotalOrden = 0;
			carroComprasVM.Orden.UsuarioAplicacion = unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(x => x.Id == user.Value);
			foreach (var item in carroComprasVM.CarroComprasLista)
			{
				item.Precio = item.Producto.Precio;
				carroComprasVM.Orden.TotalOrden += item.Precio*item.Cantidad;
			}
			return View(carroComprasVM);
		}

		public IActionResult Mas( int id)
		{
			var itemCarro = unidadTrabajo.CarroCompras
							.ObtenerPrimero(x=> x.Id == id, incluirPropiedades:"Producto");

            itemCarro.Cantidad += 1;
			unidadTrabajo.guardar();

            return RedirectToAction("Index");
		}
        public IActionResult Menos(int id)
        {
			var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var itemCarro = unidadTrabajo.CarroCompras
                            .ObtenerPrimero(x => x.Id == id && x.UsuarioAplicacionId == userId , incluirPropiedades: "Producto");

			if (itemCarro.Cantidad <= 1)
			{
				unidadTrabajo.CarroCompras.Remover(itemCarro);
				var cantidadProductos = unidadTrabajo.CarroCompras
										.ObtenerTodos(x => x.UsuarioAplicacionId == userId)
										.ToList()
										.Count();
				HttpContext.Session.SetInt32(DS.ssCarroCompras, cantidadProductos-1);
			}
			else
			{
				itemCarro.Cantidad -= 1;
            }
            unidadTrabajo.guardar();

            return RedirectToAction("Index");
        }
        public IActionResult Remover(int id)
        {
            var itemCarro = unidadTrabajo.CarroCompras
                            .ObtenerPrimero(x => x.Id == id, incluirPropiedades: "Producto");

            unidadTrabajo.CarroCompras.Remover(itemCarro);

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var cantidadProductos = unidadTrabajo.CarroCompras
                                        .ObtenerTodos(x => x.UsuarioAplicacionId == userId)
                                        .ToList()
                                        .Count();

            HttpContext.Session.SetInt32(DS.ssCarroCompras, cantidadProductos - 1);
            unidadTrabajo.guardar();

            return RedirectToAction("Index");
        }
		[HttpGet]
		public IActionResult Proceder()
		{
            var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (user == null)
            {

            }
            carroComprasVM = new CarroComprasVM()
            {
                Orden = new Orden(),
                CarroComprasLista = unidadTrabajo.CarroCompras
                                    .ObtenerTodos(x => x.UsuarioAplicacionId == user.Value, incluirPropiedades: "Producto")
            };

            carroComprasVM.Orden.TotalOrden = 0;
            carroComprasVM.Orden.UsuarioAplicacion = unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(x => x.Id == user.Value);
            foreach (var item in carroComprasVM.CarroComprasLista)
            {
                item.Precio = item.Producto.Precio;
                carroComprasVM.Orden.TotalOrden += item.Precio * item.Cantidad;
            }
			var usuario = carroComprasVM.Orden.UsuarioAplicacion;


            carroComprasVM.Orden.NombresCliente = usuario.Nombres + " " + usuario.Apellidos;

			carroComprasVM.Orden.Telefono = usuario.PhoneNumber;
            carroComprasVM.Orden.Direccion = usuario.Direccion;
            carroComprasVM.Orden.Ciudad = usuario.Ciudad;
            carroComprasVM.Orden.Pais = usuario.Pais;

            return View(carroComprasVM);
        }

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		[ActionName("Proceder")]
		public IActionResult ProcederPost( string stripeToken )
		{
			var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
			carroComprasVM.Orden.UsuarioAplicacion = unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(x => x.Id == user.Value);
			carroComprasVM.CarroComprasLista = unidadTrabajo.CarroCompras
												.ObtenerTodos( x=> x.UsuarioAplicacionId == user.Value, 
													incluirPropiedades:"Producto");

			carroComprasVM.Orden.EstadoOrden = DS.OrdenPendiente;
			carroComprasVM.Orden.EstadoPago = DS.PagoPendiente;
			carroComprasVM.Orden.UsuarioAplicacionId = user.Value;
			carroComprasVM.Orden.FechaOrden = DateTime.Now;

			unidadTrabajo.Orden.Agregar(carroComprasVM.Orden);
			unidadTrabajo.guardar();

			carroComprasVM.Orden.TotalOrden = 0;

			foreach (var item in carroComprasVM.CarroComprasLista)
			{
				OrdenDetalle ordenDetalle = new OrdenDetalle()
				{
					ProductoId = item.ProductoId,
					OrdenId= carroComprasVM.Orden.Id,
					Precio = item.Producto.Precio,
					Cantidad = item.Cantidad
				};
				carroComprasVM.Orden.TotalOrden += ordenDetalle.Cantidad * ordenDetalle.Precio;
				unidadTrabajo.OrdenDetalle.Agregar(ordenDetalle);
			}

			unidadTrabajo.CarroCompras.RemoverRango(carroComprasVM.CarroComprasLista);
			HttpContext.Session.SetInt32(DS.ssCarroCompras, 0);
			unidadTrabajo.guardar();

			if (stripeToken == null)
			{

			}
			else // procesa pago
			{
				var opciones = new ChargeCreateOptions()
				{
					Amount = (int) carroComprasVM.Orden.TotalOrden*100,
					Currency = "PEN",
					Description = "Nro Orden: " + carroComprasVM.Orden.Id,
					Source = stripeToken
				};

				var service = new ChargeService();
				Charge charge = service.Create(opciones);
				if(charge.BalanceTransactionId == null)
				{
					carroComprasVM.Orden.EstadoPago = DS.PagoRechazado;
				}
				else
				{
					carroComprasVM.Orden.TransaccionId = charge.Id;
				}

				if( charge.Status.ToLower() == "succeeded")
				{
					carroComprasVM.Orden.EstadoPago = DS.PagoAprobado;
					carroComprasVM.Orden.EstadoOrden = DS.OrdenAprobado;
					carroComprasVM.Orden.FechaPago = DateTime.Now;
				}
			}

			unidadTrabajo.guardar();

			return RedirectToAction("OrdenConfirmacion", "Carro", new { id=carroComprasVM.Orden.Id });
		}

		public IActionResult OrdenConfirmacion(int id)
		{
			return View(id);
		}

		public IActionResult ImprimirOrden(int id)
		{
			this.carroComprasVM = new CarroComprasVM();
			this.carroComprasVM.Compañia = unidadTrabajo.Compañia
											.ObtenerTodos().ToList().OrderBy(x => x.Id).Take(1).ToList()[0];

			carroComprasVM.Orden = unidadTrabajo.Orden.ObtenerPrimero(x=> x.Id == id, incluirPropiedades: "UsuarioAplicacion");
			carroComprasVM.OrdenDetalleLista = unidadTrabajo.OrdenDetalle.ObtenerTodos(x => x.OrdenId == id,incluirPropiedades: "Producto");

			return new ViewAsPdf("ImprimirOrden", carroComprasVM)
			{
				FileName = "Orden#" + carroComprasVM.Orden.Id + ".pdf",
				PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
				PageSize = Rotativa.AspNetCore.Options.Size.A4,
				CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"
			};

		}
	}
}
