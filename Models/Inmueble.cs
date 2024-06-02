using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wepApi.Models

{
	public class Inmueble
	{
		[Key]
		public int Id { get; set; }
		public string Uso { get; set; } 
		public string Tipo { get; set; } 
		public string Direccion { get; set; } = "";
		public int Ambientes { get; set; } = 1;
		//public decimal Precio { get; set; } = 0;
		public decimal Precio { get; set; }
		public Boolean Estado { get; set; } = true;
		public string? Imagen { get; set; } = "";
		[NotMapped]
		public IFormFile? ImagenFileName { get; set; }
		[ForeignKey(nameof(Propietario))]
		public int? PropietarioId { get; set; }
		public Propietario? Propietario { get; set; } = null;
	}
}
