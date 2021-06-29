using System;
using System.Collections.Generic;

namespace surtus_api_restful.Models
{
    public class Clase : IIdAutogenerado<long>
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public string Video { get; set; }
        public long IdModulo { get; set; }

        public virtual Modulo Modulo { get; set;}
        public virtual ICollection<InscritoClase> InscritoClases { get; set; }
    }
}
