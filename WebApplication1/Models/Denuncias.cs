using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Denuncias
    {
        public int IdDenuncia { get; set; }
        public string Descricao { get; set; }
        public string IdAutor { get; set; }
        public string IdDenunciado { get; set; }
        public string Administrador { get; set; }

        public virtual Administrador AdministradorNavigation { get; set; }
    }
}
