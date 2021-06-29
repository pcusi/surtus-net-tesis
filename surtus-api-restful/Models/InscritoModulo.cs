using System;
namespace surtus_api_restful.Models
{
    public class InscritoModulo
    {
        public long IdModulo { get; set; }
        public long IdInscrito { get; set; }
        public decimal Avance { get; set; }

        public virtual Modulo Modulo { get; set; }
        public virtual Inscripcion Inscripcion { get; set; }
    }
}
