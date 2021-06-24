using System;
using System.Collections.Generic;

namespace surtus_api_restful.Models
{
    public class Reto : IIdAutogenerado<long>
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public long IdModulo { get; set; }
        public long IdInscrito { get; set; }
        public long FechaCreacion { get; set; }
        public string Estado { get; set; }

        public virtual Modulo Modulo { get; set; }
        public virtual Inscripcion Inscripcion { get; set; }
        public virtual ICollection<EvaluacionReto> EvaluacionRetos { get; set; } = new HashSet<EvaluacionReto>();
    }
}
