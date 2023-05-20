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
    [Authorize(Roles = $"{DS.Role_Admin},{DS.Role_Inventario}")]
    public class ProductoController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment hostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();            
        }

        public IActionResult Upsert(int? id)
        {
            ProductoVM productoVM = new ProductoVM() {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(c => new SelectListItem {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                    }),
                MarcaLista = _unidadTrabajo.Marca.ObtenerTodos().Select( c=> new SelectListItem { 
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                }),
                PadreLista = _unidadTrabajo.Producto.ObtenerTodos().Select( c=> new SelectListItem { 
                    Text = c.Descripcion,
                    Value = c.Id.ToString()
                } )
            };
            //nuevo registro
            if(id == null)
            {
                return View(productoVM);
            }
            //actualizar
            productoVM.Producto = _unidadTrabajo.Producto.Obtener(id.GetValueOrDefault());
            if (productoVM.Producto == null)
            {
                return NotFound();
            }

            return View(productoVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                // cargar imagenes
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"imagenes\productos");
                    var extension = Path.GetExtension(files[0].FileName);
                    if( productoVM.Producto.ImagenUrl != null)
                    {
                        //para editar necesitamos borrar la imagen anterior
                        var imagenPath = Path.Combine(webRootPath, productoVM.Producto.ImagenUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(imagenPath))
                        {
                            System.IO.File.Delete(imagenPath);

                        }

                    }
                        using (var filesStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(filesStream);
                        }
                        productoVM.Producto.ImagenUrl = @"\imagenes\productos\" + fileName + extension;


                }
                else
                {
                    //si el update no cambia la imagen
                    if( productoVM.Producto.Id !=0)
                    {
                        Producto productoDB = _unidadTrabajo.Producto.Obtener(productoVM.Producto.Id);
                        productoVM.Producto.ImagenUrl = productoDB.ImagenUrl;
                    }
                }


                if (productoVM.Producto.Id == 0)
                {
                    _unidadTrabajo.Producto.Agregar(productoVM.Producto);
                }
                else
                {
                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);
                }
                _unidadTrabajo.guardar();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                productoVM.CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                });
                productoVM.MarcaLista = _unidadTrabajo.Marca.ObtenerTodos().Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                });

                if ( productoVM.Producto.Id != 0 )
                {
                    productoVM.Producto = _unidadTrabajo.Producto.Obtener( productoVM.Producto.Id );
                }

                productoVM.PadreLista = _unidadTrabajo.Producto.ObtenerTodos().Select(c => new SelectListItem
                {
                    Text = c.Descripcion,
                    Value = c.Id.ToString()
                });
            }

            return View(productoVM.Producto);
        } 

        #region API
        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var todos = _unidadTrabajo.Producto.ObtenerTodos( incluirPropiedades: "Categoria,Marca");
            return Json(new { data = todos });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productoDb = _unidadTrabajo.Producto.Obtener(id);
            if(productoDb == null)
            {
                return Json(new { success=false, message="Error al borrar" });
            }

            if(productoDb.ImagenUrl != null)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var imagenPath = Path.Combine(webRootPath, productoDb.ImagenUrl.TrimStart('\\'));
                if ( System.IO.File.Exists(imagenPath) )
                {
                    System.IO.File.Delete(imagenPath);
                }
            }

            _unidadTrabajo.Producto.Remover(productoDb);
            _unidadTrabajo.guardar();

            return Json(new { success = true, message = "Borrado exitosamente" });
        }

        #endregion
    }
}
