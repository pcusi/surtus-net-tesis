using System;
namespace surtus_api_restful.Dtos.Requests.Inscripcion
{
    public class IniciarSesionRequest
    {
        public string Usuario { get; set; }
        public string Clave { get; set; }
    }
}
