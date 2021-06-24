using System;
namespace surtus_api_restful.Models
{
    public class Modulo : IIdAutogenerado<long>
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public string Nivel { get; set; }
    }
}
