using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using surtus_api_restful.Dtos.Requests.Inscripcion;
using surtus_api_restful.Dtos.Responses.Inscripcion;
using surtus_api_restful.Models;
using surtus_api_restful.Resources;

namespace surtus_api_restful.Controllers 
{
    public class InscripcionController : AppController
    {
        public InscripcionController(SurtusDbContext db, IConfiguration config, IStringLocalizer<SharedResource> stringLocalizer) : base(db, config, stringLocalizer) {}

        [HttpPost("inscribirse")]
        public async Task<string> Inscribirse([FromBody] RegistrarInscripcionRequest request)
        {

            var claveHashed = BCrypt.Net.BCrypt.HashPassword(request.Clave, 10);

            var inscripcion = new Inscripcion
            {
                Nombres = request.Nombres,
                Apellidos = request.Apellidos,
                Usuario = request.Usuario,
                Clave = claveHashed,
                Nivel = "Básico"
            };

            _db.Inscripciones.Add(inscripcion);

            await _db.SaveChangesAsync();

            return "Inscrito";
        }

        [AllowAnonymous]
        [HttpPost("iniciarSesion")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<DatosInscritoAutenticadoResponse> IniciarSesion([FromForm] IniciarSesionRequest request)
        {
            string token = "";

            var obtenerClave = await _db.Inscripciones
                .Where(i => i.Usuario == request.Usuario)
                .SingleOrDefaultAsync();

           if (obtenerClave != null)
           {

                var autenticado = BCrypt.Net.BCrypt.Verify(request.Clave, obtenerClave.Clave);

                if (autenticado)
                {
                    var secretKey = _config.GetValue<string>("SecretKey");
                    var key = Encoding.ASCII.GetBytes(secretKey);
                    var claims = new ClaimsIdentity();
                    claims.AddClaim(new Claim("UserId", obtenerClave.Id.ToString()));

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddDays(30),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var createdToken = tokenHandler.CreateToken(tokenDescriptor);
                    token = tokenHandler.WriteToken(createdToken);
                }
                else
                {
                    throw Error(_stringLocalizer["ClaveError"], 403);
                }
            } else
            {
                throw Error(_stringLocalizer["UsuarioNoEncontrado"], 400);
            }

            return new DatosInscritoAutenticadoResponse {
                Token = token
            };
        }

        [Authorize]
        [HttpGet("datos")]
        public async Task<DatosInscritoResponse> DatosInscrito()
        {
            var userId = Convert.ToInt64(User.FindFirst("UserId").Value);
            DatosInscritoResponse inscrito = null;

            inscrito = await _db.Inscripciones
                .Where(i => i.Id == userId)
                .Select(i => new DatosInscritoResponse {
                    Nombres = i.Nombres,
                    Apellidos = i.Apellidos,
                    Usuario = i.Usuario,
                    Nivel = i.Nivel
                }).SingleOrDefaultAsync();

            return inscrito;
        }
    }
}
