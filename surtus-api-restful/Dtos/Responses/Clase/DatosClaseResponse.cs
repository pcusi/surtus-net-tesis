using System;
namespace surtus_api_restful.Dtos.Responses.Clase
{
    public class DatosClaseResponse
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public string Video { get; set; }
        public long IdModulo { get; set; }
    }
}
