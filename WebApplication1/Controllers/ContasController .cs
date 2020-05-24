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
using System.Threading.Tasks;

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
        public async Task<ActionResult> LoginAsync(string email, string password)
        {
            if (Request.Cookies.ContainsKey("CookieMonster"))
            {
                Helpers.CacheController.utilizador = email;
                return RedirectToAction("Index", "Utilizador");
            }
            DefaultController.Utilizador = new Utilizador();
            DefaultController.Utilizador.Password = password;
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
                            /*
                            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,email)
                    };
                            
                            var identety = new ClaimsIdentity(
                                   claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identety);
                            var props = new AuthenticationProperties();
                            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props).Wait();
                            */
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
                                ExpiresUtc = DateTime.Now.ToLocalTime().AddSeconds(10),
                                IsPersistent = true
                            };

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, propriedadesDeAutenticacao);




                            Helpers.CacheController.utilizador = userSingle.Email;
                            return RedirectToAction("Index", "Utilizador");
                        }
                        else

                        {
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
                            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,email)
                    };

                            var identety = new ClaimsIdentity(
                                   claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identety);
                            var props = new AuthenticationProperties();
                            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props).Wait();
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
                                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,email)
                    };

                                var identety = new ClaimsIdentity(
                                       claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                var principal = new ClaimsPrincipal(identety);
                                var props = new AuthenticationProperties();
                                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props).Wait();

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



        public async Task<IActionResult> LogOut()
        {
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