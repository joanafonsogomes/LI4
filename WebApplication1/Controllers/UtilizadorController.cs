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

        public ActionResult NovoProduto(string nome, float preco, string modo, int quantidade, string categoria, string etiquetas)
        {
            string user = Helpers.CacheController.utilizador;
            var artigos = (from m in model.Artigo select m);
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
            Artigo artigo = new Artigo()
            {
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

        //Está mal redirecionada , ainda não funciona
        public ActionResult Alterar(int? idArtigos)
        {
            Helpers.CacheController.idArtigo = idArtigos.Value;

            return View("RequisitarServico");
        }

        public ActionResult NovoArtigo()
        {
            return View("NovoArtigo");
        }

    
        /**
         * Permite vizualisar a view que permite alterar a passowrd e fetua a sua mudança
         * */
        public ActionResult Password()
        {
            return View("Password");
        }

        [HttpPost]
        public ActionResult Password(string password)
        {
            string mail = Helpers.CacheController.utilizador;
            if (ModelState.IsValid)
            {

                Utilizador std = model.Utilizador.Where(x => x.Email.Equals(mail)).FirstOrDefault();

                var local = (from x in model.Localizacao where (x.CodigoPostal == std.CodPostal) select x);

                Localizacao esq = model.Localizacao.Where(x => x.CodigoPostal.Equals(std.CodPostal)).FirstOrDefault();

                Localizacao l;

                if (local.ToList().Count == 0)
                {
                    l = new Localizacao();
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = esq.Distrito;

                    model.Localizacao.Add(l);
                    model.SaveChanges();

                }

                else
                {
                    l = model.Localizacao.FirstOrDefault(x => x.CodigoPostal == std.CodPostal);
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = esq.Distrito;

                    model.SaveChanges();
                }

                Utilizador u = model.Utilizador.FirstOrDefault(x => x.Email.Equals(std.Email));
                u.Password = password;
                u.ContaBancaria = std.ContaBancaria;
                u.Telemovel = std.Telemovel;
                u.Rua = std.Rua;
                u.NPorta = std.NPorta;
                u.CodPostal = std.CodPostal;
                u.CodPostalNavigation = l;
                model.SaveChanges();
            }
            return RedirectToAction("VerInfo", "Utilizador");
        }

        /**
        * Permite vizualisar a view que permite alterar a conta Bancaria e efetua a sua mudança
        * */
        public ActionResult CBanc()
        {
            return View("CBanc");
        }

        [HttpPost]
        public ActionResult CBanc(long banc)
        {
            string mail = Helpers.CacheController.utilizador;
            if (ModelState.IsValid)
            {

                Utilizador std = model.Utilizador.Where(x => x.Email.Equals(mail)).FirstOrDefault();

                var local = (from x in model.Localizacao where (x.CodigoPostal == std.CodPostal) select x);

                Localizacao esq = model.Localizacao.Where(x => x.CodigoPostal.Equals(std.CodPostal)).FirstOrDefault();

                Localizacao l;

                if (local.ToList().Count == 0)
                {
                    l = new Localizacao();
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = esq.Distrito;

                    model.Localizacao.Add(l);
                    model.SaveChanges();

                }

                else
                {
                    l = model.Localizacao.FirstOrDefault(x => x.CodigoPostal == std.CodPostal);
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = esq.Distrito;

                    model.SaveChanges();
                }

                Utilizador u = model.Utilizador.FirstOrDefault(x => x.Email.Equals(std.Email));
                u.Password = std.Password;
                u.ContaBancaria = banc;
                u.Telemovel = std.Telemovel;
                u.Rua = std.Rua;
                u.NPorta = std.NPorta;
                u.CodPostal = std.CodPostal;
                u.CodPostalNavigation = l;
                model.SaveChanges();
            }
            return RedirectToAction("VerInfo", "Utilizador");
        }

        /**
         * Permite vizualisar a view que permite alterar o numero de Telemovel e efetua a sua mudança
         * */
        public ActionResult Telemovel()
        {
            return View("Telemovel");
        }

        [HttpPost]
        public ActionResult Telemovel( int telemovel)
        {
            string mail = Helpers.CacheController.utilizador;
            if (ModelState.IsValid)
            {

                Utilizador std = model.Utilizador.Where(x => x.Email.Equals(mail)).FirstOrDefault();

                var local = (from x in model.Localizacao where (x.CodigoPostal == std.CodPostal) select x);

                Localizacao esq = model.Localizacao.Where(x => x.CodigoPostal.Equals(std.CodPostal)).FirstOrDefault();

                Localizacao l;

                if (local.ToList().Count == 0)
                {
                    l = new Localizacao();
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = esq.Distrito;

                    model.Localizacao.Add(l);
                    model.SaveChanges();

                }

                else
                {
                    l = model.Localizacao.FirstOrDefault(x => x.CodigoPostal == std.CodPostal);
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = esq.Distrito;

                    model.SaveChanges();
                }

                Utilizador u = model.Utilizador.FirstOrDefault(x => x.Email.Equals(std.Email));
                u.Password = std.Password;
                u.ContaBancaria = std.ContaBancaria;
                u.Telemovel = telemovel;
                u.Rua = std.Rua;
                u.NPorta = std.NPorta;
                u.CodPostal = std.CodPostal;
                u.CodPostalNavigation = l;
                model.SaveChanges();
            }
            return RedirectToAction("VerInfo", "Utilizador");
        }


        /**
         * Permite vizualisar a view que permite alterar o Codigo Postal e efetua a sua mudança
         * */
        public ActionResult CPostal()
        {
            return View("CPostal");
        }

        [HttpPost]
        public ActionResult CPostal(string codigoPostal)
        {
          string mail = Helpers.CacheController.utilizador;
            if (ModelState.IsValid)
            {

                Utilizador std = model.Utilizador.Where(x => x.Email.Equals(mail)).FirstOrDefault();

                var local = (from x in model.Localizacao where (x.CodigoPostal==codigoPostal) select x);
                
                Localizacao esq = model.Localizacao.Where(x => x.CodigoPostal.Equals(std.CodPostal)).FirstOrDefault();

                Localizacao l;

                if (local.ToList().Count == 0)
                {
                     l = new Localizacao();
                        l.CodigoPostal = codigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = esq.Distrito;

                    model.Localizacao.Add(l);
                    model.SaveChanges();

                }

                else
                {
                    l = model.Localizacao.FirstOrDefault(x => x.CodigoPostal == codigoPostal);
                    l.CodigoPostal = codigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = esq.Distrito;

                    model.SaveChanges();
                }

                Utilizador u = model.Utilizador.FirstOrDefault(x => x.Email.Equals(std.Email));
                u.Password = std.Password;
                u.ContaBancaria = std.ContaBancaria;
                u.Telemovel = std.Telemovel;
                u.Rua = std.Rua;
                u.NPorta = std.NPorta;
                u.CodPostal = std.CodPostal;
                u.CodPostalNavigation = l;

                model.SaveChanges();
            }
            return RedirectToAction("VerInfo", "Utilizador");
        }

        /**
      * Permite vizualisar a view que permite alterar o Distrito e efetua a sua mudança
      * */
        public ActionResult Distrito()
        {
            return View("Distrito");
        }

        [HttpPost]
        public ActionResult Distrito(string distrito)
        {
            string mail = Helpers.CacheController.utilizador;
            if (ModelState.IsValid)
            {

                Utilizador std = model.Utilizador.Where(x => x.Email.Equals(mail)).FirstOrDefault();

                var local = (from x in model.Localizacao where (x.CodigoPostal == std.CodPostal) select x);

                Localizacao esq = model.Localizacao.Where(x => x.CodigoPostal.Equals(std.CodPostal)).FirstOrDefault();

                Localizacao l;

                if (local.ToList().Count == 0)
                {
                    l = new Localizacao();
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = distrito;

                    model.Localizacao.Add(l);
                    model.SaveChanges();

                }

                else
                {
                    l = model.Localizacao.FirstOrDefault(x => x.CodigoPostal == std.CodPostal);
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = distrito;

                    model.SaveChanges();
                }

                Utilizador u = model.Utilizador.FirstOrDefault(x => x.Email.Equals(std.Email));
                u.Password = std.Password;
                u.ContaBancaria = std.ContaBancaria;
                u.Telemovel = std.Telemovel;
                u.Rua = std.Rua;
                u.NPorta = std.NPorta;
                u.CodPostal = std.CodPostal;
                u.CodPostalNavigation = l;

                model.SaveChanges();
            }
            return RedirectToAction("VerInfo", "Utilizador");
        }
        /**
        * Permite vizualisar a view que permite alterar a freguesia e efetua a sua mudança
        * */
        public ActionResult Freguesia() { 
        
            return View("Freguesia");
        }

        [HttpPost]
        public ActionResult Freguesia(string freguesia)
        {
            string mail = Helpers.CacheController.utilizador;
            if (ModelState.IsValid)
            {

                Utilizador std = model.Utilizador.Where(x => x.Email.Equals(mail)).FirstOrDefault();

                var local = (from x in model.Localizacao where (x.CodigoPostal == std.CodPostal) select x);

                Localizacao esq = model.Localizacao.Where(x => x.CodigoPostal.Equals(std.CodPostal)).FirstOrDefault();

                Localizacao l;

                if (local.ToList().Count == 0)
                {
                    l = new Localizacao();
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = freguesia;
                    l.Distrito = esq.Distrito;

                    model.Localizacao.Add(l);
                    model.SaveChanges();

                }

                else
                {
                    l = model.Localizacao.FirstOrDefault(x => x.CodigoPostal == std.CodPostal);
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = freguesia;
                    l.Distrito = esq.Distrito;

                    model.SaveChanges();
                }

                Utilizador u = model.Utilizador.FirstOrDefault(x => x.Email.Equals(std.Email));
                u.Password = std.Password;
                u.ContaBancaria = std.ContaBancaria;
                u.Telemovel = std.Telemovel;
                u.Rua = std.Rua;
                u.NPorta = std.NPorta;
                u.CodPostal = std.CodPostal;
                u.CodPostalNavigation = l;

                model.SaveChanges();
            }
            return RedirectToAction("VerInfo", "Utilizador");
        }


        /**
       * Permite vizualisar a view que permite alterar a rua e efetua a sua mudança
       * */
        public ActionResult Rua()
        {
            return View("Rua");
        }

        [HttpPost]
        public ActionResult Rua(string rua)
        {
            string mail = Helpers.CacheController.utilizador;
            if (ModelState.IsValid)
            {

                Utilizador std = model.Utilizador.Where(x => x.Email.Equals(mail)).FirstOrDefault();

                var local = (from x in model.Localizacao where (x.CodigoPostal == std.CodPostal) select x);

                Localizacao esq = model.Localizacao.Where(x => x.CodigoPostal.Equals(std.CodPostal)).FirstOrDefault();

                Localizacao l;

                if (local.ToList().Count == 0)
                {
                    l = new Localizacao();
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = esq.Distrito;

                    model.Localizacao.Add(l);
                    model.SaveChanges();

                }

                else
                {
                    l = model.Localizacao.FirstOrDefault(x => x.CodigoPostal == std.CodPostal);
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = esq.Distrito;

                    model.SaveChanges();
                }

                Utilizador u = model.Utilizador.FirstOrDefault(x => x.Email.Equals(std.Email));
                u.Password = std.Password;
                u.ContaBancaria = std.ContaBancaria;
                u.Telemovel = std.Telemovel;
                u.Rua = rua;
                u.NPorta = std.NPorta;
                u.CodPostal = std.CodPostal;
                u.CodPostalNavigation = l;
                model.SaveChanges();
            }
            return RedirectToAction("VerInfo", "Utilizador");
        }


        /**
        * Permite vizualisar a view que permite alterar o numero de porta e efetua a sua mudança
        * */
        public ActionResult NPorta()
        {
            return View("NPorta");
        }

        [HttpPost]
        public ActionResult NPorta(int nporta)
        {
            string mail = Helpers.CacheController.utilizador;
            if (ModelState.IsValid)
            {

                Utilizador std = model.Utilizador.Where(x => x.Email.Equals(mail)).FirstOrDefault();

                var local = (from x in model.Localizacao where (x.CodigoPostal == std.CodPostal) select x);

                Localizacao esq = model.Localizacao.Where(x => x.CodigoPostal.Equals(std.CodPostal)).FirstOrDefault();

                Localizacao l;

                if (local.ToList().Count == 0)
                {
                    l = new Localizacao();
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = esq.Distrito;

                    model.Localizacao.Add(l);
                    model.SaveChanges();

                }

                else
                {
                    l = model.Localizacao.FirstOrDefault(x => x.CodigoPostal == std.CodPostal);
                    l.CodigoPostal = esq.CodigoPostal;
                    l.Freguesia = esq.Freguesia;
                    l.Distrito = esq.Distrito;

                    model.SaveChanges();
                }

                Utilizador u = model.Utilizador.FirstOrDefault(x => x.Email.Equals(std.Email));
                u.Password = std.Password;
                u.ContaBancaria = std.ContaBancaria;
                u.Telemovel = std.Telemovel;
                u.Rua = std.Rua;
                u.NPorta = nporta;
                u.CodPostal = std.CodPostal;
                u.CodPostalNavigation = l;
                model.SaveChanges();
            }
            return RedirectToAction("VerInfo", "Utilizador");
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
}
