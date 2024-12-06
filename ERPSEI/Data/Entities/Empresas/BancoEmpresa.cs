using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Empresas
{
    public class BancoEmpresa
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Banco { get; set; } = string.Empty;
         
        public string Responsable { get; set; } = string.Empty;

        public string Firmante { get; set; } = string.Empty;

        public decimal Limite { get; set; } = 0m;

		public int? EmpresaId { get; set; }
		public Empresa? Empresa { get; set; }
	}
}
