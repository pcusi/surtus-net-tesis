using System;
namespace surtus_api_restful.Dtos.Responses.Reto
{
    public class RespuestasResponse
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public bool EsCorrecto { get; set; }
    }
}
