namespace ERPSEI.Services.Compliance
{
    public static class MapeoDocumentalEmpresasCompliance
    {
        /*
         * ==========================================================
         * EMPRESAS → COMPLIANCE
         * ==========================================================
         *
         * Estos documentos pueden viajar desde Empresas hacia
         * Compliance.
         *
         * IMPORTANTE:
         * INE2 e INE3 NO se sincronizan.
         * Únicamente INE (TipoArchivoId = 2).
         */
        private static readonly IReadOnlyDictionary<int, int>
            EmpresaACompliance =
                new Dictionary<int, int>
                {
                    [1] = 1,   // CSF → Constancia de Situación Fiscal

                    [2] = 7,   // INE → INE de accionistas
                               // SOLO la primera INE de Empresas

                    [4] = 3,   // ComprobanteDomicilio
                    [5] = 15,  // Otro
                    [6] = 2,   // CER → Certificado FIEL
                    [9] = 10,  // Hoja membretada
                    [12] = 11, // Organigrama
                    [13] = 4   // Acta constitutiva
                };

        /*
         * ==========================================================
         * COMPLIANCE → EMPRESAS
         * ==========================================================
         *
         * Aquí las equivalencias son explícitas.
         *
         * NO incluimos Compliance 7 (INE de accionistas)
         * porque Compliance permite múltiples INEs y Empresas
         * solamente tiene un campo INE principal que queremos
         * sincronizar.
         *
         * Esto evita sobrescribir INE, INE2 o INE3 de forma
         * ambigua.
         */
        private static readonly IReadOnlyDictionary<int, int>
            ComplianceAEmpresa =
                new Dictionary<int, int>
                {
                    [1] = 1,   // Constancia Fiscal → CSF
                    [3] = 4,   // Comprobante domicilio
                    [15] = 5,  // Otro
                    [2] = 6,   // Certificado FIEL → CER
                    [10] = 9,  // Hoja membretada
                    [11] = 12, // Organigrama
                    [4] = 13   // Acta constitutiva
                };

        public static int? ObtenerTipoCompliance(
            int tipoArchivoEmpresaId)
        {
            if (
                EmpresaACompliance.TryGetValue(
                    tipoArchivoEmpresaId,
                    out int tipoDocumentoComplianceId
                )
            )
            {
                return tipoDocumentoComplianceId;
            }

            return null;
        }

        public static int? ObtenerTipoEmpresa(
            int tipoDocumentoComplianceId)
        {
            if (
                ComplianceAEmpresa.TryGetValue(
                    tipoDocumentoComplianceId,
                    out int tipoArchivoEmpresaId
                )
            )
            {
                return tipoArchivoEmpresaId;
            }

            return null;
        }

        public static bool PuedeSincronizarDesdeEmpresa(
            int tipoArchivoEmpresaId)
        {
            return EmpresaACompliance.ContainsKey(
                tipoArchivoEmpresaId
            );
        }

        public static bool PuedeSincronizarDesdeCompliance(
            int tipoDocumentoComplianceId)
        {
            return ComplianceAEmpresa.ContainsKey(
                tipoDocumentoComplianceId
            );
        }

        public static IReadOnlyDictionary<int, int>
            ObtenerEquivalencias()
        {
            return EmpresaACompliance;
        }
    }
}