using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Administrador
    {
        public Administrador()
        {
            Denuncias = new HashSet<Denuncias>();
            Utilizador = new HashSet<Utilizador>();
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public long ContaBancaria { get; set; }

        public virtual ICollection<Denuncias> Denuncias { get; set; }
        public virtual ICollection<Utilizador> Utilizador { get; set; }
    }
}
