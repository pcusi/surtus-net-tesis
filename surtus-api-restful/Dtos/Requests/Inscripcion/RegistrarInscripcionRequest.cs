using System;
namespace surtus_api_restful.Dtos.Requests.Inscripcion
{
    public class RegistrarInscripcionRequest
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
    }
}
