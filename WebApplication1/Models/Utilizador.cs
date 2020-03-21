using System;
using System.Collections.Generic;
using System.Web;

namespace WebApplication1.Models
{
    public partial class Utilizador
    {
       
        public Utilizador()
        {
            Aluguer = new HashSet<Aluguer>();
            Artigo = new HashSet<Artigo>();
            Venda = new HashSet<Venda>();
        }

        public string Email { get; set; }
        public int Cc { get; set; }
        public string Nome { get; set; }
        public string Password { get; set; }
        public long ContaBancaria { get; set; }
        public double Pontuacao { get; set; }
        public string Tipo { get; set; }
        public int Telemovel { get; set; }
        public bool Estado { get; set; }
        public string Administrador { get; set; }
        public string CodPostal { get; set; }

        public virtual Administrador AdministradorNavigation { get; set; }
        public virtual Localizacao CodPostalNavigation { get; set; }
        public virtual ICollection<Aluguer> Aluguer { get; set; }
        public virtual ICollection<Artigo> Artigo { get; set; }
        public virtual ICollection<Venda> Venda { get; set; }
        public int NPorta { get; internal set; }
        public string Rua { get; internal set; }
    }
}
