using System;
namespace surtus_api_restful.Dtos.Requests.Reto
{
    public class PuntuarEvaluacionRequest
    {
        public long IdReto { get; set; }
        public int Nota { get; set; }
    }
}
