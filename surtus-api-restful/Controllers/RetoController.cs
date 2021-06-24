using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using surtus_api_restful.Dtos.Requests.Reto;
using surtus_api_restful.Dtos.Responses.Reto;
using surtus_api_restful.Models;
using surtus_api_restful.Resources;

namespace surtus_api_restful.Controllers
{
    [Authorize]
    public class RetoController : AppController
    {
        public RetoController(SurtusDbContext db, IConfiguration config, IStringLocalizer<SharedResource> stringLocalizer) : base(db, config, stringLocalizer) { }

        [HttpGet("listar")]
        public async Task<DatosRetoResponse[]> ListarRetos()
        {
            var userId = Convert.ToInt64(User.FindFirst("UserId").Value);

            var retos = await _db.Retos
                .Include(r => r.EvaluacionRetos)
                    .ThenInclude(er => er.Reto)
                .Where(r => r.IdInscrito == userId)
                .Select(r => new DatosRetoResponse
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    IdInscrito = userId,
                    IdModulo = r.IdModulo,
                    Estado = r.Estado,
                    Evaluacion = r.EvaluacionRetos
                        .Select(er => new DatosEvaluacionRetoResponse
                        {
                            Nota = er.Nota,
                            Estado = er.Estado
                        }
                    ).ToArray()
                }).ToArrayAsync();

            return retos;
        }

        [HttpPost("agregar")]
        public async Task<string> AgregarReto([FromBody] RegistrarRetoRequest request)
        {
            var userId = Convert.ToInt64(User.FindFirst("UserId").Value);

            var reto = new Reto
            {
                Nombre = request.Nombre,
                IdInscrito = userId,
                IdModulo = request.IdModulo,
                Estado = "N",
                FechaCreacion = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            _db.Retos.Add(reto);

            await _db.SaveChangesAsync();

            return "Reto creado.";
        }

        [HttpPost("evaluacionReto")]
        public async Task<EvaluacionRetoResponse> EvaluacionInscritoReto([FromBody] GenerarEvaluacionRequest request)
        {
            var userId = Convert.ToInt64(User.FindFirst("UserId").Value);

            var evaluacion = new EvaluacionRetoResponse {
                Evaluacion = null
            };

            var preguntas = new List<PreguntasRetoResponse>();

            var respuestas = new RespuestasResponse[0];

            var reto = await _db.Retos
                .Where(r => r.Id == request.IdReto && r.IdInscrito == userId)
                .SingleOrDefaultAsync();

            if (evaluacion == null)
            {
                throw Error(_stringLocalizer["RetoNoEncontrado"], 400);
            } else
            {
                if (reto != null)
                {

                    var preguntasAleatorias = await _db.Clases
                        .OrderBy(c => Guid.NewGuid())
                        .Take(5)
                        .ToArrayAsync();

                    foreach (var preguntaAl in preguntasAleatorias)
                    {
                        var pregunta = new PreguntasRetoResponse
                        {
                            Pregunta = null,
                            Imagen = null,
                            Respuestas = new RespuestasResponse[0]
                        };

                        pregunta.Pregunta = preguntaAl.Nombre;
                        pregunta.Imagen = preguntaAl.Imagen;

                        #region Obtener Respuestas Aleatorias y Respuesta correcta
                        respuestas = await _db.Clases
                             .Where(c => c.Nombre != preguntaAl.Nombre)
                             .Select(r => new RespuestasResponse
                             {
                                 Id = r.Id,
                                 Nombre = r.Nombre,
                                 EsCorrecto = false
                             })
                             .Take(3)
                             .OrderBy(c => Guid.NewGuid())
                             .ToArrayAsync();

                        var respuestaCorrecta = await _db.Clases
                            .Where(c => c.Nombre == preguntaAl.Nombre)
                            .Select(r => new RespuestasResponse
                            {
                                Id = r.Id,
                                Nombre = r.Nombre,
                                EsCorrecto = true
                            })
                            .FirstOrDefaultAsync();

                        var respuestasFinal = respuestas.ToList();

                        respuestasFinal.Add(respuestaCorrecta);
                        #endregion

                        #region Cambiar Posición del Array
                        Random r = new Random();
                        var respuestaMixta = respuestasFinal.OrderBy(x => r.Next()).ToArray();
                        #endregion

                        pregunta.Respuestas = respuestaMixta;

                        preguntas.Add(pregunta);
                    }

                    evaluacion.Evaluacion = preguntas;

                }
                else
                {
                    throw Error(_stringLocalizer["RetoNoEncontrado"], 400);
                }
            }

            return evaluacion;
        }

        [HttpPost("puntuarEvaluacion")]
        public async Task<string> PuntuarEvaluacionInscritoReto([FromBody] PuntuarEvaluacionRequest request)
        {

            var evaluacion = new EvaluacionReto();

            var reto = await _db.Retos.Where(r => r.Id == request.IdReto).SingleOrDefaultAsync();

            if (reto != null)
            {
                reto.Estado = "E";

                await _db.SaveChangesAsync();

                if (request.Nota <= 3)
                {
                    evaluacion.IdReto = request.IdReto;
                    evaluacion.Nota = request.Nota;
                    evaluacion.Estado = "Desaprobado";

                    _db.EvaluacionRetos.Add(evaluacion);

                    await _db.SaveChangesAsync();

                }
                else
                {
                    evaluacion.IdReto = request.IdReto;
                    evaluacion.Nota = request.Nota;
                    evaluacion.Estado = "Aprobado";

                    _db.EvaluacionRetos.Add(evaluacion);

                    await _db.SaveChangesAsync();
                }
            }
            return _stringLocalizer["Evaluado", 200];
        }
    }
}
