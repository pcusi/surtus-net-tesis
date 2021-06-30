using System;
namespace surtus_api_restful.Dtos.Responses.Inscripcion
{
    public class ParametrosInscritoResponse
    {
        public DatosInscritoResponse Inscrito { get; set; }
        public double Avance { get; set; }
    }
}
