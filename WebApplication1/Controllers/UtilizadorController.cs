using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Configuration;
using System.IO;
using System.Web;

namespace WebApplication1.Controllers
{
    public class UtilizadorController : Controller
    {
        private Model model = new Model();
        private IHostingEnvironment _environment;

        public UtilizadorController(IHostingEnvironment environment)
        {
            _environment = environment;
        }


        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var artigos = (from m in model.Artigo select m);
            List<Artigo> lista = artigos.ToList<Artigo>();
            var a = lista[id - 1];
            return View(a);
        }

        public ActionResult AlterarDadosUtilizador()
        {
            return View();
        }


        public ActionResult AdicionarCliente()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Descrição da página.";

            return View();
        }


        public ActionResult AdicionarCliente(string email, int cc, string nome, string password, long contaBancaria, string tipo, int telemovel, string rua, int nPorta, string codigoPostal, string freguesia, string distrito)
        {
            if (ModelState.IsValid)
            {
                Utilizador utilizador = new Utilizador()
                {
                    Email = email,
                    Cc = cc,
                    Nome = nome,
                    Password = password,
                    ContaBancaria = contaBancaria,
                    Pontuacao = 0,
                    Tipo = tipo,
                    Telemovel = telemovel,
                    Rua = rua,
                    NPorta = nPorta,
                    Estado = true,
                    Administrador = "admin@gmail.com",
                    CodPostal = codigoPostal
                };

                var local = (from x in model.Localizacao where (x.CodigoPostal == codigoPostal) select x);
                if (local.ToList().Count == 0)
                {

                    Localizacao localizacao = new Localizacao()
                    {
                        CodigoPostal = codigoPostal,
                        Freguesia = freguesia,
                        Distrito = distrito
                    };
                    model.Localizacao.Add(localizacao);
                }



                model.Utilizador.Add(utilizador);

                model.SaveChanges();
            }


            return RedirectToAction("Index", "Home");
        }

        public ActionResult UploadDocument()
        {
            return View();
        }

        [HttpGet]
        public IActionResult NovoArtigo()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult NovoArtigo(IFormFile file, string nome, float preco, string modo, int quantidade, string categoria, string etiquetas)
        {
            try
            {
                string user = Helpers.CacheController.utilizador;
                var artigos = (from m in model.Artigo select m);
                List<Artigo> lista = artigos.ToList<Artigo>();
                int i = lista.Count;

                i++;
                Artigo artigo = new Artigo() {
                IdArtigo = i++,
                Nome = nome,
                Preco = preco,
                Modo = modo,
                Quantidade = quantidade,
                Categoria = categoria,
                Etiquetas = etiquetas,
                Estado = false,
                IdDono = user,
                };

                var fileName = file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }


                artigo.Imagem = fileName;


                if (ModelState.IsValid)
                {
                    model.Artigo.Add(artigo);

                    model.SaveChanges();
                }

            }
            catch(Exception)
            {
                return Content("Could not create item...");
            }
            
            return RedirectToAction("VerArtigos", "Utilizador");
        }
        /*
        public void UploadFile(IFormFile file, int artigoId)
        {
            var fileName = file.FileName;
            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/uploads",fileName);
            using(var fileStream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            var artigos = (from m in model.Artigo select m);
            List<Artigo> lista = artigos.ToList<Artigo>();
            Console.WriteLine("Carago o id Artigo e");
            Console.WriteLine(artigoId);
            var artigo = lista[artigoId];
            artigo.Imagem = fileName;

        }*/


        public ActionResult verArtigos()
        {
            string user = Helpers.CacheController.utilizador;

            var artigos = (from m in model.Artigo where (m.IdDono == user) select m);
            List<Artigo> lista = artigos.ToList<Artigo>();

            return View(lista);
        }

        //Está mal redirecionada , ainda não funciona
        public ActionResult Alterar(int? idArtigos)
        {
            Helpers.CacheController.idArtigo = idArtigos.Value;

            return View("RequisitarServico");
        }

        [HttpPost]
        public ActionResult AlterarDadosUtilizador(string email, string password, long contaBancaria, int telemovel, string codigoPostal, string freguesia, string rua, int nPorta, string distrito)
        {

            if (ModelState.IsValid)
            {

                var user = (from x in model.Utilizador where (x.Email == email) select x);
                Utilizador us = user.ToList().ElementAt<Utilizador>(0);

                var local = (from x in model.Localizacao where (x.CodigoPostal == codigoPostal) select x);

                if (local.ToList().Count == 0)
                {

                    Localizacao localizacao = new Localizacao()
                    {
                        CodigoPostal = codigoPostal,
                        Freguesia = freguesia,
                        Distrito = distrito
                    };
                    model.Localizacao.Add(localizacao);
                    model.SaveChanges();
                }

                Utilizador u = model.Utilizador.FirstOrDefault(x => x.Email == email);
                u.Password = password;
                u.ContaBancaria = contaBancaria;
                u.Telemovel = telemovel;
                u.Rua = rua;
                u.NPorta = nPorta;
                u.CodPostal = codigoPostal;
                model.SaveChanges();

            }

            return RedirectToAction("Index", "Home");

        }

        public IActionResult Image()
        {
            return View();
        }

    }
    /*
    public ActionResult ProcurarArtigoPorEtiqueta(string etiqueta){
        if (ModelState.IsValid)
        {
            var local = (from x in model.Artigo where (x.Etiquetas.contains(etiqueta)) select x);
            local.ToList();
            return View("VerArtigos");
        }
    }

    public ActionResult ProcurarArtigoPorCategoria(string categoria){
        if(ModelState.IsValid)
        {
            var local = (from x in model.Artigo where (x.Categoria == categoria) select x);
            local.ToList();   
            return View("VerArtigos");
        }
    }

    public ActionResult ProcurarArtigoPorNome(string nome){
        if(ModelState.IsValid)
        {
            var local = (from x in model.Artigo where (x.Nome.contains(nome)) select x);
            local.ToList();    
            return View("VerArtigos");
        }
    }*/
}
