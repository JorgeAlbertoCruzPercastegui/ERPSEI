using ERPSEI.Data.Entities.Empleados;

namespace ERPSEI.Data.Entities.Intranet
{
    public class ManualPoliticaArea
    {
        public int Id { get; set; }

        public int ManualPoliticaIntranetId { get; set; }
        public ManualPoliticaIntranet ManualPoliticaIntranet { get; set; } = null!;

        public int AreaId { get; set; }
        public Area Area { get; set; } = null!;
    }
}
