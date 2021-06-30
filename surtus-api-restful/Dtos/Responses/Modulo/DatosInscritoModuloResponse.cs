using System;
namespace surtus_api_restful.Dtos.Responses.Modulo
{
    public class DatosInscritoModuloResponse
    {
        public long IdModulo { get; set; }
        public long IdInscrito { get; set; }
        public decimal Avance { get; set; }
    }
}
