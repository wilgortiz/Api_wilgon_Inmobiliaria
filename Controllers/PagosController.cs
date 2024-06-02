using wepApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace wepApi.Controllers
{
	[Route("[Controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[ApiController]
	public class PagosController : ControllerBase
	{
		private readonly DataContext contexto;
		private readonly IConfiguration configuracion;
		public PagosController(DataContext context, IConfiguration config)
		{
			contexto = context;
			configuracion = config;
		}

		// GET: Pagos/Obtener/{contrato_id}
		[HttpGet("Obtener/{contrato_id}")]
		[Authorize]
		public IActionResult obtenerPorContrato(int contrato_id)
		{
			try
			{
				int.TryParse(User.FindFirstValue("Id"), out int userId);

				var usuario = User.Identity != null
					? contexto.Propietarios.Find(userId)
					: null;
				if (usuario == null) return NotFound();

				var contrato = contexto.Contratos.Include(c => c.Inmueble).FirstOrDefault(c => c.Id == contrato_id);
				if (contrato == null) return NotFound();

				if (contrato.Inmueble.PropietarioId != usuario.Id) return Unauthorized();

				var pagos = contexto.Pagos.Where(p => p.ContratoId == contrato_id);

				return Ok(pagos);
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
		}
	}
}