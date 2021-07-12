using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using surtus_api_restful.Dtos.Requests.Clase;
using surtus_api_restful.Dtos.Requests.Glosario;
using surtus_api_restful.Dtos.Responses.Clase;
using surtus_api_restful.Dtos.Responses.Glosario;
using surtus_api_restful.Dtos.Responses.Modulo;
using surtus_api_restful.Models;
using surtus_api_restful.Resources;

namespace surtus_api_restful.Controllers
{
    [Authorize]
    public class ClaseController : AppController
    {
        public ClaseController(SurtusDbContext db, IConfiguration config, IStringLocalizer<SharedResource> stringLocalizer) : base(db, config, stringLocalizer) { }

        [HttpPost("listar")]
        public async Task<DatosClaseResponse[]> ListarModulos([FromBody] ListarClaseModuloRequest request)
        {
            var userId = Convert.ToInt64(User.FindFirst("UserId").Value);

            var clases = await _db.Clases
                .Join(_db.InscritoClases, c => c.Id, ic => ic.IdClase,
                (c, ic) => new { c, ic })
                .Where(c => c.c.IdModulo == request.IdModulo && c.ic.IdInscrito == userId)
                .Select(c => new DatosClaseResponse
                {
                    Id = c.c.Id,
                    Nombre = c.c.Nombre,
                    Imagen = c.c.Imagen,
                    Visto = c.ic.Visto
                }).ToArrayAsync();

            return clases;
        }

        [AllowAnonymous]
        [HttpPost("agregar")]
        public async Task<string> AgregarClase([FromBody] RegistrarClaseRequest request)
        {
            var clase = new Clase
            {
                Nombre = request.Nombre,
                Imagen = request.Imagen,
                IdModulo = request.IdModulo
            };

            _db.Clases.Add(clase);

            await _db.SaveChangesAsync();

            return "Clase creada.";
        }

        [AllowAnonymous]
        [HttpGet("listarGlosario")]
        public async Task<DatosGlosarioResponse[]> ListarGlosario()
        {
            var glosario = await _db.Clases
                .Join(_db.Modulos, c => c.IdModulo, m => m.Id,
                (c, m) => new { c, m })
                .OrderBy(c => c.c.Nombre)
                .Select(c => new DatosGlosarioResponse
                {
                    Id = c.c.Id,
                    Nombre = c.c.Nombre,
                    Imagen = c.c.Imagen,
                    ModuloNombre = c.m.Nombre
                }).ToArrayAsync();

            return glosario;
        }

        [HttpPost("visto")]
        public async Task<IActionResult> ClaseVista([FromBody] ClaseVistaRequest request)
        {
            var userId = Convert.ToInt64(User.FindFirst("UserId").Value);

            var inscritoClase = await _db.InscritoClases
                .Where(ic => ic.IdClase == request.IdClase && ic.IdInscrito == userId)
                .SingleOrDefaultAsync();

            if (inscritoClase != null)
            {
                inscritoClase.Visto = true;
                await _db.SaveChangesAsync();
            } else
            {
                return StatusCode(203);
            }

            var inscritoModulo = await _db.InscritoModulos
                .Where(im => im.IdModulo == request.IdModulo && im.IdInscrito == userId)
                .SingleOrDefaultAsync();

            var claseCount = await _db.Clases.Where(m => m.IdModulo == request.IdModulo).CountAsync();

            var inscritoClaseCount = await _db.InscritoClases
                .Where(ic => ic.IdInscrito == userId
                && ic.Visto == true
                && ic.Clase.IdModulo == request.IdModulo)
                .CountAsync();

            decimal frmAvance = ((decimal)inscritoClaseCount / claseCount * 100);

            inscritoModulo.Avance = frmAvance;

            await _db.SaveChangesAsync();

            return StatusCode(200);
        }
    }
}
