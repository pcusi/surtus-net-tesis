using System;
namespace surtus_api_restful.Dtos.Responses.Clase
{
    public class RegistrarClaseRequest
    {
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public long IdModulo { get; set; }
    }
}
