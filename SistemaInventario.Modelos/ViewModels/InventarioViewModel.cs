﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos.ViewModels
{
	public class InventarioViewModel
	{
        public Inventario Inventario { get; set; }
        public InventarioDetalle InventarioDetalle { get; set; }
        public List<InventarioDetalle> InventarioDetalles  { get; set; }
        public IEnumerable<SelectListItem>  BodegaLista { get; set; }
		public IEnumerable<SelectListItem> ProductoLista { get; set; }
	}
}
