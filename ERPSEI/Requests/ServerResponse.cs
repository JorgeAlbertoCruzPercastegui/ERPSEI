namespace ERPSEI.Requests
{
	public class ServerResponse
	{
		public int CodigoError { get; set; }
		public bool TieneError { get; set; }
		public string? Mensaje { get; set; }
		public object? Datos { get; set; }
		public string[] Errores { get; set; } = Array.Empty<string>();

		public ServerResponse() { 
			TieneError = false;
			Mensaje = null;
			Datos = null;
			CodigoError = 0;
		}

		public ServerResponse(bool error, string? mensaje)
		{
			TieneError = error;
			Mensaje = mensaje;
			Datos = null;
			CodigoError = 0;
		}

		public ServerResponse(bool error, string? mensaje, object? datos)
		{
			TieneError = error;
			Mensaje = mensaje;
			Datos = datos;
			CodigoError = 0;
		}

		public ServerResponse(bool error, string? mensaje, object? datos, int codigoError)
		{
			TieneError = error;
			Mensaje = mensaje;
			Datos = datos;
			CodigoError = codigoError;
		}
	}
}
