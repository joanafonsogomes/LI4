using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ContaController : Controller
    {
        private Model model = new Model();

        public ActionResult Index(string user)
        {
            ViewData["User_Name"] = "Bem vindo" + user;
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> LoginAsync(string email, string password)
        {
            DefaultController.Utilizador = new Utilizador();
            DefaultController.Utilizador.Password = password;
            // int userName = Int32.Parse(username);
            if (ModelState.IsValid)
            {
                var claims = new List<Claim>
                                 {
                                       new Claim(ClaimTypes.Name, email),
                                       new Claim(ClaimTypes.Role, "User")
                                  };

                var identidadeDeUsuario = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal claimPrincipal = new ClaimsPrincipal(identidadeDeUsuario);

                var propriedadesDeAutenticacao = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.Now.ToLocalTime().AddHours(10),
                    IsPersistent = true
                };




                var userS = (from u
                             in model.Utilizador
                             where (u.Email == email && u.Tipo == "single")
                             select u);

                if (userS.ToList().Count > 0)
                {

                    Utilizador userSingle = userS.ToList().ElementAt<Utilizador>(0);
                    if (userSingle.Estado != 2)
                    {
                        using (MD5 md5Hash = MD5.Create())
                        {
                            if (MyHelpers.VerifyMd5Hash(md5Hash, password, userSingle.Password))
                            {
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, propriedadesDeAutenticacao);
                                Helpers.CacheController.utilizador = userSingle.Email;
                                return RedirectToAction("Index", "Utilizador");
                            }
                            else

                            {
                                //ViewData["User_Name"] = "Bem vindo" + userSingle.Nome;
                                ModelState.AddModelError("", "E-mail ou password incorreto(s).");
                                return View();
                            }
                        }
                    }
                }

                var administrador = (from a
                           in model.Administrador
                                     where (a.Email == email && a.Password == password)
                                     select a);

                if (administrador.ToList().Count > 0)
                {
                    Administrador admin = administrador.ToList().ElementAt<Administrador>(0);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, propriedadesDeAutenticacao);
                    Helpers.CacheController.utilizador = admin.Email;
                    return RedirectToAction("Index", "Admin");

                }

                var userC = (from m in model.Utilizador where (m.Email == email && m.Tipo == "company") select m);
                if (userC.ToList().Count > 0)
                {
                    Utilizador utilizador = userC.ToList().ElementAt<Utilizador>(0);
                    using (MD5 md5Hash = MD5.Create())
                    {
                        if (MyHelpers.VerifyMd5Hash(md5Hash, password, utilizador.Password))
                        {
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, propriedadesDeAutenticacao);
                            Helpers.CacheController.utilizador = utilizador.Email;

                            return RedirectToAction("Index", "Company");
                        }
                        else
                        {
                            ModelState.AddModelError("password", "Password incorreta!");
                            return View();
                        }
                    }
                }

            }
            {
                //ViewData["User_Name"] = "Bem vindo" + userSingle.Nome;
                ModelState.AddModelError("", "E-mail ou password incorreto(s).");
                return View();
            }
            return View();

        }

        public ActionResult Forgot()
        {
            return View();
        }
        public ActionResult Recovery()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Enviar(string email)
        {
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(email)).FirstOrDefault();
            Helpers.CacheController.utilizador = u.Email;
            string code = RandomString(8, true);
            u.Codigo = code;
            model.SaveChanges();
            PassRec pr = new PassRec();
            pr.Rec_Button(email, code);
            return RedirectToAction("Recovery", "Conta");
        }

        [HttpPost]
        public ActionResult Recovery(string code, string pass, string pa)
        {

            string email = Helpers.CacheController.utilizador;
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(email)).FirstOrDefault();
            if (u != null)
            {
                if (Equals(code, u.Codigo))
                {
                    if (Equals(pass, pa))
                    {
                        u.Password = MyHelpers.HashPassword(pass);
                        model.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("pass nao coincidem");
                        Console.WriteLine(pass);
                        Console.WriteLine(pa);
                    }
                }
                else { Console.WriteLine("codigo errado"); Console.WriteLine(u.Codigo); }
            }
            return RedirectToAction("Login", "Conta");
        }

        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
        public async System.Threading.Tasks.Task<ActionResult> LogOutAsync()
        {
            // FormsAuthentication.SignOut();
            await HttpContext.SignOutAsync();
            Response.Cookies.Delete("CookieMonster");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Registar(Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                using (Model model = new Model())
                {
                    model.Utilizador.Add(utilizador);
                    model.SaveChanges();
                }
                ModelState.Clear();
                ViewBag.Message = utilizador.Nome + " registada com sucesso.";
            }
            return View();
        }


        public ActionResult sucessAction()
        {
            ViewBag.title = "Sucesso";
            ViewBag.mensagem = "Login realizado com sucesso";
            ViewBag.controller = "Home";
            return View("_sucessView");
        }


    }
}