using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using surtus_api_restful.Dtos.Requests.Clase;
using surtus_api_restful.Dtos.Responses.Clase;
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
            var clases = await _db.Clases
                .Where(c => c.IdModulo == request.IdModulo)
                .Select(c => new DatosClaseResponse
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Imagen = c.Imagen,
                    Video = c.Video
                }).ToArrayAsync();

            return clases;
        }

        [HttpPost("agregar")]
        public async Task<string> AgregarClase([FromBody] RegistrarClaseRequest request)
        {
            var clase = new Clase
            {
                Nombre = request.Nombre,
                Imagen = request.Imagen,
                Video = request.Video,
                IdModulo = request.IdModulo
            };

            _db.Clases.Add(clase);

            await _db.SaveChangesAsync();

            return "Clase creada.";
        }
    }
}
