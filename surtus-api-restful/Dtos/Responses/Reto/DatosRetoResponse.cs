using System;
namespace surtus_api_restful.Dtos.Responses.Reto
{
    public class DatosRetoResponse
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public long IdModulo { get; set; }
        public long IdInscrito { get; set; }
        public string Estado { get; set; }
        public DatosEvaluacionRetoResponse[] Evaluacion { get; set; }
    }
}
