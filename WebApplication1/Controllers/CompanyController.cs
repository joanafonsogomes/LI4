using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CompanyController : Controller
    {
        private Model model = new Model();
        public ActionResult Index()
        {
            var local = (from x in model.Utilizador select x);
            List<Utilizador> res = local.ToList<Utilizador>();
            foreach (Utilizador a in res)
            {
                var als = from x in model.Aluguer where x.IdUtilizador.Equals(a.Email) select x;
                a.Aluguer = als.ToList<Aluguer>();
                var arts = from g in model.Artigo where g.IdDono.Equals(a.Email) select g;
                List<Artigo> oi = arts.ToList<Artigo>();
                foreach (Artigo u in oi)
                {
                    var denun = from f in model.Denuncias where f.IdArtigo.Equals(u.IdArtigo) select f;
                    List<Denuncias> de = denun.ToList<Denuncias>();
                    foreach (Denuncias d in de) u.Denuncias.Add(d);
                    a.Artigo.Add(u);
                }
                var vens = from x in model.Venda where x.IdUtilizador.Equals(a.Email) select x;
                a.Venda = vens.ToList<Venda>();
                var vou = from x in model.Voucher where x.IdUtilizador.Equals(a.Email) select x;
                a.Voucher = vou.ToList<Voucher>();
            }
            return View(res);
        }

        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }


    }
}
