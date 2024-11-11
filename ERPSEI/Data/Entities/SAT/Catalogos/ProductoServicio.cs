using System.ComponentModel.DataAnnotations.Schema;
using ERPSEI.Data.Entities.Cuentas;
using ERPSEI.Data.Entities.Empresas;

namespace ERPSEI.Data.Entities.SAT.Catalogos
{
    public class ProductoServicio
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Clave { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public bool IncluirIVATraslado { get; set; }

        public bool IncluirIEPSTraslado { get; set; }

        public string PalabrasSimilares { get; set; } = string.Empty;

        public int Deshabilitado { get; set; } = 0;

        public ICollection<ProductoServicioPerfil>? ProductosServiciosPerfil { get; set; }

        public ICollection<CuentaContableProductoServicio>? ProductosServiciosCuenta { get; set; } = [];

    }
}
