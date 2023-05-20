using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaInventario.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = $"{DS.Role_Admin}")]
	public class CompañiaController : Controller
	{
		private readonly IUnidadTrabajo _unidadTrabajo;
		private readonly IWebHostEnvironment _hostEnvironment;
		public CompañiaController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment hostEnvironment)
		{
			_unidadTrabajo = unidadTrabajo;
			_hostEnvironment = hostEnvironment;
		}

		public IActionResult Index()
		{
			var compañia = _unidadTrabajo.Compañia.ObtenerTodos();
			return View(compañia);
		}

		public IActionResult Upsert(int? id)
		{
			CompañiaVM compañiaVM = new CompañiaVM()
			{
				Compañia = new Compañia(),

				BodegaLista = _unidadTrabajo.Bodega
								.ObtenerTodos().ToList()
								.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Nombre })
			};

			//nuevo registro
			if (id == null)
			{
				return View(compañiaVM);
			}
			//actualizar
			compañiaVM.Compañia = _unidadTrabajo.Compañia.Obtener(id.GetValueOrDefault());
			if (compañiaVM.Compañia == null)
			{
				return NotFound();
			}

			return View(compañiaVM);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Upsert(CompañiaVM compañiaVM)
		{
			if (ModelState.IsValid)
			{
				// cargar imagenes
				string webRootPath = _hostEnvironment.WebRootPath;
				var files = HttpContext.Request.Form.Files;
				if (files.Count > 0)
				{
					string fileName = Guid.NewGuid().ToString();
					var uploads = Path.Combine(webRootPath, @"imagenes\compañia");
					var extension = Path.GetExtension(files[0].FileName);
					if (compañiaVM.Compañia.LogoUrl != null)
					{
						//para editar necesitamos borrar la imagen anterior
						var imagenPath = Path.Combine(webRootPath, compañiaVM.Compañia.LogoUrl.TrimStart('\\'));
						if (System.IO.File.Exists(imagenPath))
						{
							System.IO.File.Delete(imagenPath);
						}

					}
					using (var filesStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
					{
						files[0].CopyTo(filesStream);
					}
					compañiaVM.Compañia.LogoUrl = @"\imagenes\compañia\" + fileName + extension;


				}
				else
				{
					//si el update no cambia la imagen
					if (compañiaVM.Compañia.Id != 0)
					{
						Compañia compañiaDB = _unidadTrabajo.Compañia.Obtener(compañiaVM.Compañia.Id);
						compañiaVM.Compañia.LogoUrl = compañiaDB.LogoUrl;
					}
				}


				if (compañiaVM.Compañia.Id == 0)
				{
					_unidadTrabajo.Compañia.Agregar(compañiaVM.Compañia);
				}
				else
				{
					_unidadTrabajo.Compañia.Actualizar(compañiaVM.Compañia);
				}
				_unidadTrabajo.guardar();

				return RedirectToAction(nameof(Index));
			}
			else
			{
				compañiaVM.BodegaLista = _unidadTrabajo.Bodega.ObtenerTodos().Select(c => new SelectListItem
				{
					Text = c.Nombre,
					Value = c.Id.ToString()
				});
			 

				if (compañiaVM.Compañia.Id != 0)
				{
					compañiaVM.Compañia = _unidadTrabajo.Compañia.Obtener(compañiaVM.Compañia.Id);
				}

			 
			}

			return View(compañiaVM.Compañia);
		}

		#region API
		[HttpGet]
		public IActionResult ObtenerTodos()
		{
			var todos = _unidadTrabajo.Compañia.ObtenerTodos(incluirPropiedades: "Bodega").Take(1);
			return Json(new { data = todos });
		}

		

		#endregion
	}
}
