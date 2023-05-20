using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
	public class CompañiaRepositorio : Repositorio<Compañia>, ICompañiaRepositorio
	{
		private readonly ApplicationDbContext db;

		public CompañiaRepositorio(ApplicationDbContext db) : base(db)
		{
			this.db = db;
		}

		public void Actualizar(Compañia compañia)
		{
			var compañiaDb =  db.Compañias.FirstOrDefault(x => x.Id == compañia.Id);
			if(compañiaDb!= null)
			{
				compañiaDb.LogoUrl = compañia.LogoUrl != null ? compañia.LogoUrl : compañia.LogoUrl;
				compañiaDb.Nombre = compañia.Nombre;
				compañiaDb.Descripcion = compañia.Descripcion;
				compañiaDb.Direccion = compañia.Direccion;
				compañiaDb.BodegaVentaId = compañia.BodegaVentaId;
				compañiaDb.Pais = compañia.Pais;
				compañiaDb.Ciudad = compañia.Ciudad;
				compañiaDb.Telefono = compañia.Telefono;
			}
		}
	}
}
