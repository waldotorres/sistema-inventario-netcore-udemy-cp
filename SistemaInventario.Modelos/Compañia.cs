﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
	public class Compañia
	{
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:80)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(200)]
        public string  Descripcion { get; set; }
        [Required]
        [StringLength(60)]
        public string Pais { get; set; }
        [Required]
        [MaxLength(60)]
        public string Ciudad { get; set; }
        [Required]
        [MaxLength (100)]
        public string Direccion { get; set; }
		[Required]
		[MaxLength(40)]
		public string Telefono { get; set; }
        [Required]
        public int BodegaVentaId { get; set; }
        [ForeignKey("BodegaVentaId")]
        public Bodega Bodega { get; set; }
        public string LogoUrl { get; set; }


    }
}
