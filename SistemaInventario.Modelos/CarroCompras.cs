using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
	public class CarroCompras
	{
		[Key]
        public int Id { get; set; }
		[Required]		
        public string  UsuarioAplicacionId { get; set; }
		[ForeignKey("UsuarioAplicacionId")]
		public UsuarioAplicacion UsuarioAplicacion { get; set; }
		[Required]
        public int ProductoId { get; set; }
		[ForeignKey("ProductoId")]
        public Producto Producto { get; set; }
		[Required]
		[Range(1, 1000, ErrorMessage = "El valor debe estar entre {1} - {2}")]
		public int Cantidad { get; set; } = 1;
		[NotMapped]
        public double Precio { get; set; }
    }
}
