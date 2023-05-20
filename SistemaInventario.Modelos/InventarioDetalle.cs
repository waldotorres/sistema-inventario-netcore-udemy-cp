using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
	public class InventarioDetalle
	{
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name ="Inventario")]
        public int InventarioId { get; set; }
        [ForeignKey("InventarioId")]
        public Inventario Inventario { get; set; }
        [Required]
        [Display(Name ="Producto")]
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }
        [Required]
        [Display(Name ="Stock Anterior")]
        [Column(TypeName ="decimal(15,3)")]
        public decimal StockAnterior { get; set; }
        [Required]
        [Display(Name ="Cantidad")]
		[Column(TypeName = "decimal(15,3)")]
		public decimal Cantidad { get; set; }
    }
}
