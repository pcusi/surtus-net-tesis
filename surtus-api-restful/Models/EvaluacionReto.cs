using System;
namespace surtus_api_restful.Models
{
    public class EvaluacionReto
    {
        public long IdReto { get; set; }
        public int Nota { get; set; }
        public string Estado { get; set; }
        public long FechaFinalizacion { get; set; }

        public virtual Reto Reto { get; set; }
    }
}
