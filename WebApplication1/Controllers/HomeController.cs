using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace WebApplication1.Controllers
{

    public class HomeController : Controller
    {
        private Model model = new Model();
        private readonly ILogger<HomeController> _logger;
        private ContaController conta = new ContaController();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public ActionResult Index()
        {
            var local = (from x in model.Artigo select x);
            List<Artigo> res = local.ToList<Artigo>();
            return View(res);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Image()
        {
            return View();
        }

        public IActionResult UploadPic()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }

        public IActionResult ErrorSearch()
        {
            return View();
        }

        public ActionResult Details(int idArtigo)
        {
            Console.WriteLine(idArtigo);

            Artigo ss = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(idArtigo));

            var cenas = ss.IdArtigo;
            Console.WriteLine(cenas);

            var comentarios = from x in model.Comentarios where (x.IdArtigo.Equals(idArtigo)) select x;
            List<Comentarios> c = comentarios.ToList<Comentarios>();

            foreach(Comentarios a in c)
            {
                ss.Comentarios.Add(a);
            }


            return View(ss);

        }


        public ActionResult MaiorClassificacao()
        {
            var local = (from x in model.Artigo select x);
            List<Artigo> res = local.ToList<Artigo>();

            Dictionary<Artigo,double> mp = new Dictionary<Artigo, double>();
            for (int i = 0; i < res.Count; i++)
            {
                Artigo a = (from x in model.Artigo where (x.IdArtigo == res[i].IdArtigo) select x).ToList().ElementAt<Artigo>(0); ;
                mp.Add(a, a.Pontuacao );

            }

            if (mp.Count >= 20)
            {
                mp = mp.OrderBy(p => p.Value).Reverse().Take(20).ToDictionary(p => p.Key, p => p.Value);
            }
            else
            {
                mp = mp.OrderBy(p => p.Value).Reverse().ToDictionary(p => p.Key, p => p.Value);
            }
            List<Artigo> res2 = mp.Keys.ToList();
            return View(res2);

        }


        public ActionResult MaisVendidos()
        {
            var local = (from x in model.Artigo select x);
            List<Artigo> res = local.ToList<Artigo>();
            
            Dictionary<Artigo, int> mp = new Dictionary<Artigo,int>();
            for(int i = 0; i < res.Count; i++)
            {
                    int a = (from x in model.Aluguer where (x.IdArtigo == res[i].IdArtigo) select x).ToList().Count;
                    int v = (from x in model.Venda where (x.IdArtigo == res[i].IdArtigo) select x).ToList().Count;
                    mp.Add(res[i], a + v);
               
            }

            if (mp.Count >= 20)
            {
                mp=mp.OrderBy(p => p.Value).Reverse().Take(20).ToDictionary(p => p.Key, p => p.Value);
            }
            else
            {
                mp=mp.OrderBy(p => p.Value).Reverse().ToDictionary(p => p.Key, p => p.Value);
            }
            List<Artigo> res2 = mp.Keys.ToList();
            return View(res2);

        }
        public ActionResult Novidades()
        {
            var local = (from x in model.Artigo select x);
            List<Artigo> res = local.ToList<Artigo>();
            res.Reverse();
            List<Artigo> res2 = new List<Artigo>();
            if (res.Count >= 20)
            {
                for (int i = 0; i < 20; i++)
                {
                    res2.Add(res[i]);
                }
            }
            else
            {
                for (int i = 0; i < res.Count; i++)
                {
                    res2.Add(res[i]);
                }
            }
            return View(res2);
            
        }



        public ActionResult VerInfo()
        {
            string user = Helpers.CacheController.utilizador;

            var info = (from m in model.Utilizador where (m.Email.Equals(user)) select m);
            List<Utilizador> lista = info.ToList<Utilizador>();

            Utilizador res = lista.ElementAt(0);
            var local = (from m in model.Localizacao where (m.CodigoPostal.Equals(res.CodPostal)) select m);

            List<Localizacao> list = local.ToList<Localizacao>();
            Localizacao l = list.ElementAt(0);

            res.CodPostalNavigation = l;

            return View(res);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login(string username, string password)
        {
            return conta.Login(username, password);
        }
        public ActionResult Procurei()
        {
            var data = "info que eu preciso guardar para mandar";
            return View(data);
        }


        public ActionResult SearchArtigos(string search)
        {
            var local = (from x in model.Artigo where (x.Nome.Contains(search)) select x);
            var local2 = (from x in model.Artigo where (x.Etiquetas.Contains(search)) select x);

            if (local.ToList().Count > 0 || local2.ToList().Count > 0)
            {
                List<Artigo> lista = local.ToList<Artigo>();
                List<Artigo> lista2 = local2.ToList<Artigo>();
                List<Artigo> listaUnion = lista.Union(lista2).ToList();

                return View(listaUnion);
            }

            else return RedirectToAction("ErrorSearch", "Home");
        }
        
  
            public ActionResult SearchCategoria(string categoria)
            {
                var local = (from x in model.Artigo where (x.Categoria == categoria) select x);
                if (local.ToList().Count > 0)
                {
                    List<Artigo> list = local.ToList<Artigo>();
                    return View(list);
                }

                else return RedirectToAction("ErrorSearch", "Home");
            }

    }


}
