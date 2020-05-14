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


        public ActionResult Perfil()
        {
            string uti = Helpers.CacheController.utilizador;
            Utilizador std = model.Utilizador.Where(x => x.Email.Equals(uti)).FirstOrDefault();

            var vouchers = from alu in model.Voucher where (alu.IdUtilizador.Equals(uti)) select alu;
            /**
            List<Voucher> v = vouchers.ToList<Voucher>();

            foreach (Voucher a in v)
            {
                std.Voucher.Add(a);
            }
            */

            return View(std);
        }


        public ActionResult AdicionarCarrinho(int idArtigo)
        {

            Console.WriteLine(idArtigo);

            Artigo ss = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(idArtigo));

            var cenas = ss.IdArtigo;
            Console.WriteLine(cenas);
            string nower = Helpers.CacheController.utilizador;
            var vendas = (from vend in model.Venda where (vend.IdRent == nower && vend.Estado == 0) select vend);
            List<Venda> lista = vendas.ToList<Venda>();
            Utilizador uti = model.Utilizador.FirstOrDefault(x => x.Email.Equals(nower));
            int size = model.Venda.Length() + 3;

            Venda vendinha = new Venda()
            {
                IdVenda = size,
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

            return View(res);


        }




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
                    Estado = 0,
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


        /*public ActionResult Details(int idArtigo)
        {

            Artigo art = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(idArtigo));

            var cenas = art.IdArtigo;

            var comentarios = from x in model.Comentarios where (x.IdArtigo.Equals(idArtigo)) select x;
            List<Comentarios> c = comentarios.ToList<Comentarios>();

            foreach (Comentarios a in c)
            {
                art.Comentarios.Add(a);
            }


            return View(art);

        }*/
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


            return View(ss);

        }

        /**envia o pedido ao dono*/
        [HttpPost]
        public ActionResult Details(DateTime inicio, DateTime fim)
        {
            int ar = Helpers.CacheController.IdArtigo;
            Artigo ss = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(ar));
            string dono = ss.IdDono;
            Utilizador donito = model.Utilizador.FirstOrDefault(x => x.Email.Equals(dono));

            int tamanho = model.Aluguer.Length() + 1;

            int m = (fim - inicio).Days;
            Aluguer novo = new Aluguer()
            {
                IdAluguer = tamanho,
                IdArtigo = ar,
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
            return RedirectToAction("AluguerPedido", "Utilizador");
        }

        public ActionResult AluguerPedido()
        {
            int s = Helpers.CacheController.aluguerRealizado;
            Aluguer ss = model.Aluguer.FirstOrDefault(x => x.IdAluguer.Equals(s));
            int art = Helpers.CacheController.IdArtigo;
            Artigo a = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(art));
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
            return View(res);
        }

        /** Verifica disponibilidade de Datas para o caso de mudar-mos de ideia
        [HttpPost]
        public ActionResult Details(List<IFormFile> file, DateTime inicio, DateTime fim)
        {
            foreach (IFormFile cenas in file)
            {
                Console.WriteLine(cenas.FileName);
            }
            try
            {
                Console.WriteLine("???? like da fuck");
                int s = Helpers.CacheController.IdArtigo;
                Artigo ss = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(s));
                if (inicio >= fim )
                {
                    Console.WriteLine("entrei no primeiro ciclo.");
                    return View();

                }
                else
                {
                    Console.WriteLine("entrei no segundo ciclo.");
                    int imp = 0;
                    int tamanho = ss.Aluguer.Length() + 1;
                    foreach (Aluguer a in ss.Aluguer)
                    {
                        if (!(a.DataInicio > inicio && a.DataInicio > fim || inicio > a.DataFim && inicio > a.DataFim))
                        {
                            imp = 1;
                        }
                    }
                    if (imp == 0)
                    {
                        int m = (fim-inicio).Days;
                        Aluguer novo = new Aluguer()
                        {
                            IdAluguer = tamanho,
                            IdArtigo = s,
                            IdUtilizador = ss.IdDono,
                            Preco = ss.Preco,
                            Duracao = m,
                            IdRent = Helpers.CacheController.utilizador,
                            DataInicio = inicio,
                            DataFim = fim,
                            Estado = false,
                            Quantidade = 1,
                        };
                        ss.Aluguer.Add(novo);
                        model.SaveChanges();
                    }
                    else return View();

                }
            }
            catch (Exception)
            {
                return Content("Erro...");
            }
            return RedirectToAction("HAlugueres", "Utilizador");
        }

        
             */

        [HttpGet]
        public IActionResult NovoArtigo()
        {
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
                int i = lista.Count;

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
                    Pontuacao =0,
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

            return View(lista);
        }
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
            model.SaveChanges();
            return RedirectToAction("AluguerPedidos", "Utilizador");
        }

        public ActionResult Recusar(int idAluguer)
        {
            Aluguer u = (from alu in model.Aluguer where (alu.IdAluguer == idAluguer) select alu).ToList().ElementAt<Aluguer>(0);
            u.Estado = 2; //estado 2 para recusado
                          // model.Aluguer.Remove(u);
            model.SaveChanges();
            return RedirectToAction("AluguerPedidos", "Utilizador");
        }

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

            return View(nots);
        }
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

            return View(nots);
        }


        public ActionResult Historico()
        {
            return View("Historico");
        }
        public ActionResult AluguerInfo()
        {
            return View("AluguerInfo");
        }

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


            return View(alugueres);
        }
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


            return View(vendas);
        }

        public ActionResult FinalizarCompra()
        {

            string user = Helpers.CacheController.utilizador;

            var vendas = (from vend in model.Venda where (vend.IdRent == user && vend.Estado == 0) select vend);
            List<Venda> lista = vendas.ToList<Venda>();

            foreach (Venda venda in lista)
            {
                Artigo artigo = (from m in model.Artigo where (m.IdArtigo == venda.IdArtigo) select m).ToList().ElementAt<Artigo>(0);
                if (venda.Quantidade <= artigo.Quantidade)
                {
                    artigo.Quantidade -= venda.Quantidade;
                    venda.Estado = 1;
                }
                else
                {
                    //model.Venda.Remove(venda);
                    venda.Estado = 2;
                    String s = "Sorry, but the item " + venda.IdArtigo + " is out of stock";
                    return Content(s);
                }
            }
            model.SaveChanges();
            return RedirectToAction("VendaInfo", "Utilizador");
        }

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
            return View(carInfo);
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

        public ActionResult Alterar(int idArtigo)
        {

            Artigo ss = model.Artigo.FirstOrDefault(x => x.IdArtigo.Equals(idArtigo));

            Helpers.CacheController.IdArtigo = idArtigo;
            var cenas = ss.IdArtigo;
            Console.WriteLine(cenas);
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

        public ActionResult Password()
        {
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

            else return RedirectToAction("ErrorSearch", "Utilizador");
        }

        public ActionResult AlteraNome()
        {
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
            return RedirectToAction("verArtigos", "Utilizador");
        }

        public ActionResult AlteraQuantidade()
        {
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

        /* [HttpGet]
         public IActionResult SearchComentarios(int IdArtigo)
         {
             var local = (from x in model.Comentarios where (x.IdArtigo == IdArtigo) select x);

             List<Comentarios> list = local.ToList<Comentarios>();

             return View(list);
         }*/

        [HttpPost]
        public IActionResult SearchComentarios(int IdArtigo)
        {
            var local = (from x in model.Comentarios where (x.IdArtigo == IdArtigo) select x);

            List<Comentarios> list = local.ToList<Comentarios>();

            return View(list);

        }

        [HttpPost]
        public IActionResult AddComentario(String Descricao, int IdArtigo)
        {
            string user = Helpers.CacheController.utilizador;
            var comentarios = (from com in model.Comentarios where (com.IdArtigo == IdArtigo) select com);
            List<Comentarios> lista = comentarios.ToList<Comentarios>();
            var totalcomentarios = (from comtotal in model.Comentarios select comtotal);
            List<Comentarios> listatotal = totalcomentarios.ToList<Comentarios>();
            int id = listatotal.Count + 1;

            Comentarios c = new Comentarios();
            c.IdComentario = id;
            c.Descricao = Descricao;
            c.IdUtilizador = user;
            c.IdArtigo = IdArtigo;

            if (ModelState.IsValid)
            {
                model.Comentarios.Add(c);

                model.SaveChanges();
            }

            return RedirectToAction("Details", "Utilizador", new { idArtigo = IdArtigo });
        }

        //[HttpPost]
        /*  public IActionResult FiveStars(int IdArtigo)
          {
              Artigo art = (from x in model.Artigo where (x.IdArtigo == IdArtigo) select x);

              double pontuacao = art.Pontuacao;

              double newPontuacao = pontuacao + 

              return RedirectToAction("Details", "Utilizador", new { idArtigo = IdArtigo });

          }*/


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
    }
    
}
