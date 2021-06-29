using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using surtus_api_restful.Dtos.Requests.Modulo;
using surtus_api_restful.Dtos.Responses.Modulo;
using surtus_api_restful.Models;
using surtus_api_restful.Resources;

namespace surtus_api_restful.Controllers
{
    [Authorize]
    public class ModuloController : AppController
    {
        public ModuloController(SurtusDbContext db, IConfiguration config, IStringLocalizer<SharedResource> stringLocalizer) : base(db, config, stringLocalizer) {}

        [HttpGet("listar")]
        public async Task<DatosModuloResponse[]> ListarModulos()
        {
            var modulos = await _db.Modulos
                .Where(m => m.Nivel == "Básico")
                .Select(c => new DatosModuloResponse
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Imagen = c.Imagen,
                    Nivel = c.Nivel
                }).ToArrayAsync();

            return modulos;
        }

        [HttpPost("agregar")]
        public async Task<string> AgregarModulo([FromBody] RegistrarModuloRequest request)
        {
            var modulo = new Modulo
            {
                Nombre = request.Nombre,
                Imagen = request.Imagen,
                Nivel = request.Nivel
            };

            _db.Modulos.Add(modulo);

            await _db.SaveChangesAsync();

            return "Módulo creado.";
        }


        [HttpPost("inscritoModulo")]
        public async Task<IActionResult> InscripcionPorModulo([FromBody] RegistrarInscritoModuloRequest request)
        {

            var userId = Convert.ToInt64(User.FindFirst("UserId").Value);

            var encontrarInscritoModulo = await _db.InscritoModulos
                .Where(im => im.IdModulo == request.IdModulo && im.IdInscrito == userId)
                .FirstOrDefaultAsync();

            var inscritoModulo = new InscritoModulo();
            var inscritoClase = new InscritoClase();

            if (encontrarInscritoModulo == null)
            {
                inscritoModulo.IdInscrito = userId;
                inscritoModulo.IdModulo = request.IdModulo;
                inscritoModulo.Avance = 0;

                _db.InscritoModulos.Add(inscritoModulo);

                await _db.SaveChangesAsync();

                var encontrarClaseModulo = await _db.Clases.Where(m => m.IdModulo == request.IdModulo).ToArrayAsync();
                var claseCount = await _db.Clases.Where(m => m.IdModulo == request.IdModulo).CountAsync();

                for (var i = 0; i < claseCount; i++)
                {
                    foreach (var clase in encontrarClaseModulo)
                    {

                        var encontrarInscritoClase = await _db.InscritoClases.
                            Where(ic => ic.IdInscrito == userId && ic.IdClase == clase.Id).SingleOrDefaultAsync();

                        if (encontrarInscritoClase == null)
                        {
                            inscritoClase.IdClase = clase.Id;
                            inscritoClase.IdInscrito = userId;
                            inscritoClase.Visto = false;

                            _db.InscritoClases.Add(inscritoClase);

                            await _db.SaveChangesAsync();
                        }

                        else
                        {
                            _db.Entry(inscritoClase).State = EntityState.Detached;
                        }
                    }
                }

            } else
            {
                return StatusCode(203);
            }

            return StatusCode(200);
        }
    }
}
