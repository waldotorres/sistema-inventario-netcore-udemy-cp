using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Utilidades
{
    public static class DS
    {
        public const string Role_Cliente = "Cliente";
		public const string Role_Admin = "Admin";
		public const string Role_Inventario = "Inventario";
		public const string Role_Ventas = "Ventas";
        public const string ssCarroCompras = "Sesion carro compras";

        public const string OrdenPendiente = "Pendiente";
		public const string OrdenAprobado = "Aprobado";
		public const string OrdenEnProceso = "EnProceso";
		public const string OrdenEnviado = "Enviado";
		public const string OrdenCancelado = "Cancelado";
		public const string OrdenDevuelto = "Devuelto";
		public const string OrdenRechazado = "Rechazado";

		public const string PagoPendiente = "Pendiente";
		public const string PagoAprobado = "Aprobado";
		public const string PagoRetrasado = "Retrasado";
		public const string PagoRechazado = "Rechazado";
        public const string PagoDevuelto= "Devuelto";
        public const string PagoCancelado = "Cancelado";

    }
}
