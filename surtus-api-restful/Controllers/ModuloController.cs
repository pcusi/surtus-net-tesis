using System;
using System.Linq;
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
    }
}
