namespace ERPSEI.Services.Adquisiciones
{
    public sealed class AdquisicionesSolicitudPagoCentralDto
    {
        public int SolicitudId { get; set; }

        public int SolicitudPagoId { get; set; }

        public int AreaId { get; set; }

        public string Area { get; set; } =
            string.Empty;

        public string Folio { get; set; } =
            string.Empty;

        public string Titulo { get; set; } =
            string.Empty;

        public string NombreProveedor { get; set; } =
            string.Empty;

        public string Moneda { get; set; } =
            string.Empty;

        public string FormaPago { get; set; } =
            string.Empty;

        public decimal Total { get; set; }

        public int EstatusId { get; set; }

        public string Estatus { get; set; } =
            string.Empty;

        public DateTime FechaSolicitud { get; set; }

        public DateTime FechaGeneracion { get; set; }

        public int TotalFirmas { get; set; }

        public int TotalFirmasRequeridas { get; set; } =
            4;

        public bool PresupuestoAprobado { get; set; }

        public bool PdfGenerado { get; set; }

        public string? NombreArchivo { get; set; }

        public bool UsuarioPuedeDescargar { get; set; }

        public int TotalParcialidades { get; set; }

        public int ParcialidadesPagadas { get; set; }

        public decimal ImportePagado { get; set; }

        public decimal ImportePendiente =>
            Math.Max(
                0m,
                Total - ImportePagado
            );

        public bool FirmasCompletas =>
            TotalFirmas == TotalFirmasRequeridas;

        public bool ListaParaProcesar =>
            PresupuestoAprobado &&
            FirmasCompletas &&
            PdfGenerado &&
            !string.IsNullOrWhiteSpace(
                NombreArchivo
            );

        public bool PuedeDescargar =>
            ListaParaProcesar &&
            UsuarioPuedeDescargar;

        public decimal PorcentajePagado =>
            Total <= 0m
                ? 0m
                : Math.Round(
                    ImportePagado / Total * 100m,
                    2
                );

        public string EstadoPago =>
            ImportePendiente == 0m && Total > 0m
                ? "Pagado"
                : ImportePagado > 0m
                    ? "Pago parcial"
                    : "Pendiente";
    }
}