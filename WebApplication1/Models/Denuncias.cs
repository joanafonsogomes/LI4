using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Denuncias
    {
        public int IdDenuncia { get; set; }
        public string Descricao { get; set; }
        public string IdAutor { get; set; }
        public string Administrador { get; set; }

        public int IdArtigo { get; set; }


        public DateTime Data { get; set; }

        public virtual Administrador AdministradorNavigation { get; set; }

        public virtual Utilizador IdUtilizadorNavigation { get; set; }

        public virtual Artigo IdArtigoNavigation { get; set; }

    }
}
