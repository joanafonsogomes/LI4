using WebApplication1.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Nancy.Authentication.Forms;
using WebApplication1.Helpers;
using WebApplication1.Models;
using System;
using System.Linq;
using System.Security.Cryptography;

using RestSharp;
using Nancy.Authentication.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections;
using System.Text;

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
            /*
            if (!Response.Cookies.Equals("CookieMonster"))
            {
                Helpers.CacheController.utilizador = email;
                return RedirectToAction("Index", "Utilizador");
            }*/

            DefaultController.Utilizador = new Utilizador();
            DefaultController.Utilizador.Password = password;
            // int userName = Int32.Parse(username);
            if (ModelState.IsValid)
            {
                var userS = (from u in model.Utilizador where (u.Email == email && u.Tipo == "single") select u);

                if (userS.ToList().Count > 0)
                {

                    Utilizador userSingle = userS.ToList().ElementAt<Utilizador>(0);
                    using (MD5 md5Hash = MD5.Create())
                    {
                        if (MyHelpers.VerifyMd5Hash(md5Hash, password, userSingle.Password))
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

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, propriedadesDeAutenticacao);

                            Helpers.CacheController.utilizador = userSingle.Email;
                            return RedirectToAction("Index", "Utilizador");
                        }
                        else

                        {
                            //ViewData["User_Name"] = "Bem vindo" + userSingle.Nome;
                            ModelState.AddModelError("password", "Password incorreta!");
                            return View();
                        }
                    }
                }

                else if (((from m in model.Administrador where (m.Email == email) select m)).ToList().Count > 0)
                {
                    var admin = ((from m in model.Administrador where (m.Email == email) select m)).ToList().ElementAt<Administrador>(0);

                    using (MD5 md5Hash = MD5.Create())
                    {
                        if (MyHelpers.VerifyMd5Hash(md5Hash, password, admin.Password))
                        {

                            //ViewData["User_Name"] = "Bem vindo";
                            /*
                            HttpCookie cookie = MyHelpers.CreateAuthorizeTicket(cliente.Id.ToString(), cliente.Role);
                                Response.Cookies.Add(cookie);
                                */
                            return RedirectToAction("Index", "Administrador");
                        }
                        else
                        {
                            ModelState.AddModelError("password", "Password incorreta!");
                            return View();
                        }
                    }

                }
                else
                {
                    var userC = (from m in model.Utilizador where (m.Email == email && m.Tipo == "company") select m);
                    if (userC.ToList().Count > 0)
                    {
                        Utilizador utilizador = userC.ToList().ElementAt<Utilizador>(0);
                        using (MD5 md5Hash = MD5.Create())
                        {
                            if (MyHelpers.VerifyMd5Hash(md5Hash, password, utilizador.Password))
                            {
                                /*
                                HttpCookie cookie = MyHelpers.CreateAuthorizeTicket(cliente.Id.ToString(), cliente.Role);
                                Response.Cookies.Add(cookie);
                                */
                                ViewData["User_Name"] = "Bem vindo" + utilizador.Nome;
                                return RedirectToAction("Index", "Funcionario");
                            }
                            else
                            {
                                ModelState.AddModelError("password", "Password incorreta!");
                                return View();
                            }
                        }
                    }


                    else
                    {
                        ModelState.AddModelError("", "Login data is incorrect!");
                        return View();
                    }
                }
            }

            return View();
        }

        public ActionResult Forgot()
        {
            return View();
        }

       

        [HttpPost]
        public ActionResult Enviar(string email) {
            Utilizador u = model.Utilizador.Where(x => x.Email.Equals(email)).FirstOrDefault();
            string pass = RandomString(8, true);
            u.Password = MyHelpers.HashPassword(u.Password);
            model.SaveChanges();
            PassRec pr = new PassRec();
            pr.Rec_Button(email,pass);

            return RedirectToAction("Index", "Home");
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