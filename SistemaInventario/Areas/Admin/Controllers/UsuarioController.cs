using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.Utilidades;
using System;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UsuarioController( ApplicationDbContext context )
        {
            this._db = context;
        }



        public IActionResult Index()
        {
            return View();
        }

        #region API

        [HttpGet]
        public async Task<ActionResult> ObtenerTodos()
        {
            var usuariosLista = await _db.UsuarioAplicacion.ToListAsync();
            var userRoles = await _db.UserRoles.ToListAsync();
            var roles = await _db.Roles.ToListAsync();


            foreach (var usuario in usuariosLista)
            {
                var roleId = userRoles.FirstOrDefault(x => x.UserId == usuario.Id).RoleId;
                usuario.Role = roles.FirstOrDefault(x => x.Id == roleId).Name;
            }

            return Json(new { data = usuariosLista });

        }

        [HttpPost]
        public async Task<ActionResult> BloquearDesbloquear([FromBody] string id)
        {
            var usuario = await _db.UsuarioAplicacion.FirstOrDefaultAsync(x => x.Id == id);
            if(usuario == null)
            {
                //return NotFound("Error de usuario");  
                return Json(new { data = new { success = false }, message = "Error de usuaio." });
            }

            usuario.LockoutEnd = (usuario.LockoutEnd != null && usuario.LockoutEnd > DateTime.Now) ? null:  DateTime.Now.AddYears(100)  ;

            await _db.SaveChangesAsync();
            //return Ok("Operacion exitosa");
            return Json(new { success=true, message ="Operacion exitosa" });

        }


        #endregion

    }
}
