using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class SpecialIndexes
    {
        public SpecialIndexes()
        {
            Aluguer = new HashSet<Aluguer>();
            Artigo = new HashSet<Artigo>();
            Venda = new HashSet<Venda>();
            Comentarios = new HashSet<Comentarios>();
            Voucher = new HashSet<Voucher>();
            Denuncias = new HashSet<Denuncias>();
            Utilizador = new HashSet<Utilizador>();
        }

        public virtual ICollection<Aluguer> Aluguer { get; set; }
        public virtual ICollection<Artigo> Artigo { get; set; }
        public virtual ICollection<Venda> Venda { get; set; }
        public virtual ICollection<Comentarios> Comentarios { get; set; }
        public virtual ICollection<Voucher> Voucher { get; set; }
        public virtual ICollection<Denuncias> Denuncias { get; set; }
        public virtual ICollection<Utilizador> Utilizador { get; set; }
    }
}
