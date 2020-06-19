using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CompanyController : Controller
    {

        private Model model = new Model();

        public ActionResult About() { 
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }


        public IActionResult ErrorSearch()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View();
        }

        public ActionResult Perfil()
        {
            string uti = Helpers.CacheController.utilizador;
            Utilizador std = model.Utilizador.Where(x => x.Email.Equals(uti)).FirstOrDefault();
            var local = (from m in model.Localizacao where (m.CodigoPostal.Equals(std.CodPostal)) select m);

            List<Localizacao> list = local.ToList<Localizacao>();
            Localizacao l = list.ElementAt(0);

            std.CodPostalNavigation = l;

            var vouchers = from alu in model.Voucher where (alu.IdUtilizador.Equals(uti)) select alu;

            List<Voucher> v = vouchers.ToList<Voucher>();

            foreach (Voucher a in v)
            {
                std.Voucher.Add(a);
            }

            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View(std);
        }

        public IActionResult NovoArtigo()
        {
            string user = Helpers.CacheController.utilizador;

            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View();
        }


        [HttpPost]
        public IActionResult NovoArtigo(List<IFormFile> file, string nome, float preco, string modo, int quantidade, string categoria, string etiquetas, string descricao)
        {
            foreach (IFormFile cenas in file)
            {
                Console.WriteLine(cenas.FileName);
            }
            try
            {

                string user = Helpers.CacheController.utilizador;





                var artigos = (from m in model.Artigo select m);
                List<Artigo> lista = artigos.ToList<Artigo>();
                Artigo a = lista[lista.Count - 1];
                int i = a.IdArtigo;
                Console.WriteLine("index" + i);
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
                    Estado = 0,
                    Pontuacao = 0,
                    NumeroVotos = 0,
                    PontucaoAcumulada = 0,
                    Descricao = descricao,
                    IdDono = user,
                };

                string fileName = "";
                string name = "";

                if (file.Count == 1)
                {
                    Console.WriteLine("entrei no primeiro ciclo.");
                    fileName = file[0].FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file[0].CopyTo(fileStream);
                    }
                }
                else
                {
                    Console.WriteLine("entrei no segundo ciclo.");
                    int j;
                    int count = file.Count;
                    for (j = 0; j < count; j++)
                    {
                        IFormFile f = file[j];
                        name = f.FileName;
                        fileName += name;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", name);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            f.CopyTo(fileStream);
                        }
                        if (i != count - 1)
                        {
                            fileName += " ";
                        }

                    }
                    Console.WriteLine(fileName);
                }

                artigo.Imagem = fileName;


                if (ModelState.IsValid)
                {
                    model.Artigo.Add(artigo);

                    model.SaveChanges();
                }

            }
            catch (Exception)
            {
                return Content("Could not create item...");
            }

            return RedirectToAction("VerArtigos", "Utilizador");
        }

        public ActionResult verArtigos()
        {
            string user = Helpers.CacheController.utilizador;

            var artigos = (from m in model.Artigo where (m.IdDono == user) select m);
            List<Artigo> lista = artigos.ToList<Artigo>();

            Utilizador uti = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = uti.Notificacoes;

            ViewData["noti"] = notifications;

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

            Utilizador uti = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = uti.Notificacoes;

            ViewData["noti"] = notifications;

            return View(res);
        }


        public ActionResult Alterar(int idArtigo)
        {

            Artigo ss = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(idArtigo));

            Helpers.CacheController.IdArtigo = idArtigo;
            var cenas = ss.IdArtigo;

            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;


            return View(ss);
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
                u.Password = MyHelpers.HashPassword(u.Password);
                model.SaveChanges();
            }

            return RedirectToAction("VerInfo", "Utilizador");
        }

        /**
        * Permite vizualisar a view que permite alterar a conta Bancaria e efetua a sua mudança
        * */


        public ActionResult CBanc()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("CBanc");
        }


        public ActionResult Password()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("Password");
        }


        [HttpPost]
        public ActionResult CBanc(long conta)
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
                u.ContaBancaria = conta;
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
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("Telemovel");
        }

        [HttpPost]
        public ActionResult Telemovel(int telemovel)
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
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("CPostal");
        }


        [HttpPost]
        public ActionResult CPostal(string codigoPostal)
        {
            string mail = Helpers.CacheController.utilizador;
            if (ModelState.IsValid)
            {

                Utilizador std = model.Utilizador.Where(x => x.Email.Equals(mail)).FirstOrDefault();

                var local = (from x in model.Localizacao where (x.CodigoPostal == codigoPostal) select x);

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
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

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
            return RedirectToAction("", "Utilizador");
        }
        /**
        * Permite vizualisar a view que permite alterar a freguesia e efetua a sua mudança
        * */

        public ActionResult Freguesia()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

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
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

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
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;
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


        public ActionResult SearchArtigos(string search)
        {
            var local = (from x in model.Artigo where (x.Nome.Contains(search)) select x);
            var local2 = (from x in model.Artigo where (x.Etiquetas.Contains(search)) select x);

            if (local.ToList().Count > 0 || local2.ToList().Count > 0)
            {
                List<Artigo> lista = local.ToList<Artigo>();
                List<Artigo> lista2 = local2.ToList<Artigo>();
                List<Artigo> listaUnion = lista.Union(lista2).ToList();

                string user = Helpers.CacheController.utilizador;
                Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
                int notifications = u.Notificacoes;

                ViewData["noti"] = notifications;

                return View(listaUnion);
            }

            else return RedirectToAction("ErrorSearch", "Home");
        }

        public ActionResult AlteraNome()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("AlteraNome");
        }


        [HttpPost]
        public ActionResult AlteraNome(string nome)
        {

            int id = Helpers.CacheController.IdArtigo;
            if (ModelState.IsValid)
            {

                Artigo std = model.Artigo.Where(x => x.IdArtigo.Equals(id)).FirstOrDefault();


                Artigo a = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(std.IdArtigo));
                a.Nome = nome;
                a.Preco = std.Preco;
                a.Modo = std.Modo;
                a.Quantidade = std.Quantidade;
                a.Categoria = std.Categoria;
                a.Etiquetas = std.Etiquetas;
                a.Estado = std.Estado;
                a.Descricao = std.Descricao;
                a.Imagem = std.Imagem;
                model.SaveChanges();
            }
            return RedirectToAction("verArtigos", "Utilizador");
        }


        public ActionResult AlteraPreco()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("AlteraPreco");
        }


        [HttpPost]
        public ActionResult AlteraPreco(float preco)
        {

            int id = Helpers.CacheController.IdArtigo;
            if (ModelState.IsValid)
            {

                Artigo std = model.Artigo.Where(x => x.IdArtigo.Equals(id)).FirstOrDefault();


                Artigo a = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(std.IdArtigo));
                a.Nome = std.Nome;
                a.Preco = preco;
                a.Modo = std.Modo;
                a.Quantidade = std.Quantidade;
                a.Categoria = std.Categoria;
                a.Etiquetas = std.Etiquetas;
                a.Estado = std.Estado;
                a.Descricao = std.Descricao;
                a.Imagem = std.Imagem;
                model.SaveChanges();
            }
            return RedirectToAction("verArtigos", "Utilizador");
        }


        public ActionResult AlteraModo()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("AlteraModo");
        }


        [HttpPost]
        public ActionResult AlteraModo(string modo)
        {

            int id = Helpers.CacheController.IdArtigo;
            if (ModelState.IsValid)
            {

                Artigo std = model.Artigo.Where(x => x.IdArtigo.Equals(id)).FirstOrDefault();


                Artigo a = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(std.IdArtigo));
                a.Nome = std.Nome;
                a.Preco = std.Preco;
                a.Modo = modo;
                a.Quantidade = std.Quantidade;
                a.Categoria = std.Categoria;
                a.Etiquetas = std.Etiquetas;
                a.Estado = std.Estado;
                a.Descricao = std.Descricao;
                a.Imagem = std.Imagem;
                model.SaveChanges();
            }
            return RedirectToAction("verArtigos", "Utilizador");
        }



        public ActionResult AlteraCategoria()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("AlteraCategoria");
        }


        [HttpPost]
        public ActionResult AlteraCategoria(string categoria)
        {

            int id = Helpers.CacheController.IdArtigo;
            if (ModelState.IsValid)
            {

                Artigo std = model.Artigo.Where(x => x.IdArtigo.Equals(id)).FirstOrDefault();


                Artigo a = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(std.IdArtigo));
                a.Nome = std.Nome;
                a.Preco = std.Preco;
                a.Modo = std.Modo;
                a.Quantidade = std.Quantidade;
                a.Categoria = categoria;
                a.Etiquetas = std.Etiquetas;
                a.Estado = std.Estado;
                a.Descricao = std.Descricao;
                a.Imagem = std.Imagem;
                model.SaveChanges();
            }
            return RedirectToAction("VerArtigos", "Utilizador");
        }


        public ActionResult AlteraQuantidade()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("AlteraQuantidade");
        }


        [HttpPost]
        public ActionResult AlteraQuantidade(int quantidade)
        {

            int id = Helpers.CacheController.IdArtigo;
            if (ModelState.IsValid)
            {

                Artigo std = model.Artigo.Where(x => x.IdArtigo.Equals(id)).FirstOrDefault();


                Artigo a = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(std.IdArtigo));
                a.Nome = std.Nome;
                a.Preco = std.Preco;
                a.Modo = std.Modo;
                a.Quantidade = quantidade;
                a.Categoria = std.Categoria;
                a.Etiquetas = std.Etiquetas;
                a.Estado = std.Estado;
                a.Descricao = std.Descricao;
                a.Imagem = std.Imagem;
                model.SaveChanges();
            }
            return RedirectToAction("verArtigos", "Utilizador");
        }


        public ActionResult AlteraDescricao()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("AlteraDescricao");
        }

        [HttpPost]
        public ActionResult AlteraDescricao(string descricao)
        {

            int id = Helpers.CacheController.IdArtigo;
            if (ModelState.IsValid)
            {

                Artigo std = model.Artigo.Where(x => x.IdArtigo.Equals(id)).FirstOrDefault();


                Artigo a = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(std.IdArtigo));
                a.Nome = std.Nome;
                a.Preco = std.Preco;
                a.Modo = std.Modo;
                a.Quantidade = std.Quantidade;
                a.Categoria = std.Categoria;
                a.Etiquetas = std.Etiquetas;
                a.Estado = std.Estado;
                a.Descricao = descricao;
                a.Imagem = std.Imagem;
                model.SaveChanges();
            }
            return RedirectToAction("verArtigos", "Utilizador");
        }


        public ActionResult AlteraEstado()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("AlteraEstado");
        }


        [HttpPost]
        public ActionResult AlteraEstado(int estado)
        {

            int id = Helpers.CacheController.IdArtigo;
            if (ModelState.IsValid)
            {

                Artigo std = model.Artigo.Where(x => x.IdArtigo.Equals(id)).FirstOrDefault();


                Artigo a = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(std.IdArtigo));
                a.Nome = std.Nome;
                a.Preco = std.Preco;
                a.Modo = std.Modo;
                a.Quantidade = std.Quantidade;
                a.Categoria = std.Categoria;
                a.Etiquetas = std.Etiquetas;
                a.Estado = estado;
                a.Descricao = std.Descricao;
                a.Imagem = std.Imagem;
                model.SaveChanges();
            }
            return RedirectToAction("verArtigos", "Utilizador");
        }



        public ActionResult AlteraEtiquetas()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("AlteraEtiquetas");
        }

        [HttpPost]
        public ActionResult AlteraEtiquetas(string etiquetas)
        {

            int id = Helpers.CacheController.IdArtigo;
            if (ModelState.IsValid)
            {

                Artigo std = model.Artigo.Where(x => x.IdArtigo.Equals(id)).FirstOrDefault();


                Artigo a = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(std.IdArtigo));
                a.Nome = std.Nome;
                a.Preco = std.Preco;
                a.Modo = std.Modo;
                a.Quantidade = std.Quantidade;
                a.Categoria = std.Categoria;
                a.Etiquetas = etiquetas;
                a.Estado = std.Estado;
                a.Descricao = std.Descricao;
                a.Imagem = std.Imagem;
                model.SaveChanges();
            }
            return RedirectToAction("verArtigos", "Utilizador");
        }



        public ActionResult AlteraImagem()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("AlteraImagem");
        }


        [HttpPost]
        public ActionResult AlteraImagem(List<IFormFile> file)
        {
            var artigos = (from m in model.Artigo select m);
            List<Artigo> lista = artigos.ToList<Artigo>();
            int i = lista.Count;

            i++;
            string fileName = "";
            string name = "";

            if (file.Count == 1)
            {
                Console.WriteLine("entrei no primeiro ciclo.");
                fileName = file[0].FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }
            }
            else
            {
                Console.WriteLine("entrei no segundo ciclo.");
                int j;
                int count = file.Count;
                for (j = 0; j < count; j++)
                {
                    IFormFile f = file[j];
                    name = f.FileName;
                    fileName += name;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", name);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        f.CopyTo(fileStream);
                    }
                    if (i != count - 1)
                    {
                        fileName += " ";
                    }

                }
                Console.WriteLine(fileName);
            }
            int id = Helpers.CacheController.IdArtigo;

            Artigo std = model.Artigo.Where(x => x.IdArtigo.Equals(id)).FirstOrDefault();


            Artigo a = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(std.IdArtigo));
            a.Nome = std.Nome;
            a.Preco = std.Preco;
            a.Modo = std.Modo;
            a.Quantidade = std.Quantidade;
            a.Categoria = std.Categoria;
            a.Etiquetas = std.Etiquetas;
            a.Estado = std.Estado;
            a.Descricao = std.Descricao;
            a.Imagem = fileName;
            model.SaveChanges();
            //model.Artigo.Update(a);
            // model.Artigo.Add(a);

            return RedirectToAction("verArtigos", "Utilizador");
        }

       
        public ActionResult Cat()
        {
            string us = Helpers.CacheController.utilizador;
            var art = from x in model.Artigo where x.IdDono.Equals(us) select x;
            List<Artigo> res = art.ToList<Artigo>();
            return View(res);
        }






        public ActionResult Index()
        {
            string user = Helpers.CacheController.utilizador;
            SpecialIndexes special = new SpecialIndexes();
            special.Email = user;

            //get artigos
            var local = from x in model.Artigo where (x.IdDono.Equals(user)) select x;
            List<Artigo> res = local.ToList<Artigo>();


            Dictionary<Artigo, int> mp = new Dictionary<Artigo, int>();

            var d = from x in model.Denuncias select x;
            List<Denuncias> denuncias = d.ToList<Denuncias>();
            List<Denuncias> denunFinal = new List<Denuncias>();
            List<Denuncias> resDenuncias = new List<Denuncias>();


            //artigos + vendidos
            for (int i = 0; i < res.Count(); i++)
            {
                int v = (from x in model.Venda where (x.IdArtigo == res[i].IdArtigo) select x).ToList().Count;
                mp.Add(res[i], v);
            }

            if (mp.Count >= 5)
            {
                mp = mp.OrderBy(p => p.Value).Reverse().Take(5).ToDictionary(p => p.Key, p => p.Value);
            }
            else
            {
                mp = mp.OrderBy(p => p.Value).Reverse().ToDictionary(p => p.Key, p => p.Value);
            }
            List<Artigo> res2 = mp.Keys.ToList();

            //denuncias dos artigos da empresa (5 + recentes)
            foreach (Denuncias de in denuncias)
            {
                foreach (Artigo a in res2)
                {
                    if (de.IdArtigo == a.IdArtigo)
                    {
                        denunFinal.Add(de);
                    }
                }
            }
            denunFinal.Reverse();
            if (denunFinal.Count >= 5)
            {
                resDenuncias = denunFinal.Take(5).ToList();
            }
            else
            {
                resDenuncias = denunFinal;
            }

            //comentários dos artigos da empresa (5 + recentes)
            var c = from x in model.Comentarios select x;
            List<Comentarios> comentarios = c.ToList<Comentarios>();
            List<Comentarios> comFinal = new List<Comentarios>();
            List<Comentarios> resComentarios = new List<Comentarios>();

            foreach (Comentarios co in comentarios)
            {
                foreach (Artigo a in res)
                {
                    if (co.IdArtigo == a.IdArtigo)
                    {
                        comFinal.Add(co);
                    }
                }
            }
            comFinal.Reverse();
            if (comFinal.Count >= 5)
            {
                resComentarios = comFinal.Take(5).ToList();
            }
            else
            {
                resComentarios = comFinal;
            }

            //lista de vendas (15 elementos)

            var vendas = from ven in model.Venda where ven.IdUtilizador == user && ven.Estado == 1 select ven;
            List<Venda> lista = vendas.ToList<Venda>();
            List<Venda> resVendas = new List<Venda>();
            lista.Reverse();
            if (lista.Count >= 15)
            {
                resVendas = lista.Take(15).ToList();
            }
            else{
                resVendas = lista;
            }

   
            foreach(Artigo a in res2) special.Artigo.Add(a); //5 artigos + vendidos
            foreach (Denuncias a in resDenuncias) special.Denuncias.Add(a); //5 denuncias + recentes
            foreach (Comentarios a in resComentarios) special.Comentarios.Add(a); //5 comentarios + recentes
            foreach (Venda a in resVendas) special.Venda.Add(a); //15 últimas vendas
            model.SaveChanges();
            return View(special);

        }


    }
}

