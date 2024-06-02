using wepApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;
using System.Net.Sockets;
using System.Net;

using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting; 
using System.IO; 



namespace wepApi.Controllers
{
	[Route("[Controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[ApiController]
	public class PropietariosController : ControllerBase
	{
		private readonly DataContext contexto;
		private readonly IConfiguration configuracion;
		private string hashSalt = "";


		private readonly IWebHostEnvironment hostEnvironment;  

		public PropietariosController(DataContext context, IConfiguration config, IWebHostEnvironment env)
		{
			contexto = context;
			configuracion = config;
			hashSalt = configuracion["Salt"] ?? "";
			hostEnvironment = env; 
		}








		// POST: Propietarios/Register
		[HttpPost("Register")]
		[AllowAnonymous]
		public IActionResult Register([FromForm] RegisterView registerView)
		{
			try
			{
				string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password: registerView.Password,
					salt: Encoding.ASCII.GetBytes(hashSalt),
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 10000,
					numBytesRequested: 256 / 8
				));

				var newPropietario = new Propietario
				{
					Nombre = registerView.Nombre,
					Apellido = registerView.Apellido,
					Dni = registerView.Dni,
					Telefono = registerView.Telefono,
					Email = registerView.Email,
					Password = hashed
				};

				contexto.Propietarios.Add(newPropietario);
				contexto.SaveChanges();

				return Ok(new { Message = "Usuario registrado con éxito" });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}















		// POST: Propietarios/Login
		[HttpPost("Login")]
		[AllowAnonymous]
		public IActionResult Login([FromForm] LoginView loginView)
		{
			try
			{
				string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password: loginView.Password,
					salt: System.Text.Encoding.ASCII.GetBytes(hashSalt),
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 10000,
					numBytesRequested: 256 / 8
				));

				var propietario = contexto.Propietarios.FirstOrDefault(x => x.Email == loginView.Email);
				if (propietario == null || hashed != propietario.Password)
				{
					return BadRequest("Nombre de usuario o clave incorrecta");
				}
				else
				{
					string secretKey = configuracion["TokenAuthentication:SecretKey"] ?? throw new ArgumentNullException(nameof(secretKey));
					var securityKey = secretKey != null ? new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey)) : null;
					var credenciales = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, propietario.Email),
						new Claim("Id", propietario.Id.ToString())
					};

					var token = new JwtSecurityToken(
						issuer: configuracion["TokenAuthentication:Issuer"],
						audience: configuracion["TokenAuthentication:Audience"],
						claims: claims,
						expires: DateTime.Now.AddMinutes(60),
						signingCredentials: credenciales
					);

					return Ok(new JwtSecurityTokenHandler().WriteToken(token));
				}

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}










		// GET: Popietarios/Perfil
		[HttpGet("Perfil")]
		[Authorize]
		public IActionResult GetPropietario()
		{
			try
			{
				var propietario = User.Identity != null
					? contexto.Propietarios
						.Where(x => x.Email == User.Identity.Name)
						.Select(x => new Propietario(x))
						.FirstOrDefault()
					: null;

				if (propietario == null)
				{
					return NotFound();
				}

				return Ok(propietario);
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
		}








		[HttpPut("Editar")]
		[Authorize]
		public IActionResult PutPropietario(Propietario propietario)
		{
			try
			{
				int.TryParse(User.FindFirstValue("Id"), out int userId);
				var propietarioDb = User.Identity != null
					? contexto.Propietarios.Find(userId)
					: null;

				if (propietarioDb == null) return NotFound();

				if (propietario.Id != propietarioDb.Id) return BadRequest();

				if (
					string.IsNullOrEmpty(propietario.Nombre) ||
					string.IsNullOrEmpty(propietario.Apellido) ||
					string.IsNullOrEmpty(propietario.Email) ||
					string.IsNullOrEmpty(propietario.Telefono)
				)
				{
					return BadRequest("Ningún campo puede ser vacío");
				}

				propietarioDb.Dni = propietario.Dni;
				propietarioDb.Nombre = propietario.Nombre;
				propietarioDb.Apellido = propietario.Apellido;
				propietarioDb.Email = propietario.Email;
				propietarioDb.Telefono = propietario.Telefono;
				//if (!string.IsNullOrEmpty(propietario.Avatar)) // Agregar la actualización del avatar
				//{
				//	propietarioDb.Avatar = propietario.Avatar;
				//}

				contexto.Propietarios.Update(propietarioDb);
				contexto.SaveChanges();

				return Ok(propietario);
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
		}















		[HttpPut("CambiarClave")]
		[Authorize]
		public IActionResult CambiarClave([FromBody] CambiarClaveView cambiarClaveView)
		{
			try
			{
				int userId = int.Parse(User.FindFirstValue("Id"));
				var propietario = contexto.Propietarios.Find(userId);

				if (propietario == null)
				{
					return NotFound("Usuario no encontrado");
				}

				string hashedActual = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password: cambiarClaveView.ClaveActual,
					salt: Encoding.ASCII.GetBytes(hashSalt),
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 10000,
					numBytesRequested: 256 / 8
				));

				if (hashedActual != propietario.Password)
				{
					return BadRequest("La clave actual es incorrecta");
				}

				if (cambiarClaveView.ClaveActual == cambiarClaveView.ClaveNueva)
				{
					return BadRequest("La nueva clave debe ser diferente a la actual");
				}

				string hashedNueva = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password: cambiarClaveView.ClaveNueva,
					salt: Encoding.ASCII.GetBytes(hashSalt),
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 10000,
					numBytesRequested: 256 / 8
				));

				propietario.Password = hashedNueva;
				contexto.Propietarios.Update(propietario);
				contexto.SaveChanges();

				return Ok("Clave cambiada con éxito");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}














		[HttpGet("cambiarpassword")]
		public async Task<IActionResult> CambiarPassword()
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.ASCII.GetBytes(configuracion["TokenAuthentication:SecretKey"]);
				var symmetricKey = new SymmetricSecurityKey(key);
				Random rand = new Random(Environment.TickCount);
				string randomChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
				string nuevaClave = "";
				for (int i = 0; i < 8; i++)
				{
					nuevaClave += randomChars[rand.Next(0, randomChars.Length)];
				}
				string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password: nuevaClave,
					salt: Encoding.ASCII.GetBytes(hashSalt),
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 10000,
					numBytesRequested: 256 / 8));
				var p = await contexto.Propietarios.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
				if (p == null)
				{
					return BadRequest("Nombre de usuario incorrecto");
				}
				else
				{
					p.Password = hashed;
					contexto.Propietarios.Update(p);
					await contexto.SaveChangesAsync();
					var message = new MimeMessage();
					message.To.Add(new MailboxAddress(p.Nombre, p.Email));
					message.From.Add(new MailboxAddress("Sistema", configuracion["SMTPUser"]));
					message.Subject = "Restablecimiento de Contraseña";
					message.Body = new TextPart("html")
					{
						Text = $"<h1>Hola {p.Nombre},</h1>" +
							   $"<p>Has cambiado tu contraseña de forma correcta. " +
							   $"Tu nueva contraseña es la siguiente: {nuevaClave}</p>"
					};
					using var client = new SmtpClient();
					client.ServerCertificateValidationCallback = (s, c, h, e) => true;
					await client.ConnectAsync("sandbox.smtp.mailtrap.io", 587, MailKit.Security.SecureSocketOptions.StartTls);
					await client.AuthenticateAsync(configuracion["SMTPUser"], configuracion["SMTPPass"]);
					await client.SendAsync(message);
					await client.DisconnectAsync(true);

					return Ok("Se ha restablecido la contraseña correctamente.");
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}



		private string GetLocalIpAddress()
		{
			string localIp = null;
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					localIp = ip.ToString();
					break;
				}
			}
			return localIp;
		}







[HttpPost("olvidecontraseña")]
		[AllowAnonymous]
		public async Task<IActionResult> EnviarEmail([FromForm] string email)
		{
			try
			{
				var propietario = await contexto.Propietarios.FirstOrDefaultAsync(x => x.Email == email);
				if (propietario == null)
				{
					return NotFound("No se encontró ningún usuario con esta dirección de correo electrónico.");
				}
				var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuracion["TokenAuthentication:SecretKey"]));
				var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, propietario.Email),
					new Claim("FullName", $"{propietario.Nombre} {propietario.Apellido}"),
					new Claim(ClaimTypes.Role, "Usuario"),
				};
				var token = new JwtSecurityToken(
					issuer: configuracion["TokenAuthentication:Issuer"],
					audience: configuracion["TokenAuthentication:Audience"],
					claims: claims,
					expires: DateTime.Now.AddMinutes(5),
					signingCredentials: credenciales
				);
				var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
				var dominio = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
				var resetLink = Url.Action("CambiarPassword", "Propietarios");
				var rutaCompleta = Request.Scheme + "://" + GetLocalIpAddress() + ":" + Request.Host.Port + resetLink;
				var message = new MimeMessage();
				message.To.Add(new MailboxAddress(propietario.Nombre, propietario.Email));
				message.From.Add(new MailboxAddress("Sistema", configuracion["SMTPUser"]));
				message.Subject = "Restablecimiento de Contraseña";
				message.Body = new TextPart("html")
				{
					Text = $@"<h1>Hola {propietario.Nombre},</h1>
						   <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.
							<p>Por favor, haz clic en el siguiente enlace para crear una nueva contraseña:</p>
						   <a href='{rutaCompleta}?access_token={tokenString}'>{rutaCompleta}?access_token={tokenString}</a>"
				};
				using var client = new SmtpClient();
				client.ServerCertificateValidationCallback = (s, c, h, e) => true;
				await client.ConnectAsync("sandbox.smtp.mailtrap.io", 587, MailKit.Security.SecureSocketOptions.StartTls);
				await client.AuthenticateAsync(configuracion["SMTPUser"], configuracion["SMTPPass"]);
				await client.SendAsync(message);
				await client.DisconnectAsync(true);
				return Ok("Se ha enviado el enlace de restablecimiento de contraseña correctamente.");
			}
			catch (Exception ex)
			{
				return BadRequest($"Error: {ex.Message}");
			}
		}

















		[HttpPut]
		public async Task<IActionResult> Put([FromBody] Propietario entidad)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var propietario = await contexto.Propietarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entidad.Id);
					if (propietario == null)
					{
						return NotFound("Propietario no encontrado");
					}
					entidad.Password = propietario.Password;
					Console.WriteLine("Contraseña: " + entidad.Password);
					contexto.Propietarios.Update(entidad);
					await contexto.SaveChangesAsync();
					var key = new SymmetricSecurityKey(
						Encoding.ASCII.GetBytes(configuracion["TokenAuthentication:SecretKey"]));
					var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
					var claims = new List<Claim>
					{
  						new Claim(ClaimTypes.Name, entidad.Email),
						new Claim("FullName", $"{entidad.Nombre} {entidad.Apellido}")



						//new Claim(ClaimTypes.Name, entidad.Email),
						//new Claim("FullName", entidad.Nombre + " " + entidad.Apellido),
						//new Claim(ClaimTypes.Role, "Administrador"),
					};
					var token = new JwtSecurityToken(
						issuer: configuracion["TokenAuthentication:Issuer"],
						audience: configuracion["TokenAuthentication:Audience"],
						claims: claims,
						expires: DateTime.Now.AddHours(60000),
						signingCredentials: credenciales
					);
					return Ok(new JwtSecurityTokenHandler().WriteToken(token));
				}

				return BadRequest("Modelo inválido");
			}
			catch (Exception ex)
			{
				// Agregar más detalles del error en el registro para depuración
				return BadRequest($"Error: {ex.Message}");
			}
		}







	}//FIN
}