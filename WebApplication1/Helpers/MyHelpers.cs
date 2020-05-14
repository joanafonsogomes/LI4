
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
/*
using System.Web.security;


namespace WebApplication1.Helpers
{
    public static class MyHelpers
    {
        public static HttpCookie CreateAuthorizeTicket(string id_user, string roles)
        {
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, id_user, DateTime.Now, DateTime.Now.AddMinutes(30), false, roles); //dados do utilizador
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket)); return cookie;
        }
    }
    */



