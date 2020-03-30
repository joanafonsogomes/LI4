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


        public IActionResult Index()
        {
            return View();
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

        public IActionResult ErrorSearch()
        {
            return View();
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
