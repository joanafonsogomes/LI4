using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Localizacao
    {
        public Localizacao()
        {
            Utilizador = new HashSet<Utilizador>();
        }

        public string CodigoPostal { get; set; }
        public string Freguesia { get; set; }
        public string Distrito { get; set; }

        public virtual ICollection<Utilizador> Utilizador { get; set; }
    }
}
