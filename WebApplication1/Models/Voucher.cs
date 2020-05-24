using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Voucher
    {
        public string Codigo { get; set; }
        public int Estado { get; set; }
        public double ValorOferta { get; set; }
        public string IdUtilizador { get; set; }

        public virtual Utilizador IdUtilizadorNavigation { get; set; }
    }
}
