using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class MarcaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        public MarcaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();            
        }

        public IActionResult Upsert(int? id)
        {
            Marca marca = new Marca();
            //nuevo registro
            if(id == null)
            {
                return View(marca);
            }
            //actualizar
            marca = _unidadTrabajo.Marca.Obtener(id.GetValueOrDefault());
            if (marca == null)
            {
                return NotFound();
            }

            return View(marca);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Marca marca)
        {
            if (ModelState.IsValid)
            {
                if (marca.Id == 0)
                {
                    _unidadTrabajo.Marca.Agregar(marca);
                }
                else
                {
                    _unidadTrabajo.Marca.Actualizar(marca);
                }
                _unidadTrabajo.guardar();

                return RedirectToAction(nameof(Index));
            }

            return View(marca);
        }

        #region API
        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var todos = _unidadTrabajo.Marca.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var marcaDb = _unidadTrabajo.Marca.Obtener(id);
            if(marcaDb == null)
            {
                return Json(new { success=false, message="Error al borrar" });
            }

            _unidadTrabajo.Marca.Remover(marcaDb);
            _unidadTrabajo.guardar();

            return Json(new { success = true, message = "Borrado exitosamente" });
        }



        #endregion
    }
}
