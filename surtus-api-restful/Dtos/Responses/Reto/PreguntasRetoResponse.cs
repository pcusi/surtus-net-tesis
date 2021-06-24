using System;
namespace surtus_api_restful.Dtos.Responses.Reto
{
    public class PreguntasRetoResponse
    {
        public string Pregunta { get; set; }
        public string Imagen { get; set; }
        public RespuestasResponse[] Respuestas { get; set; }
    }
}
