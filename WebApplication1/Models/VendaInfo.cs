using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace WebApplication1.Models
{

    public partial class VendaInfo
    {
        public VendaInfo()
        {
        }

        public int IdArtigo { get; set; }
        public int IdVenda { get; set; }
        public String NomeArtigo { get; set; }
        public double Preco { get; set; }
        public int Quantidade { get; set; }
        public string Imagem { get; set; }
        public String Tipo { get; set; }

        
        public string Email { get; set; }
        public string Nome { get; set; }
        public int Telemovel { get; set; }
        public string CodPostal { get; set; }
        public int NPorta { get; internal set; }
        public string Rua { get; internal set; }
    }
}
