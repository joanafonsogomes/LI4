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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Specialized;
using WebApplication1.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace WebApplication1.Controllers
{
    public class UtilizadorController : Controller
    {
        private Model model = new Model();
        private IHostingEnvironment _environment;

        public DateTime SelectedDate { get; set; }
        public DateTime SelectedDateTo { get; set; }

        public UtilizadorController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public ActionResult Index()
        {
            var local = (from x in model.Artigo select x);
            List<Artigo> res = local.ToList<Artigo>();

            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View(res);
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

            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View();
        }

        public ActionResult Contact()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

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


        [Authorize]
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



        [Authorize]
        public ActionResult AdicionarCarrinho(int idArtigo)
        {

            Console.WriteLine(idArtigo);

            Artigo ss = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(idArtigo));

            var cenas = ss.IdArtigo;
            Console.WriteLine("> O artigo tem o id: " + cenas);

            string nower = Helpers.CacheController.utilizador;
            var vendas = (from vend in model.Venda where (vend.IdRent == nower && vend.Estado == 0) select vend);
            int teste = 0;
            List<Venda> lista = vendas.ToList<Venda>();
            int ind = 0;
            foreach(Venda a in lista)
            {
                ind++;
                if(a.IdArtigo == idArtigo)
                {
                    teste = 1; a.Quantidade++;
                    break;
                }
            }
            
            if (teste == 0)
            {
                Utilizador uti = model.Utilizador.FirstOrDefault(x => x.Email.Equals(nower));

                List<Venda> vendasTotal = (from a in model.Venda select a).ToList();

                List<int> indexes = new List<int>();

                foreach (Venda vt in vendasTotal)
                {
                    indexes.Add(vt.IdVenda);
                }

                int tamanho = (indexes.Max()) + 1;

                Venda vendinha = new Venda()
                {
                    IdVenda = tamanho,
                    IdArtigo = ss.IdArtigo,
                    IdUtilizador = ss.IdDono,
                    Preco = ss.Preco,
                    IdRent = nower,
                    Estado = 0,
                    Quantidade = 1,


                };
                lista.Add(vendinha);

                VendaInfo res = new VendaInfo()
                {
                    IdArtigo = ss.IdArtigo,
                    IdVenda = vendinha.IdVenda,
                    NomeArtigo = ss.Nome,
                    Preco = vendinha.Preco,
                    Quantidade = vendinha.Quantidade,
                    Imagem = ss.Imagem,
                    Email = nower,

                };

                uti.Venda.Add(vendinha);


                model.Venda.Add(vendinha);
                model.SaveChanges();

                string user = Helpers.CacheController.utilizador;
                Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
                int notifications = u.Notificacoes;

                ViewData["noti"] = notifications;

                return View(res);
            }
            else
            {
                Venda tete = lista.ElementAt<Venda>(ind-1);
                VendaInfo res = new VendaInfo()

                {
                    IdArtigo = ss.IdArtigo,
                    IdVenda = tete.IdVenda,
                    NomeArtigo = ss.Nome,
                    Preco = tete.Preco,
                    Quantidade = tete.Quantidade,
                    Imagem = ss.Imagem,
                    Email = nower,

                };
                model.SaveChanges();

                string user = Helpers.CacheController.utilizador;
                Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
                int notifications = u.Notificacoes;

                ViewData["noti"] = notifications;

                return View(res);
            }                 }

        public ActionResult MaiorClassificacao()
        {
            var local = (from x in model.Artigo select x);
            List<Artigo> res = local.ToList<Artigo>();

            Dictionary<Artigo, double> mp = new Dictionary<Artigo, double>();
            for (int i = 0; i < res.Count; i++)
            {
                Artigo a = (from x in model.Artigo where (x.IdArtigo == res[i].IdArtigo) select x).ToList().ElementAt<Artigo>(0); ;
                mp.Add(a, a.Pontuacao);

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

            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View(res2);

        }

        public ActionResult MaisVendidos()
        {
            var local = (from x in model.Artigo select x);
            List<Artigo> res = local.ToList<Artigo>();

            Dictionary<Artigo, int> mp = new Dictionary<Artigo, int>();
            for (int i = 0; i < res.Count; i++)
            {
                int a = (from x in model.Aluguer where (x.IdArtigo == res[i].IdArtigo) select x).ToList().Count;
                int v = (from x in model.Venda where (x.IdArtigo == res[i].IdArtigo) select x).ToList().Count;
                mp.Add(res[i], a + v);

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

            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

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

            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View(res2);

        }





        [HttpPost]
        public ActionResult AdicionarCliente(string email, int cc, string nome, string password, long contaBancaria, string tipo, int telemovel, string rua, int nPorta, string codigoPostal, string freguesia, string distrito)
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
                Estado = 0,
                Administrador = "admin@gmail.com",
                CodPostal = codigoPostal,
                NDenuncias = 0
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
            if (ModelState.IsValid)
            {

                utilizador.Password = MyHelpers.HashPassword(utilizador.Password);
                model.Utilizador.Add(utilizador);
                model.SaveChanges();
            }


            return RedirectToAction("Index", "Home");
        }

        public ActionResult UploadDocument()
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

            foreach (Comentarios a in c)
            {
                ss.Comentarios.Add(a);
            }

            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View(ss);

        }


        [Authorize]
        public ActionResult GoToDenuncias(int idArtigo)
        {
            return RedirectToAction("Denunciar", "Utilizador", new { IdArtigo = idArtigo } );
        }


        /**envia o pedido ao dono*/


        [HttpPost]
        public ActionResult Details(int IdArtigo, DateTime inicio, DateTime fim)
        {
            Artigo ss = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(IdArtigo));
            string dono = ss.IdDono;
            Utilizador donito = model.Utilizador.FirstOrDefault(x => x.Email.Equals(dono));

            List<Aluguer> alugueres = (from a in model.Aluguer select a).ToList();

            List<int> indexes = new List<int>();

            foreach(Aluguer a in alugueres){
                indexes.Add(a.IdAluguer);
            }

            int tamanho = (indexes.Max()) + 1;

            int m = (fim - inicio).Days;
            Aluguer novo = new Aluguer()
            {
                IdAluguer = tamanho,
                IdArtigo = IdArtigo,
                IdUtilizador = ss.IdDono,
                Preco = ss.Preco,
                Duracao = m,
                IdRent = Helpers.CacheController.utilizador,
                DataInicio = inicio,
                DataFim = fim,
                Estado = 0,
                Quantidade = 1,
            };
            model.Aluguer.Add(novo);
            model.SaveChanges();
            Helpers.CacheController.aluguerRealizado = tamanho;

            return RedirectToAction("AluguerPedido", "Utilizador", new { idArtigo = IdArtigo });
        }

        [Authorize]
        public ActionResult AluguerPedido(int idArtigo)
        {
            int s = Helpers.CacheController.aluguerRealizado;
            Aluguer ss = model.Aluguer.FirstOrDefault(x => x.IdAluguer.Equals(s));
            Artigo a = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(idArtigo));
            ss.IdArtigoNavigation.Nome = a.Nome;
            ss.IdArtigoNavigation.Imagem = a.Imagem;

            AluguerInfo res = new AluguerInfo()
            {
                IdArtigo = ss.IdArtigo,
                IdAluguer = ss.IdAluguer,
                NomeArtigo = ss.IdArtigoNavigation.Nome,
                Preco = ss.Preco,
                Quantidade = ss.Quantidade,
                Imagem = ss.IdArtigoNavigation.Imagem,
                Email = ss.IdRent,
                DataInicio = ss.DataInicio,
                DataFim = ss.DataFim,

            };

            Utilizador u = (from m in model.Utilizador where (m.Email.Equals(a.IdDono)) select m).ToList().ElementAt<Utilizador>(0);
            u.Notificacoes++;

            string user = Helpers.CacheController.utilizador;
            Utilizador uti = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = uti.Notificacoes;

            ViewData["noti"] = notifications;

            model.SaveChanges();

            return View(res);
        }


        [Authorize]
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


        [Authorize]
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


        [Authorize]
        public ActionResult Aceitar(int idAluguer)
        {
            Aluguer u = (from alu in model.Aluguer where (alu.IdAluguer == idAluguer) select alu).ToList().ElementAt<Aluguer>(0);
            u.Estado = 1;
            Artigo a = (from m in model.Artigo where (m.IdArtigo == u.IdArtigo) select m).ToList().ElementAt<Artigo>(0);
            if (u.Quantidade <= a.Quantidade)
            {
                a.Quantidade -= u.Quantidade;
            }
            else
            {
                return Content("You don't have enough items...");
            }

            Utilizador uti = (from m in model.Utilizador where (m.Email.Equals(u.IdRent)) select m).ToList().ElementAt<Utilizador>(0);
            uti.Notificacoes++;

            model.SaveChanges();
            return RedirectToAction("AluguerPedidos", "Utilizador");
        }

        [Authorize]
        public ActionResult Recusar(int idAluguer)
        {
            Aluguer u = (from alu in model.Aluguer where (alu.IdAluguer == idAluguer) select alu).ToList().ElementAt<Aluguer>(0);
            u.Estado = 2; //estado 2 para recusado
                          // model.Aluguer.Remove(u);

            Utilizador uti = (from m in model.Utilizador where (m.Email.Equals(u.IdRent)) select m).ToList().ElementAt<Utilizador>(0);
            uti.Notificacoes++;

            model.SaveChanges();
            return RedirectToAction("AluguerPedidos", "Utilizador");
        }


        [Authorize]
        public ActionResult AluguerPedidos()
        {
            string user = Helpers.CacheController.utilizador;

            var alugueres = (from alu in model.Aluguer where (alu.IdUtilizador == user && alu.Estado == 0) select alu);
            List<Aluguer> lista = alugueres.ToList<Aluguer>();
            List<AluguerInfo> nots = new List<AluguerInfo>();
            foreach (Aluguer alug in lista)
            {
                Artigo artigo = (from m in model.Artigo where (m.IdArtigo == alug.IdArtigo) select m).ToList().ElementAt<Artigo>(0);
                Utilizador u = (from m in model.Utilizador where (m.Email.Equals(alug.IdRent)) select m).ToList().ElementAt<Utilizador>(0);

                AluguerInfo not = new AluguerInfo()

                {
                    IdArtigo = artigo.IdArtigo,
                    IdAluguer = alug.IdAluguer,
                    NomeArtigo = artigo.Nome,
                    Preco = alug.Preco,
                    Quantidade = alug.Quantidade,
                    Imagem = artigo.Imagem,
                    DataInicio = alug.DataInicio,
                    DataFim = alug.DataFim,
                    Email = u.Email,
                    Nome = u.Nome,
                    Telemovel = u.Telemovel,
                    CodPostal = u.CodPostal,
                    Rua = u.Rua,
                    NPorta = u.NPorta
                };
                nots.Add(not);
            }


            Utilizador uti = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = uti.Notificacoes;

            ViewData["noti"] = notifications;

            return View(nots);
        }


        [Authorize]
        public ActionResult AluguerRespostas()
        {
            string user = Helpers.CacheController.utilizador;

            var alugueres = (from alu in model.Aluguer where (alu.IdRent == user) select alu);
            List<Aluguer> lista = alugueres.ToList<Aluguer>();
            List<AluguerInfo> nots = new List<AluguerInfo>();
            String tipo = " ";
            foreach (Aluguer alug in lista)
            {
                Artigo artigo = (from m in model.Artigo where (m.IdArtigo == alug.IdArtigo) select m).ToList().ElementAt<Artigo>(0);
                Utilizador u = (from m in model.Utilizador where (m.Email.Equals(alug.IdUtilizador)) select m).ToList().ElementAt<Utilizador>(0);
                if (alug.Estado == 0)
                {
                    tipo = "Pendente";

                }
                else if (alug.Estado == 1)
                {
                    tipo = "Aceite";
                }
                else if (alug.Estado == 2)
                {
                    tipo = "Recusado";
                }
                AluguerInfo not = new AluguerInfo()

                {
                    Tipo = tipo,
                    IdArtigo = artigo.IdArtigo,
                    IdAluguer = alug.IdAluguer,
                    NomeArtigo = artigo.Nome,
                    Preco = alug.Preco,
                    Quantidade = alug.Quantidade,
                    Imagem = artigo.Imagem,
                    DataInicio = alug.DataInicio,
                    DataFim = alug.DataFim,
                    Email = u.Email,
                    Nome = u.Nome,
                    Telemovel = u.Telemovel,
                    CodPostal = u.CodPostal,
                    Rua = u.Rua,
                    NPorta = u.NPorta
                };
                nots.Add(not);
            }

            Utilizador uti = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = uti.Notificacoes;

            ViewData["noti"] = notifications;

            return View(nots);
        }


        [Authorize]
        public ActionResult Historico()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("Historico");
        }


        [Authorize]
        public ActionResult AluguerInfo()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador std = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            std.Notificacoes = 0;

            model.SaveChanges();

            int notifications = std.Notificacoes;

            ViewData["noti"] = notifications;

            return View("AluguerInfo");
        }


        [Authorize]
        public ActionResult HAlugueres()
        {
            string user = Helpers.CacheController.utilizador;
            List<AluguerInfo> alugueres = new List<AluguerInfo>();

            var alugueres1 = from alu in model.Aluguer where (alu.IdUtilizador == user || alu.IdRent == user) && alu.Estado == 1 select alu;
            List<Aluguer> lista1 = alugueres1.ToList<Aluguer>();

            string tipo = " ";


            foreach (Aluguer alug in lista1)
            {
                Utilizador u = new Utilizador();

                if (alug.IdUtilizador == user)
                {
                    tipo = "Recebido";
                    u = (from m in model.Utilizador where (m.Email.Equals(alug.IdRent)) select m).ToList().ElementAt<Utilizador>(0);
                }
                else if (alug.IdRent == user)
                {
                    tipo = "Realizado";
                    u = (from m in model.Utilizador where (m.Email.Equals(alug.IdUtilizador)) select m).ToList().ElementAt<Utilizador>(0);
                }

                Artigo artigo = (from m in model.Artigo where (m.IdArtigo == alug.IdArtigo) select m).ToList().ElementAt<Artigo>(0);

                AluguerInfo a1 = new AluguerInfo()

                {
                    IdArtigo = artigo.IdArtigo,
                    IdAluguer = alug.IdAluguer,
                    NomeArtigo = artigo.Nome,
                    Preco = alug.Preco,
                    Quantidade = alug.Quantidade,
                    Imagem = artigo.Imagem,
                    DataInicio = alug.DataInicio,
                    DataFim = alug.DataFim,
                    Email = u.Email,
                    Nome = u.Nome,
                    Telemovel = u.Telemovel,
                    CodPostal = u.CodPostal,
                    Rua = u.Rua,
                    NPorta = u.NPorta,
                    Tipo = tipo

                };
                alugueres.Add(a1);
            }

            Utilizador uti = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = uti.Notificacoes;

            ViewData["noti"] = notifications;

            return View(alugueres);
        }
        


        [Authorize]
        public ActionResult HVendas()
        {
            string user = Helpers.CacheController.utilizador;
            List<VendaInfo> vendas = new List<VendaInfo>();

            var vendas1 = from ven in model.Venda where (ven.IdUtilizador == user || ven.IdRent == user) && ven.Estado == 1 select ven;
            List<Venda> lista1 = vendas1.ToList<Venda>();

            string tipo = " ";


            foreach (Venda v in lista1)
            {
                Utilizador u = new Utilizador();

                if (v.IdUtilizador == user)
                {
                    tipo = "Recebido";
                    u = (from m in model.Utilizador where (m.Email.Equals(v.IdRent)) select m).ToList().ElementAt<Utilizador>(0);
                }
                else if (v.IdRent == user)
                {
                    tipo = "Realizado";
                    u = (from m in model.Utilizador where (m.Email.Equals(v.IdUtilizador)) select m).ToList().ElementAt<Utilizador>(0);
                }

                Artigo artigo = (from m in model.Artigo where (m.IdArtigo == v.IdArtigo) select m).ToList().ElementAt<Artigo>(0);

                VendaInfo a1 = new VendaInfo()

                {
                    IdArtigo = artigo.IdArtigo,
                    IdVenda = v.IdVenda,
                    NomeArtigo = artigo.Nome,
                    Preco = v.Preco,
                    Quantidade = v.Quantidade,
                    Imagem = artigo.Imagem,
                    Email = u.Email,
                    Nome = u.Nome,
                    Telemovel = u.Telemovel,
                    CodPostal = u.CodPostal,
                    Rua = u.Rua,
                    NPorta = u.NPorta,
                    Tipo = tipo

                };
                vendas.Add(a1);
            }

            Utilizador uti = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = uti.Notificacoes;

            ViewData["noti"] = notifications;


            return View(vendas);
        }

        [Authorize]
        public ActionResult FinalizarCompra()
        {

            string user = Helpers.CacheController.utilizador;

            var vendas = (from vend in model.Venda where (vend.IdRent == user && vend.Estado == 0) select vend);
            List<Venda> lista = vendas.ToList<Venda>();

            foreach (Venda venda in lista)
            {
                Artigo artigo = (from m in model.Artigo where (m.IdArtigo == venda.IdArtigo) select m).ToList().ElementAt<Artigo>(0);
                artigo.Quantidade -= venda.Quantidade;
                venda.Estado = 1;
            }
            model.SaveChanges();
            return RedirectToAction("VendaInfo", "Utilizador");
        }

        [Authorize]
        public ActionResult RemArtCarrinho(int idVenda)
        {
            Venda v = (from vend in model.Venda where (vend.IdVenda == idVenda) select vend).ToList().ElementAt<Venda>(0);
            //model.Venda.Remove(v);
            v.Estado = 2;
            model.SaveChanges();
            return RedirectToAction("VendaInfo", "Utilizador");
        }

        public ActionResult VendaInfo()
        {

            /**
            string user = Helpers.CacheController.utilizador;

            var vendas = (from vend in model.Venda where (vend.IdRent == user && vend.Estado == 0) select vend);
            List<Venda> lista = vendas.ToList<Venda>();

            List<VendaInfo> carInfo = new List<VendaInfo>();
            foreach (Venda venda in lista)
            {
                Artigo artigo = (from m in model.Artigo where (m.IdArtigo == venda.IdArtigo) select m).ToList().ElementAt<Artigo>(0);
                VendaInfo car = new VendaInfo()

                {
                    IdArtigo = artigo.IdArtigo,
                    IdVenda = venda.IdVenda,
                    NomeArtigo = artigo.Nome,
                    Preco = venda.Preco,
                    Quantidade = venda.Quantidade,
                    Imagem = artigo.Imagem
                };
                carInfo.Add(car);
            }
            */
            string user = Helpers.CacheController.utilizador;
            var vendas = (from vend in model.Venda where (vend.IdRent == user && vend.Estado == 0) select vend);
            List<Venda> lista = vendas.ToList<Venda>();
            foreach (Venda venda in lista)
            {
                Artigo artigo = (from m in model.Artigo where (m.IdArtigo == venda.IdArtigo) select m).ToList().ElementAt<Artigo>(0);
                var uti = from x in model.Utilizador where x.Email.Equals(user) select x;
                Utilizador res = uti.ToList<Utilizador>().FirstOrDefault();
                venda.IdArtigoNavigation = artigo;

                var vouchers = from alu in model.Voucher where (alu.IdUtilizador.Equals(user)) select alu;

                List<Voucher> v = vouchers.ToList<Voucher>();

                foreach (Voucher a in v)
                {
                    res.Voucher.Add(a);
                }

                venda.IdUtilizadorNavigation = res;
            }

            Utilizador uti2 = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = uti2.Notificacoes;

            ViewData["noti"] = notifications;



            return View(lista);
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        public ActionResult CBanc()
        {
            string user = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return View("CBanc");
        }

        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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


        public ActionResult SearchCategoria(string categoria)
        {
            var local = (from x in model.Artigo where (x.Categoria == categoria) select x);
            if (local.ToList().Count > 0)
            {
                List<Artigo> list = local.ToList<Artigo>();

                string user = Helpers.CacheController.utilizador;
                Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
                int notifications = u.Notificacoes;

                ViewData["noti"] = notifications;

                return View(list);
            }

            else return RedirectToAction("ErrorSearch", "Utilizador");
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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


        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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


        [Authorize]
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


        [Authorize]
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

        [HttpPost]
        public IActionResult SearchComentarios(int IdArtigo)
        {
            var local = (from x in model.Comentarios where (x.IdArtigo == IdArtigo) select x);

            List<Comentarios> list = local.ToList<Comentarios>();

            return View(list);

        }

        [Authorize]
        [HttpPost]
        public IActionResult AddComentario(String Descricao, int IdArtigo)
        {
            string user = Helpers.CacheController.utilizador;
            var comentarios = (from com in model.Comentarios where (com.IdArtigo == IdArtigo) select com);
            List<Comentarios> lista = comentarios.ToList<Comentarios>();
            var totalcomentarios = (from comtotal in model.Comentarios select comtotal);
            List<Comentarios> listatotal = totalcomentarios.ToList<Comentarios>();

            List<int> indexes = new List<int>();

            foreach (Comentarios coment in listatotal)
            {
                indexes.Add(coment.IdComentario);
            }

            int tamanho = (indexes.Max()) + 1;

            Comentarios c = new Comentarios();
            c.IdComentario = tamanho;
            c.Descricao = Descricao;
            c.IdUtilizador = user;
            c.IdArtigo = IdArtigo;

            if (ModelState.IsValid)
            {
                model.Comentarios.Add(c);

                model.SaveChanges();
            }

            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(user)).FirstOrDefault();
            int notifications = u.Notificacoes;

            ViewData["noti"] = notifications;

            return RedirectToAction("Details", "Utilizador", new { idArtigo = IdArtigo });
        }


        [Authorize]
        [HttpPost]
        public IActionResult Stars(int IdArtigo, int pont)
        {
            Artigo art = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(IdArtigo));

            double pontuacao = art.Pontuacao;

            int nrVotos = art.NumeroVotos;
            int newNrVotos = nrVotos + 1;

            art.NumeroVotos = newNrVotos;

            double newPontuacaoAcumulada = art.PontucaoAcumulada + pont;
            double newPontuacao = newPontuacaoAcumulada / (double)newNrVotos;

            art.Pontuacao = Math.Truncate(100 * newPontuacao) / 100; ;
            art.PontucaoAcumulada = newPontuacaoAcumulada;

            if (ModelState.IsValid)
            {

                model.SaveChanges();
            }

            return RedirectToAction("Details", "Utilizador", new { IdArtigo = IdArtigo });

        }

        [Authorize]
        public ActionResult Remover(int idArtigo)
        {

            Artigo art = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(idArtigo));

            List<Comentarios> comentarios = model.Comentarios.Where(x => x.IdArtigo.Equals(art.IdArtigo)).ToList();
            foreach(Comentarios c in comentarios){
                model.Comentarios.Remove(c);
            }

            List<Aluguer> alugueres = model.Aluguer.Where(x => x.IdArtigo.Equals(art.IdArtigo)).ToList();
            foreach (Aluguer a in alugueres)
            {
                model.Aluguer.Remove(a);
            }

            List<Venda> vendas = model.Venda.Where(x => x.IdArtigo.Equals(art.IdArtigo)).ToList();
            foreach (Venda v in vendas)
            {
                model.Venda.Remove(v);
            }

            model.SaveChanges();

            model.Artigo.Remove(art);
            model.SaveChanges();
            return RedirectToAction("VerArtigos", "Utilizador");
        }


        /*
         * 
        //Procurar os comentários de um utilizador qualquer-----------------------------------
        public ActionResult SearchComentarioUser(String user){

            var local = (from x in model.Comentarios where (x.IdUtilizador == user) select x);
            if (local.ToList().Count > 0)
            {
                List<Artigo> list = local.ToList<Artigo>();
                return View(list);
            }

            else return RedirectToAction("ErrorSearch", "Utilizador");
        }*/

        /*
        [HttpGet]
        public ActionResult SearchComentarioArtigo(int artigo){

            var local = (from x in model.Comentarios where (x.IdArtigo == artigo) select x);
            if (local.ToList().Count > 0)
            {
                List<Artigo> list = local.ToList<Artigo>();

                return PartialView("show", local.YourDbSet.ToList());

            }

            else return RedirectToAction("ErrorSearch", "Utilizador");
        }
        */

        /*
        public ActionResult AplicarDesconto(List<Venda> list, float discount){
            float num = 0.0;

            foreach(Venda v in list){
                num += v.Preco;
            }

            num = num * (1-discount);

            return (num);
        }

        //Adicionar um Voucher á BD -------------------------------------
        public ActionResult AdicionarVoucher(string codigo,float valor){
            string user = Helpers.CacheController.utilizador;

            Vouchers v = new Vouchers();
            v.Codigo = codigo;
            v.Estado = 1;
            v.ValorOferta = valor;
            v.IdUtilizador = user;

            if (ModelState.IsValid){
                    model.Vouchers.Add(v);
                    model.SaveChanges();
            }

            return ;
        }
        
        //Verificar se é possível ou não utilizar o voucher pretendido --------------------------------------------------------------
        public ActionResult UsarVoucher(string codigo){
            string user = Helpers.CacheController.utilizador;

            var vouchers = (from v in model.Vouchers where (v.Codigo == codigo && v.Estado == 1 && v.IdUtilizador == user) select v);
            List<Vouchers> lista = vouchers.ToList<Vouchers>();

            if(lista.Count != 0){
                Vouchers v = lista.get(0);
                v.Estado = 0;
                return (v.ValorOferta);
            }
            else{
                String s = "Sorry, but the Voucher " + codigo + " does not exist/ is not aplicable";
                return Content(s);
            }
        }
       */

        public ActionResult Denunciar(int IdArtigo)
        {
            Artigo ss = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(IdArtigo));
            return View(ss);
        }



        
        

    [HttpPost]
        public ActionResult AddDenunciar(String Descricao, int IdArtigo)
        {
            string user = CacheController.utilizador;
            var totalDenuncias = (from comtotal in model.Denuncias select comtotal);
            List<Denuncias> listatotal = totalDenuncias.ToList<Denuncias>();

            Artigo a = model.Artigo.Where(x => x.IdArtigo == IdArtigo).FirstOrDefault();
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(a.IdDono)).FirstOrDefault();

            if (ModelState.IsValid)
        {

            List<int> indexes = new List<int>();

            foreach (Denuncias denunc in listatotal)
            {
                indexes.Add(denunc.IdDenuncia);
            }

            int tamanho = (indexes.Max()) + 1;

            Denuncias d = new Denuncias();
            d.IdDenuncia = tamanho;
            d.Descricao = Descricao;
            d.IdAutor = user;
            d.Administrador = "admin@gmail.com";
            d.IdArtigo = IdArtigo;
            DateTime today = DateTime.Today;
                d.Data = today;

                u.NDenuncias++;


                model.Denuncias.Add(d);

                model.SaveChanges();
            }


            return RedirectToAction("Index", "Utilizador");
        }

    }

}
