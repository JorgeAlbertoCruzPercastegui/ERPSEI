namespace ERPSEI.Services.Compliance
{
    public static class MapeoDocumentalEmpresasCompliance
    {
        /*
         * ==========================================================
         * EMPRESAS → COMPLIANCE
         * ==========================================================
         *
         * TipoArchivoEmpresa.Id → EbTipoDocumento.Id
         *
         * IMPORTANTE:
         *
         * 12 = Acta Constitutiva
         * 13 = Organigrama
         *
         * El INE general de Empresas (Tipo 2) ya NO se sincroniza
         * como INE de accionistas.
         *
         * Para ello ahora existe específicamente:
         *
         * 16 = INEAccionistas
         *      ↕
         * 7  = INE accionistas
         * ==========================================================
         */
        private static readonly
            IReadOnlyDictionary<int, int>
            EmpresaACompliance =
                new Dictionary<int, int>
                {
                    [1] = 1,
                    [2] = 7,
                    [4] = 3,
                    [5] = 15,
                    [6] = 2,
                    [9] = 10,
                    [12] = 4,
                    [13] = 11,
                    [14] = 5,
                    [15] = 6,
                    [17] = 8,
                    [18] = 9,
                    [19] = 12,
                    [20] = 13,
                    [21] = 14
                };

        /*
         * ==========================================================
         * COMPLIANCE → EMPRESAS
         * ==========================================================
         *
         * EbTipoDocumento.Id → TipoArchivoEmpresa.Id
         *
         * Ahora todos los documentos que tienen una equivalencia
         * real pueden sincronizarse también en sentido inverso.
         * ==========================================================
         */
        private static readonly
            IReadOnlyDictionary<int, int>
            ComplianceAEmpresa =
                new Dictionary<int, int>
                {
                    [1] = 1,
                    [2] = 6,
                    [3] = 4,
                    [4] = 12,
                    [5] = 14,
                    [6] = 15,

                    [7] = 2,

                    [8] = 17,
                    [9] = 18,
                    [10] = 9,
                    [11] = 13,
                    [12] = 19,
                    [13] = 20,
                    [14] = 21,
                    [15] = 5
                };

        public static int?
            ObtenerTipoCompliance(
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

        public static int?
            ObtenerTipoEmpresa(
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

        public static bool
            PuedeSincronizarDesdeEmpresa(
                int tipoArchivoEmpresaId)
        {
            return EmpresaACompliance.ContainsKey(
                tipoArchivoEmpresaId
            );
        }

        public static bool
            PuedeSincronizarDesdeCompliance(
                int tipoDocumentoComplianceId)
        {
            return ComplianceAEmpresa.ContainsKey(
                tipoDocumentoComplianceId
            );
        }

        public static
            IReadOnlyDictionary<int, int>
            ObtenerEquivalencias()
        {
            return EmpresaACompliance;
        }
    }
}