using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class UtilizadorController : Controller
    {
        private Model model = new Model();

        public ActionResult Index()
        {
            return View();
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
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpPost]
        public ActionResult AdicionarCliente(string email, int cc, string nome, string password, long contaBancaria, string tipo, int telemovel, string rua, int nPorta, string codigoPostal, string freguesia, string distrito){
            if (ModelState.IsValid)
            {
                Utilizador utilizador= new Utilizador()
            {
                Email= email,
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

        public ActionResult NovoProduto (string nome, float preco, string modo, int quantidade,string categoria,string etiquetas)
        {
            string user= Helpers.CacheController.utilizador;
            var artigos = (from m in model.Artigo  select m);
            List<Artigo> lista = artigos.ToList<Artigo>();
            int i = lista.Count;

            /**
             * Serve para apagar artigos
             * CUIDADO A USA-LA
            var art = (from u in model.Artigo where (u.IdArtigo == i) select u);
            Artigo a = art.ToList().ElementAt<Artigo>(0);
            model.Artigo.Remove(a);
            */

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
                IdDono = user
            };

            if (ModelState.IsValid)
            {
                model.Artigo.Add(artigo);
               // model.Artigo.Remove(a);
                model.SaveChanges();
            }

            return RedirectToAction("VerArtigos", "Utilizador");
        }
        


            public ActionResult verArtigos()
        {
           string user = Helpers.CacheController.utilizador;

            var artigos = (from m in model.Artigo where (m.IdDono == user) select m);
            List<Artigo> lista = artigos.ToList<Artigo>();

            return View(lista);
        }

        //Está mal redirecionada , ainda não funciona
        public ActionResult Alterar (int? idArtigos)
        {
            Helpers.CacheController.idArtigo = idArtigos.Value;

           return View("RequisitarServico");
        }

        public ActionResult NovoArtigo()
        {
            return View("NovoArtigo");
        }


        [HttpPost]
        public ActionResult AlterarDadosUtilizador(string email, string password, long contaBancaria, int telemovel, string codigoPostal, string freguesia, string rua, int nPorta, string distrito)
        {

            if (ModelState.IsValid)
            {

                var user = (from x in model.Utilizador where (x.Email == email) select x);
                Utilizador us = user.ToList().ElementAt<Utilizador>(0);

                var local = (from x in model.Localizacao where (x.CodigoPostal == codigoPostal) select x);

                if (local.ToList().Count==0)
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

    }
}
