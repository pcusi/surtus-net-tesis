using System;
namespace surtus_api_restful.Models
{
    public class Inscripcion : IIdAutogenerado<long>
    {
        public long Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string Nivel { get; set; }
    }
}
