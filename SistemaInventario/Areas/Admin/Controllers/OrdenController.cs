using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using Stripe;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdenController:Controller
    {
        private readonly IUnidadTrabajo unidadTrabajo;
		private readonly ApplicationDbContext db;

		public OrdenController( IUnidadTrabajo unidadTrabajo, ApplicationDbContext db )
        {
            this.unidadTrabajo = unidadTrabajo;
			this.db = db;
		}

        [BindProperty]
        public OrdenDetalleViewModel ordenDetalleVM { get; set; }

        public IActionResult Index()
        {
            return View();  
        }

        public IActionResult Detalle(int id)
        {
            this.ordenDetalleVM = new OrdenDetalleViewModel()
            {
                Orden = unidadTrabajo.Orden.ObtenerPrimero(x => x.Id == id, incluirPropiedades: "UsuarioAplicacion"),
                OrdenDetalleLista = unidadTrabajo.OrdenDetalle.ObtenerTodos(x=> x.OrdenId == id, incluirPropiedades:"Producto")
			};

            return View(ordenDetalleVM);

		}

        [Authorize( Roles = $"{ DS.Role_Admin },{ DS.Role_Ventas }" )]
        public IActionResult Procesar( int id )
        {
            var orden = unidadTrabajo.Orden.ObtenerPrimero(x => x.Id == id);
            orden.EstadoOrden = DS.OrdenEnProceso;
            unidadTrabajo.guardar();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = $"{DS.Role_Admin},{DS.Role_Ventas}")]
        public IActionResult EnviarOrden(int id)
        {
            var orden = unidadTrabajo.Orden.ObtenerPrimero(x => x.Id == this.ordenDetalleVM.Orden.Id );
            orden.NumeroEnvio = this.ordenDetalleVM.Orden.NumeroEnvio;
            orden.Carrier = this.ordenDetalleVM.Orden.Carrier;
            orden.EstadoOrden = DS.OrdenEnviado;
            orden.FechaEnvio = DateTime.Now;

            unidadTrabajo.guardar();
            return RedirectToAction("Index");
        }


        public IActionResult CancelarOrden(int id)
        {
            var orden = unidadTrabajo.Orden.ObtenerPrimero(x => x.Id == id);
            if ( orden.EstadoPago == DS.PagoAprobado )
            {
                var opciones = new RefundCreateOptions()
                {
                    Amount = (int)(orden.TotalOrden * 100.00),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge =orden.TransaccionId
                };

                Refund refund = new RefundService().Create( opciones );
                orden.EstadoOrden = DS.OrdenDevuelto;
                orden.EstadoPago = DS.PagoDevuelto;
            }
            else
            {
                orden.EstadoOrden = DS.OrdenCancelado;
                orden.EstadoPago = DS.PagoCancelado;
            }

            unidadTrabajo.guardar();
            return RedirectToAction("Index");
        }

        #region API
        [HttpGet]
        public IActionResult ObtenerOrdenLista(string estado)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var queryable = db.Ordenes  
                            .Include(x => x.UsuarioAplicacion).AsQueryable();
            

            if( !(User.IsInRole(DS.Role_Admin) || User.IsInRole(DS.Role_Ventas)))
            {
				queryable = queryable.Where(x => x.UsuarioAplicacionId == userId);
            }

            switch (estado)
            {
                case "pendiente":
					queryable = queryable.Where(x => x.EstadoPago == DS.PagoPendiente || x.EstadoPago == DS.PagoRetrasado);
				    break;
				case "aprobado":
					queryable = queryable.Where(x => x.EstadoPago == DS.PagoAprobado );
					break;
				case "rechazado":
					queryable = queryable.Where(x => x.EstadoOrden == DS.OrdenRechazado || x.EstadoOrden == DS.OrdenCancelado);
					break;
				case "completado":
					queryable = queryable.Where(x => x.EstadoOrden == DS.OrdenEnviado );
					break;
				default:
					break;
            }


            var resultado = queryable.ToList();
          
            return Json(new { data = resultado });

        }

        #endregion

    }
}
