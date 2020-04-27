using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Comentarios
    {
        public int IdComentario { get; set; }
        public string Descricao { get; set; }
        public string IdUtilizador { get; set; }
        public int IdArtigo { get; set; }

        public virtual Artigo IdArtigoNavigation { get; set; }
        public virtual Utilizador IdUtilizadorNavigation { get; set; }
    }
}
