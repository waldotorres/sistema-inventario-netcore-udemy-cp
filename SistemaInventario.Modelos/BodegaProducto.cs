using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
	public class BodegaProducto
	{
        public int Id { get; set; }
        [Required]
        [Display(Name ="Bodega")]
        public int BodegaId { get; set; }
        [ForeignKey("BodegaId")]
        public Bodega Bodega { get; set; }
        [Required]
        [Display(Name ="Prodcuto")]
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }
        [Required]
        [Display(Name ="Cantidad")]
        [Column( TypeName ="decimal(15,3)" )]
        public decimal Cantidad { get; set; }

    }
}
