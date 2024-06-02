using System.ComponentModel.DataAnnotations;
namespace wepApi.Models;


public class CambiarClaveView
{
    [DataType(DataType.Password)]
    public string ClaveActual { get; set; }

    [DataType(DataType.Password)]
    public string ClaveNueva { get; set; }
}

