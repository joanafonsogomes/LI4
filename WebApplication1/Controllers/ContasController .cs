using WebApplication1.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Nancy.Authentication.Forms;

using RestSharp;
using Nancy.Authentication.Forms;

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
        public ActionResult Login(string email, string password)
        {

            DefaultController.Utilizador = new Utilizador();
            DefaultController.Utilizador.Password = password;
            // int userName = Int32.Parse(username);
            if (ModelState.IsValid)
            {
                var userS = (from u in model.Utilizador where (u.Email == email && u.Tipo == "single") select u);

                if (userS.ToList().Count > 0)
                {

                    Utilizador userSingle = userS.ToList().ElementAt<Utilizador>(0);
                    if (string.Equals(password, userSingle.Password))
                    {
                        ViewData["User_Name"] = "Bem vindo" + userSingle.Nome;
                        //return RedirectToAction("Index", "UtilizadorSingle");
                        //   return RedirectToAction("About", "Utilizador");
                        Helpers.CacheController.utilizador = userSingle.Email;
                        return RedirectToAction("Index", "Utilizador");
                    }
                    else
                    {
                        //ViewData["User_Name"] = "Bem vindo" + userSingle.Nome;
                        //ModelState.AddModelError("password", "Password incorreta!");
                        return View();
                    }
                }

                else if (((from m in model.Administrador where (m.Email == email) select m)).ToList().Count > 0)
                {
                    var admin = ((from m in model.Administrador where (m.Email == email) select m)).ToList().ElementAt<Administrador>(0);

                    if (string.Equals(password, admin.Password))
                    {

                        //ViewData["User_Name"] = "Bem vindo";
                        return RedirectToAction("Index", "Administrador");
                    }
                    else
                    {
                        ModelState.AddModelError("password", "Password incorreta!");
                        return View();
                    }
                }


                else
                {
                    var userC = (from m in model.Utilizador where (m.Email == email && m.Tipo == "company") select m);
                    if (userC.ToList().Count > 0)
                    {
                        Utilizador utilizador = userC.ToList().ElementAt<Utilizador>(0);
                        if (string.Equals(password, utilizador.Password))
                        {

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
            }

            else
            {
                ModelState.AddModelError("", "Login data is incorrect!");
                return View();
            }

            return View();
        }


        public ActionResult LogOut()
        {
            // FormsAuthentication.SignOut();
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