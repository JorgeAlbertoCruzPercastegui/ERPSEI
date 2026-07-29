using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCatalogoInicialExpedientesBancarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Id",
                    "Categoria",
                    "Descripcion",
                    "EsObligatorio",
                    "FechaActualizacion",
                    "FechaCreacion",
                    "Nombre",
                    "Orden",
                    "UsuarioCreacionId"
                },
                values: new object[]
                {
                    1,
                    "Fiscal",
                    "Constancia de Situación Fiscal vigente de la empresa.",
                    true,
                    null,
                    new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                    "Constancia de Situación Fiscal",
                    1,
                    "SYSTEM"
                });

            migrationBuilder.InsertData(
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Id",
                    "Categoria",
                    "Descripcion",
                    "EsObligatorio",
                    "FechaActualizacion",
                    "FechaCreacion",
                    "Nombre",
                    "Orden",
                    "RequiereFechaVencimiento",
                    "UsuarioCreacionId"
                },
                values: new object[]
                {
                    2,
                    "Fiscal",
                    "Certificado de firma electrónica vigente.",
                    true,
                    null,
                    new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                    "Certificado FIEL",
                    2,
                    true,
                    "SYSTEM"
                });

            migrationBuilder.InsertData(
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Id",
                    "Categoria",
                    "Descripcion",
                    "EsObligatorio",
                    "FechaActualizacion",
                    "FechaCreacion",
                    "Nombre",
                    "Orden",
                    "PermiteMultiplesArchivos",
                    "RequiereFechaVencimiento",
                    "UsuarioCreacionId"
                },
                values: new object[]
                {
                    3,
                    "Domicilio",
                    "Comprobante de domicilio fiscal o comercial.",
                    true,
                    null,
                    new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                    "Comprobante de domicilio",
                    3,
                    true,
                    true,
                    "SYSTEM"
                });

            migrationBuilder.InsertData(
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Id",
                    "Categoria",
                    "Descripcion",
                    "EsObligatorio",
                    "FechaActualizacion",
                    "FechaCreacion",
                    "Nombre",
                    "Orden",
                    "UsuarioCreacionId"
                },
                values: new object[]
                {
                    4,
                    "Corporativo",
                    "Acta constitutiva de la sociedad.",
                    true,
                    null,
                    new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                    "Acta constitutiva",
                    4,
                    "SYSTEM"
                });

            migrationBuilder.InsertData(
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Id",
                    "Categoria",
                    "Descripcion",
                    "FechaActualizacion",
                    "FechaCreacion",
                    "Nombre",
                    "Orden",
                    "PermiteMultiplesArchivos",
                    "UsuarioCreacionId"
                },
                values: new object[,]
                {
                    {
                        5,
                        "Corporativo",
                        "Reformas, protocolizaciones o instrumentos adicionales.",
                        null,
                        new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                        "Actas o instrumentos adicionales",
                        5,
                        true,
                        "SYSTEM"
                    },
                    {
                        6,
                        "Legal",
                        "Poderes notariales vigentes de representantes o apoderados.",
                        null,
                        new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                        "Poder notarial",
                        6,
                        true,
                        "SYSTEM"
                    }
                });

            migrationBuilder.InsertData(
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Id",
                    "Categoria",
                    "Descripcion",
                    "EsObligatorio",
                    "FechaActualizacion",
                    "FechaCreacion",
                    "Nombre",
                    "Orden",
                    "PermiteMultiplesArchivos",
                    "RequiereFechaVencimiento",
                    "UsuarioCreacionId"
                },
                values: new object[]
                {
                    7,
                    "Accionistas",
                    "Identificación oficial de los accionistas.",
                    true,
                    null,
                    new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                    "INE de accionistas",
                    7,
                    true,
                    true,
                    "SYSTEM"
                });

            migrationBuilder.InsertData(
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Id",
                    "Categoria",
                    "Descripcion",
                    "EsObligatorio",
                    "FechaActualizacion",
                    "FechaCreacion",
                    "Nombre",
                    "Orden",
                    "PermiteMultiplesArchivos",
                    "UsuarioCreacionId"
                },
                values: new object[]
                {
                    8,
                    "Accionistas",
                    "Constancia de Situación Fiscal de cada accionista.",
                    true,
                    null,
                    new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                    "CSF de accionistas",
                    8,
                    true,
                    "SYSTEM"
                });

            migrationBuilder.InsertData(
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Id",
                    "Categoria",
                    "Descripcion",
                    "EsObligatorio",
                    "FechaActualizacion",
                    "FechaCreacion",
                    "Nombre",
                    "Orden",
                    "PermiteMultiplesArchivos",
                    "RequiereFechaVencimiento",
                    "UsuarioCreacionId"
                },
                values: new object[]
                {
                    9,
                    "Accionistas",
                    "Comprobante de domicilio de cada accionista.",
                    true,
                    null,
                    new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                    "Comprobante de domicilio de accionistas",
                    9,
                    true,
                    true,
                    "SYSTEM"
                });

            migrationBuilder.InsertData(
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Id",
                    "Categoria",
                    "Descripcion",
                    "FechaActualizacion",
                    "FechaCreacion",
                    "Nombre",
                    "Orden",
                    "UsuarioCreacionId"
                },
                values: new object[]
                {
                    10,
                    "Corporativo",
                    "Hoja membretada vigente de la empresa.",
                    null,
                    new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                    "Hoja membretada",
                    10,
                    "SYSTEM"
                });

            migrationBuilder.InsertData(
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Id",
                    "Categoria",
                    "Descripcion",
                    "EsObligatorio",
                    "FechaActualizacion",
                    "FechaCreacion",
                    "Nombre",
                    "Orden",
                    "UsuarioCreacionId"
                },
                values: new object[]
                {
                    11,
                    "Corporativo",
                    "Organigrama actualizado de la empresa.",
                    true,
                    null,
                    new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                    "Organigrama",
                    11,
                    "SYSTEM"
                });

            migrationBuilder.InsertData(
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Id",
                    "Categoria",
                    "Descripcion",
                    "EsObligatorio",
                    "FechaActualizacion",
                    "FechaCreacion",
                    "Nombre",
                    "Orden",
                    "PermiteMultiplesArchivos",
                    "UsuarioCreacionId"
                },
                values: new object[]
                {
                    12,
                    "Financiero",
                    "Última declaración anual o mensual disponible.",
                    true,
                    null,
                    new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                    "Declaración anual o mensual",
                    12,
                    true,
                    "SYSTEM"
                });

            migrationBuilder.InsertData(
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Id",
                    "Categoria",
                    "Descripcion",
                    "EsObligatorio",
                    "FechaActualizacion",
                    "FechaCreacion",
                    "Nombre",
                    "Orden",
                    "RequiereFechaVencimiento",
                    "UsuarioCreacionId"
                },
                values: new object[]
                {
                    13,
                    "Fiscal",
                    "Constancia de opinión de cumplimiento emitida por el SAT.",
                    true,
                    null,
                    new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                    "Opinión de cumplimiento SAT",
                    13,
                    true,
                    "SYSTEM"
                });

            migrationBuilder.InsertData(
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Id",
                    "Categoria",
                    "Descripcion",
                    "FechaActualizacion",
                    "FechaCreacion",
                    "Nombre",
                    "Orden",
                    "PermiteMultiplesArchivos",
                    "UsuarioCreacionId"
                },
                values: new object[,]
                {
                    {
                        14,
                        "Evidencias",
                        "Imágenes o evidencias solicitadas por instituciones bancarias.",
                        null,
                        new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                        "Prueba de vida",
                        14,
                        true,
                        "SYSTEM"
                    },
                    {
                        15,
                        "Otros",
                        "Documentación adicional requerida por la institución.",
                        null,
                        new DateTime(2026, 7, 29, 0, 0, 0, DateTimeKind.Unspecified),
                        "Otro documento",
                        15,
                        true,
                        "SYSTEM"
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "EB_TiposDocumento",
                keyColumn: "Id",
                keyValue: 15);
        }
    }
}