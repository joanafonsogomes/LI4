using System;
using System.Net;
using System.Net.Mail;

public partial class PassRec
{
	public void Rec_Button(string email, string pass) 
    {
      

        MailMessage mm = new MailMessage("rentitallofficial@gmail.com", email);
        mm.Subject = "Recuperação de password!";
        mm.Body = string.Format("Hello! Esta é a tua nova pass : "+pass);
        mm.IsBodyHtml = true;
        SmtpClient smtp = new SmtpClient();
        smtp.Host = "smtp.gmail.com";
        smtp.EnableSsl = true;
        NetworkCredential nc = new NetworkCredential();
        nc.UserName = "rentitallofficial@gmail.com";
        nc.Password = "rent12345";
        smtp.UseDefaultCredentials = true;
        smtp.Credentials = nc;
        smtp.Port = 587;
        smtp.EnableSsl = true;
        smtp.Send(mm);
        


    }
}
