using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Filters
{
    public class Email
    {
        public async Task<bool> SendEmail(string email, string message, string subject, Econfig econfig, string? name = null)
        {
            try
            {
                string _name = "";
                string host = econfig._host;
                if (!string.IsNullOrEmpty(name))
                {
                    _name = name;
                }
                string username = econfig._username;
                string senha = econfig._senha;
                int porta = Convert.ToInt32(econfig._porta);

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(username, _name)
                };

                mail.To.Add(email);
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(host, porta))
                {
                    smtp.Credentials = new NetworkCredential(username, senha);
                    smtp.EnableSsl = true;

                    await smtp.SendMailAsync(mail);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                // Gravar log de erro ao enviar e-mail
                Console.WriteLine("email not send" + ex.Message);
                return false;
            }
        }

        public class Econfig
        {
            public string _host { get; set; }
            public string _username { get; set; }
            public string _senha { get; set; }
            public string _porta { get; set; }
        }
    }


}
