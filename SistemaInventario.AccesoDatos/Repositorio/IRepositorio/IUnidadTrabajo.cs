using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio.IRepositorio
{
    public interface IUnidadTrabajo : IDisposable
    {
        IBodegaRepositorio Bodega { get; }
        ICategoriaRepositorio Categoria { get;  }
        IMarcaRepositorio Marca { get; }
        IProductoRepositorio Producto{ get; }
        ICompañiaRepositorio Compañia { get; }

		ICarroComprasRepositorio CarroCompras { get; }
		IOrdenRepositorio Orden { get; }
		IOrdenDetalleRepositorio OrdenDetalle { get; }

		IUsuarioAplicacionRepositorio UsuarioAplicacion { get; }

        void guardar();
    }
}
