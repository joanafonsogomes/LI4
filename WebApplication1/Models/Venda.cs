using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Venda
    {
        public int IdVenda { get; set; }
        public int IdArtigo { get; set; }
        public string IdUtilizador { get; set; }
        public double Preco { get; set; }
        public string IdRent { get; set; }
        public int Estado { get; set; }
        public int Quantidade { get; set; }

        public virtual Artigo IdArtigoNavigation { get; set; }
        public virtual Utilizador IdUtilizadorNavigation { get; set; }
    }
}
