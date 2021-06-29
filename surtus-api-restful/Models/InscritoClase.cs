using System;
namespace surtus_api_restful.Models
{
    public class InscritoClase
    {
        public long IdInscrito { get; set; }
        public long IdClase { get; set; }
        public bool Visto { get; set; }

        public virtual Clase Clase { get; set; }
        public virtual Inscripcion Inscripcion { get; set; }
    }
}
