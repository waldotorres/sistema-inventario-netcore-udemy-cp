using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos.ViewModels
{
	public class CarroComprasVM
	{
        public Compañia Compañia { get; set; }
        public BodegaProducto BodegaProducto { get; set; }
        public CarroCompras CarroCompras { get; set; }
        public IEnumerable<CarroCompras> CarroComprasLista { get; set; }
        public Orden Orden { get; set; }
        public IEnumerable<OrdenDetalle> OrdenDetalleLista { get; set; }
    }
}
