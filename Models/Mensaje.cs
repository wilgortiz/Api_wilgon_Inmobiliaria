/*
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


namespace wepApi.Models
{
    public static class Mensaje
    {
        private static string GetMensajeEnlace(Propietario perfil, string enlace = "")
        {
            return $@"<p>Hola {perfil.Nombre}:</p>
                      <p>Hemos recibido una solicitud de restablecimiento de contraseña de tu cuenta.</p>
                      <p>Haz clic en el botón que aparece a continuación para cambiar tu contraseña.</p>
                      <p>Ten en cuenta que este enlace es válido solo durante 24 horas. Una vez transcurrido el plazo, deberás volver a solicitar el restablecimiento de la contraseña.</p>
                      <a href='{enlace}'>Cambiar contraseña</a>";
        }

        public static async Task<bool> EnviarEnlace(Propietario perfil, string subject, IConfiguration config, IWebHostEnvironment environment, JWT jwt)
        {
            try
            {
                var dominio = environment.IsDevelopment() ? config["AppSettings:DevelopmentDomain"] : config["AppSettings:ProductionDomain"];
                string token = new JwtSecurityTokenHandler().WriteToken(jwt.GenerarToken(perfil.Id, 5));
                string enlace = dominio + $"Propietario/token?access_token={token}";
                Console.WriteLine("enlace   token: " + enlace);

                var message = new MimeMessage();
                message.To.Add(new MailboxAddress(perfil.Nombre, perfil.Email));
                message.From.Add(new MailboxAddress("Inmobiliaria TP", config["Email:SMTPUser"]));
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = GetMensajeEnlace(perfil, enlace)
                };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                    client.Connect(config["Email:SMTPHost"], int.Parse(config["Email:SMTPPort"]), MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(config["Email:SMTPUser"], config["Email:SMTPPass"]);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
*/