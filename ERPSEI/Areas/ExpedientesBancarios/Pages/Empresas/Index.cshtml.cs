using ERPSEI.Data;
using ERPSEI.Data.Entities.ExpedientesBancarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ERPSEI.Areas.ExpedientesBancarios.Pages.Empresas
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public IndexModel(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
        }

        public void OnGet()
        {
        }

        // =====================================================
        // LISTADO PARA BOOTSTRAP TABLE
        // GET ?handler=Empresas
        // =====================================================
        public async Task<IActionResult> OnGetEmpresasAsync(
            string? busqueda,
            string? estatus)
        {
            string filtro = busqueda?.Trim() ?? string.Empty;
            string filtroEstatus = string.IsNullOrWhiteSpace(estatus)
                ? "Activas"
                : estatus.Trim();

            IQueryable<EbEmpresa> query = _context.EbEmpresas
                .AsNoTracking();

            query = filtroEstatus switch
            {
                "Inactivas" => query.Where(x => x.Deshabilitado),
                "Todas" => query,
                _ => query.Where(x => !x.Deshabilitado)
            };

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(x =>
                    x.RazonSocial.Contains(filtro) ||
                    x.NombreCorto.Contains(filtro) ||
                    x.Rfc.Contains(filtro) ||
                    (x.Nivel != null && x.Nivel.Contains(filtro)) ||
                    (x.ActividadComercial != null &&
                     x.ActividadComercial.Contains(filtro)));
            }

            var empresas = await query
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    id = x.Id,
                    razonSocial = x.RazonSocial,
                    nombreCorto = x.NombreCorto,
                    rfc = x.Rfc,
                    nivel = x.Nivel,
                    actividadComercial = x.ActividadComercial,
                    telefonoBancos = x.TelefonoBancos,
                    correoBancos = x.CorreoBancos,
                    fechaConstitucion = x.FechaConstitucion,
                    numeroEscritura = x.NumeroEscritura,
                    domicilioFiscal = x.DomicilioFiscal,
                    observaciones = x.Observaciones,
                    deshabilitado = x.Deshabilitado,
                    fechaCreacion = x.FechaCreacion
                })
                .ToListAsync();

            return new JsonResult(empresas);
        }

        // =====================================================
        // CONSULTAR REGISTRO
        // GET ?handler=Empresa&id=1
        // =====================================================
        public async Task<IActionResult> OnGetEmpresaAsync(int id)
        {
            var empresa = await _context.EbEmpresas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (empresa == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa solicitada."
                });
            }

            return new JsonResult(new
            {
                success = true,
                data = new
                {
                    id = empresa.Id,
                    razonSocial = empresa.RazonSocial,
                    nombreCorto = empresa.NombreCorto,
                    rfc = empresa.Rfc,
                    nivel = empresa.Nivel,
                    actividadComercial = empresa.ActividadComercial,
                    telefonoBancos = empresa.TelefonoBancos,
                    correoBancos = empresa.CorreoBancos,
                    fechaConstitucion = empresa.FechaConstitucion?
                        .ToString("yyyy-MM-dd"),
                    numeroEscritura = empresa.NumeroEscritura,
                    domicilioFiscal = empresa.DomicilioFiscal,
                    observaciones = empresa.Observaciones,
                    deshabilitado = empresa.Deshabilitado
                }
            });
        }

        // =====================================================
        // CREAR EMPRESA
        // POST ?handler=Crear
        // =====================================================
        public async Task<IActionResult> OnPostCrearAsync(
            [FromBody] EmpresaRequest request)
        {
            NormalizarRequest(request);

            Dictionary<string, string[]> errores =
                ValidarRequest(request, requiereId: false);

            if (errores.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Revisa la información capturada.",
                    errors = errores
                });
            }

            bool rfcExistente = await _context.EbEmpresas
                .IgnoreQueryFilters()
                .AnyAsync(x => x.Rfc == request.Rfc);

            if (rfcExistente)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Ya existe una empresa registrada con este RFC.",
                    errors = new Dictionary<string, string[]>
                    {
                        ["Rfc"] = new[]
                        {
                            "Ya existe una empresa registrada con este RFC."
                        }
                    }
                });
            }

            string usuarioId = ObtenerUsuarioId();

            var empresa = new EbEmpresa
            {
                RazonSocial = request.RazonSocial,
                NombreCorto = request.NombreCorto,
                Rfc = request.Rfc,
                Nivel = request.Nivel,
                ActividadComercial = request.ActividadComercial,
                TelefonoBancos = request.TelefonoBancos,
                CorreoBancos = request.CorreoBancos,
                FechaConstitucion = request.FechaConstitucion,
                NumeroEscritura = request.NumeroEscritura,
                DomicilioFiscal = request.DomicilioFiscal,
                Observaciones = request.Observaciones,
                Deshabilitado = false,
                Eliminado = false,
                FechaCreacion = DateTime.Now,
                UsuarioCreacionId = usuarioId
            };

            _context.EbEmpresas.Add(empresa);
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "La empresa se registró correctamente.",
                id = empresa.Id
            });
        }

        // =====================================================
        // EDITAR EMPRESA
        // POST ?handler=Editar
        // =====================================================
        public async Task<IActionResult> OnPostEditarAsync(
            [FromBody] EmpresaRequest request)
        {
            NormalizarRequest(request);

            Dictionary<string, string[]> errores =
                ValidarRequest(request, requiereId: true);

            if (errores.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Revisa la información capturada.",
                    errors = errores
                });
            }

            var empresa = await _context.EbEmpresas
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (empresa == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa que deseas editar."
                });
            }

            bool rfcExistente = await _context.EbEmpresas
                .IgnoreQueryFilters()
                .AnyAsync(x =>
                    x.Rfc == request.Rfc &&
                    x.Id != request.Id);

            if (rfcExistente)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El RFC ya está asignado a otra empresa.",
                    errors = new Dictionary<string, string[]>
                    {
                        ["Rfc"] = new[]
                        {
                            "El RFC ya está asignado a otra empresa."
                        }
                    }
                });
            }

            empresa.RazonSocial = request.RazonSocial;
            empresa.NombreCorto = request.NombreCorto;
            empresa.Rfc = request.Rfc;
            empresa.Nivel = request.Nivel;
            empresa.ActividadComercial = request.ActividadComercial;
            empresa.TelefonoBancos = request.TelefonoBancos;
            empresa.CorreoBancos = request.CorreoBancos;
            empresa.FechaConstitucion = request.FechaConstitucion;
            empresa.NumeroEscritura = request.NumeroEscritura;
            empresa.DomicilioFiscal = request.DomicilioFiscal;
            empresa.Observaciones = request.Observaciones;
            empresa.FechaActualizacion = DateTime.Now;
            empresa.UsuarioActualizacionId = ObtenerUsuarioId();

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "La empresa se actualizó correctamente."
            });
        }

        // =====================================================
        // HABILITAR / DESHABILITAR
        // POST ?handler=CambiarEstatus
        // =====================================================
        public async Task<IActionResult> OnPostCambiarEstatusAsync(
            [FromBody] EmpresaIdRequest request)
        {
            if (request.Id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador de la empresa no es válido."
                });
            }

            var empresa = await _context.EbEmpresas
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (empresa == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa solicitada."
                });
            }

            empresa.Deshabilitado = !empresa.Deshabilitado;
            empresa.FechaActualizacion = DateTime.Now;
            empresa.UsuarioActualizacionId = ObtenerUsuarioId();

            await _context.SaveChangesAsync();

            string mensaje = empresa.Deshabilitado
                ? "La empresa se deshabilitó correctamente."
                : "La empresa se habilitó correctamente.";

            return new JsonResult(new
            {
                success = true,
                message = mensaje,
                deshabilitado = empresa.Deshabilitado
            });
        }

        // =====================================================
        // ELIMINACIÓN LÓGICA
        // POST ?handler=Eliminar
        // =====================================================
        public async Task<IActionResult> OnPostEliminarAsync(
            [FromBody] EmpresaIdRequest request)
        {
            if (request.Id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador de la empresa no es válido."
                });
            }

            var empresa = await _context.EbEmpresas
                .Include(x => x.Accionistas)
                .Include(x => x.Documentos)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (empresa == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa solicitada."
                });
            }

            bool tieneInformacionRelacionada =
                empresa.Accionistas.Any() ||
                empresa.Documentos.Any();

            if (tieneInformacionRelacionada)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "La empresa tiene información relacionada. " +
                        "Puedes deshabilitarla, pero no eliminarla."
                });
            }

            empresa.Eliminado = true;
            empresa.Deshabilitado = true;
            empresa.FechaActualizacion = DateTime.Now;
            empresa.UsuarioActualizacionId = ObtenerUsuarioId();

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "La empresa se eliminó correctamente."
            });
        }

        // =====================================================
        // DOCUMENTOS DE UNA EMPRESA
        // GET ?handler=Documentos&empresaId=1
        // =====================================================
        public async Task<IActionResult> OnGetDocumentosAsync(
            int empresaId)
        {
            if (empresaId <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "El identificador de la empresa no es válido."
                });
            }

            bool empresaExiste = await _context.EbEmpresas
                .AsNoTracking()
                .AnyAsync(x => x.Id == empresaId);

            if (!empresaExiste)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "No se encontró la empresa solicitada."
                });
            }

            DateTime fechaActual = DateTime.Today;
            DateTime fechaProximaVencimiento =
                fechaActual.AddDays(30);

            var tiposDocumento = await _context
                .EbTiposDocumento
                .AsNoTracking()
                .Where(x =>
                    !x.Eliminado &&
                    !x.Deshabilitado)
                .OrderBy(x => x.Orden)
                .ThenBy(x => x.Nombre)
                .Select(x => new
                {
                    id = x.Id,
                    nombre = x.Nombre,
                    categoria = x.Categoria,
                    descripcion = x.Descripcion,
                    esObligatorio = x.EsObligatorio,
                    requiereFechaVencimiento =
                        x.RequiereFechaVencimiento,
                    permiteMultiplesArchivos =
                        x.PermiteMultiplesArchivos,
                    orden = x.Orden
                })
                .ToListAsync();

            var documentosEmpresa = await _context
                .EbDocumentos
                .AsNoTracking()
                .Where(x =>
                    x.EmpresaId == empresaId &&
                    !x.Eliminado &&
                    x.EsVersionActual)
                .OrderByDescending(x => x.FechaCarga)
                .Select(x => new
                {
                    id = x.Id,
                    empresaId = x.EmpresaId,
                    tipoDocumentoId = x.TipoDocumentoId,
                    nombreOriginal = x.NombreOriginal,
                    nombreAlmacenado = x.NombreAlmacenado,
                    rutaArchivo = x.RutaArchivo,
                    extension = x.Extension,
                    mimeType = x.MimeType,
                    tamanoBytes = x.TamanoBytes,
                    version = x.Version,
                    fechaCarga = x.FechaCarga,
                    fechaVencimiento = x.FechaVencimiento,
                    estado = x.Estado,
                    observaciones = x.Observaciones
                })
                .ToListAsync();

            var documentos = tiposDocumento
                .Select(tipo =>
                {
                    var archivos = documentosEmpresa
                        .Where(x =>
                            x.tipoDocumentoId == tipo.id)
                        .OrderByDescending(x => x.fechaCarga)
                        .ToList();

                    string estatus;

                    if (archivos.Count == 0)
                    {
                        estatus = "Pendiente";
                    }
                    else if (!tipo.requiereFechaVencimiento)
                    {
                        estatus = "Cargado";
                    }
                    else
                    {
                        bool tieneVencidos = archivos.Any(x =>
                            x.fechaVencimiento.HasValue &&
                            x.fechaVencimiento.Value.Date <
                            fechaActual);

                        bool tieneProximosAVencer = archivos.Any(x =>
                            x.fechaVencimiento.HasValue &&
                            x.fechaVencimiento.Value.Date >=
                            fechaActual &&
                            x.fechaVencimiento.Value.Date <=
                            fechaProximaVencimiento);

                        bool tieneVigentes = archivos.Any(x =>
                            x.fechaVencimiento.HasValue &&
                            x.fechaVencimiento.Value.Date >
                            fechaProximaVencimiento);

                        if (tieneVencidos)
                        {
                            estatus = "Vencido";
                        }
                        else if (tieneProximosAVencer)
                        {
                            estatus = "Próximo a vencer";
                        }
                        else if (tieneVigentes)
                        {
                            estatus = "Vigente";
                        }
                        else
                        {
                            /*
                             * Existe un archivo, pero no tiene fecha
                             * de vencimiento capturada.
                             */
                            estatus = "Cargado";
                        }
                    }

                    return new
                    {
                        id = tipo.id,
                        nombre = tipo.nombre,
                        categoria = tipo.categoria,
                        descripcion = tipo.descripcion,
                        obligatorio = tipo.esObligatorio,
                        requiereFechaVencimiento =
                            tipo.requiereFechaVencimiento,
                        permiteMultiples =
                            tipo.permiteMultiplesArchivos,
                        orden = tipo.orden,
                        estatus,
                        totalArchivos = archivos.Count,

                        archivos = archivos.Select(archivo => new
                        {
                            id = archivo.id,
                            tipoDocumentoId =
                                archivo.tipoDocumentoId,
                            nombreOriginal =
                                archivo.nombreOriginal,
                            extension = archivo.extension,
                            mimeType = archivo.mimeType,
                            tamanoBytes = archivo.tamanoBytes,
                            version = archivo.version,
                            fechaCarga = archivo.fechaCarga,
                            fechaVencimiento =
                                archivo.fechaVencimiento,
                            estado = archivo.estado,
                            observaciones =
                                archivo.observaciones
                        })
                        .ToList()
                    };
                })
                .ToList();

            int totalRequeridos = documentos.Count(x =>
                x.obligatorio);

            int totalCargados = documentos.Count(x =>
                x.totalArchivos > 0);

            int totalPendientes = documentos.Count(x =>
                x.obligatorio &&
                x.totalArchivos == 0);

            int totalVencidos = documentos.Count(x =>
                x.estatus == "Vencido");

            int totalProximosAVencer = documentos.Count(x =>
                x.estatus == "Próximo a vencer");

            return new JsonResult(new
            {
                success = true,

                data = documentos,

                resumen = new
                {
                    totalDocumentos = documentos.Count,
                    totalRequeridos,
                    totalCargados,
                    totalPendientes,
                    totalVencidos,
                    totalProximosAVencer
                }
            });
        }

        // =====================================================
        // VISUALIZAR DOCUMENTO
        // GET ?handler=VisualizarDocumento&id=1
        // =====================================================
        public async Task<IActionResult> OnGetVisualizarDocumentoAsync(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var documento = await _context.EbDocumentos
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    !x.Eliminado &&
                    x.EsVersionActual);

            if (documento == null)
            {
                return NotFound();
            }

            string? rutaBaseDocumentos =
                _configuration[
                    "ExpedientesBancarios:RutaDocumentos"
                ];

            if (string.IsNullOrWhiteSpace(rutaBaseDocumentos))
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError
                );
            }

            string rutaBaseCompleta = Path.GetFullPath(
                rutaBaseDocumentos
            );

            string rutaFisica = Path.GetFullPath(
                Path.Combine(
                    rutaBaseCompleta,
                    documento.RutaArchivo
                        .Replace("/", Path.DirectorySeparatorChar.ToString())
                )
            );

            /*
             * Evita acceder a rutas fuera del directorio configurado.
             */
            string rutaBaseConSeparador =
                rutaBaseCompleta.TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                ) + Path.DirectorySeparatorChar;

            if (!rutaFisica.StartsWith(
                    rutaBaseConSeparador,
                    StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest();
            }

            if (!System.IO.File.Exists(rutaFisica))
            {
                return NotFound();
            }

            string mimeType = string.IsNullOrWhiteSpace(
                documento.MimeType)
                    ? "application/octet-stream"
                    : documento.MimeType;

            /*
             * Al no enviar downloadFileName, el navegador intenta
             * mostrar el archivo en línea.
             */
            return PhysicalFile(
            rutaFisica,
            mimeType
);
        }

        // =====================================================
        // CARGAR DOCUMENTO
        // POST ?handler=CargarDocumento
        // =====================================================
        [RequestSizeLimit(104857600)]
        public async Task<IActionResult> OnPostCargarDocumentoAsync(
            [FromForm] CargarDocumentoRequest request)
        {
            const long tamanoMaximo = 25L * 1024L * 1024L;

            string? rutaFisica = null;

            try
            {
                string[] extensionesPermitidas =
                {
            ".pdf",
            ".doc",
            ".docx",
            ".xls",
            ".xlsx",
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".zip"
        };

                if (request.EmpresaId <= 0)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "La empresa seleccionada no es válida."
                    });
                }

                if (request.TipoDocumentoId <= 0)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "El tipo de documento no es válido."
                    });
                }

                if (request.Archivo == null ||
                    request.Archivo.Length <= 0)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Selecciona un archivo para cargar.",
                        errors = new Dictionary<string, string[]>
                        {
                            ["Archivo"] = new[]
                            {
                        "El archivo es obligatorio."
                    }
                        }
                    });
                }

                if (request.Archivo.Length > tamanoMaximo)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "El archivo excede el tamaño máximo permitido.",
                        errors = new Dictionary<string, string[]>
                        {
                            ["Archivo"] = new[]
                            {
                        "El archivo no puede superar los 25 MB."
                    }
                        }
                    });
                }

                string extension = Path
                    .GetExtension(request.Archivo.FileName)
                    .ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(extension) ||
                    !extensionesPermitidas.Contains(extension))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "El tipo de archivo no está permitido.",
                        errors = new Dictionary<string, string[]>
                        {
                            ["Archivo"] = new[]
                            {
                        "Formatos permitidos: PDF, Word, Excel, " +
                        "JPG, PNG, WEBP y ZIP."
                    }
                        }
                    });
                }

                bool empresaExiste = await _context.EbEmpresas
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id == request.EmpresaId &&
                        !x.Deshabilitado);

                if (!empresaExiste)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "No se encontró la empresa seleccionada."
                    });
                }

                var tipoDocumento = await _context.EbTiposDocumento
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.TipoDocumentoId &&
                        !x.Eliminado &&
                        !x.Deshabilitado);

                if (tipoDocumento == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "No se encontró el tipo de documento seleccionado."
                    });
                }

                if (tipoDocumento.RequiereFechaVencimiento &&
                    !request.FechaVencimiento.HasValue)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "El documento requiere una fecha de vencimiento.",
                        errors = new Dictionary<string, string[]>
                        {
                            ["FechaVencimiento"] = new[]
                            {
                        "La fecha de vencimiento es obligatoria."
                    }
                        }
                    });
                }

                if (request.FechaVencimiento.HasValue &&
                    request.FechaVencimiento.Value.Date < DateTime.Today)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "La fecha de vencimiento no puede ser anterior a hoy.",
                        errors = new Dictionary<string, string[]>
                        {
                            ["FechaVencimiento"] = new[]
                            {
                        "Selecciona una fecha válida."
                    }
                        }
                    });
                }

                string nombreOriginal = Path.GetFileName(
                    request.Archivo.FileName
                );

                string nombreAlmacenado =
                    $"{Guid.NewGuid():N}{extension}";

                string? rutaBaseDocumentos =
    _configuration[
        "ExpedientesBancarios:RutaDocumentos"
    ];

                if (string.IsNullOrWhiteSpace(rutaBaseDocumentos))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "No se encuentra configurada la ruta de almacenamiento documental."
                    })
                    {
                        StatusCode =
                            StatusCodes.Status500InternalServerError
                    };
                }

                rutaBaseDocumentos = Path.GetFullPath(
                    rutaBaseDocumentos
                );

                string directorioFisico = Path.Combine(
                    rutaBaseDocumentos,
                    request.EmpresaId.ToString(),
                    request.TipoDocumentoId.ToString()
                );

                Console.WriteLine(
                    $"Directorio documental: {directorioFisico}"
                );

                Directory.CreateDirectory(
                    directorioFisico
                );

                rutaFisica = Path.Combine(
                    directorioFisico,
                    nombreAlmacenado
                );

                /*
                 * Esta ruta es la referencia interna que se almacena
                 * en la base de datos. No incluye la ruta física raíz.
                 */
                string rutaRelativa = Path.Combine(
                    request.EmpresaId.ToString(),
                    request.TipoDocumentoId.ToString(),
                    nombreAlmacenado
                ).Replace("\\", "/");

                var documentosActuales =
                    await _context.EbDocumentos
                        .Where(x =>
                            x.EmpresaId == request.EmpresaId &&
                            x.TipoDocumentoId ==
                                request.TipoDocumentoId &&
                            !x.Eliminado &&
                            x.EsVersionActual)
                        .ToListAsync();

                int version = 1;

                if (!tipoDocumento.PermiteMultiplesArchivos)
                {
                    version = documentosActuales.Any()
                        ? documentosActuales.Max(x => x.Version) + 1
                        : 1;

                    foreach (EbDocumento documentoAnterior
                             in documentosActuales)
                    {
                        documentoAnterior.EsVersionActual = false;
                    }
                }

                await using (
                    var stream = new FileStream(
                        rutaFisica,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None
                    )
                )
                {
                    await request.Archivo.CopyToAsync(
                        stream
                    );
                }

                string estado = CalcularEstadoDocumento(
                    request.FechaVencimiento,
                    tipoDocumento.RequiereFechaVencimiento
                );

                var documento = new EbDocumento
                {
                    EmpresaId = request.EmpresaId,
                    TipoDocumentoId = request.TipoDocumentoId,

                    NombreOriginal = nombreOriginal,
                    NombreAlmacenado = nombreAlmacenado,
                    RutaArchivo = rutaRelativa,

                    Extension = extension,
                    MimeType = string.IsNullOrWhiteSpace(
                        request.Archivo.ContentType)
                            ? "application/octet-stream"
                            : request.Archivo.ContentType,

                    TamanoBytes = request.Archivo.Length,
                    Version = version,

                    FechaCarga = DateTime.Now,
                    FechaVencimiento =
                        request.FechaVencimiento,

                    Estado = estado,

                    Observaciones = NormalizarOpcional(
                        request.Observaciones
                    ),

                    EsVersionActual = true,
                    Eliminado = false,

                    UsuarioCargaId = ObtenerUsuarioId()
                };

                _context.EbDocumentos.Add(documento);

                await _context.SaveChangesAsync();

                return new JsonResult(new
                {
                    success = true,
                    message =
                        "El documento se cargó correctamente.",
                    id = documento.Id
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "======================================"
                );

                Console.WriteLine(
                    "ERROR AL CARGAR DOCUMENTO"
                );

                Console.WriteLine(
                    ex.ToString()
                );

                Console.WriteLine(
                    "======================================"
                );

                if (!string.IsNullOrWhiteSpace(rutaFisica) &&
                    System.IO.File.Exists(rutaFisica))
                {
                    try
                    {
                        System.IO.File.Delete(rutaFisica);
                    }
                    catch (Exception errorEliminar)
                    {
                        Console.WriteLine(
                            "No se pudo eliminar el archivo incompleto:"
                        );

                        Console.WriteLine(
                            errorEliminar.ToString()
                        );
                    }
                }

                return new JsonResult(new
                {
                    success = false,
                    message =
                        "Ocurrió un error al guardar el documento.",
                    detail = _environment.IsDevelopment()
                        ? ex.Message
                        : null
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        private static string CalcularEstadoDocumento(
            DateTime? fechaVencimiento,
            bool requiereFechaVencimiento)
        {
            if (!requiereFechaVencimiento)
            {
                return "Cargado";
            }

            if (!fechaVencimiento.HasValue)
            {
                return "Cargado";
            }

            DateTime fechaActual = DateTime.Today;
            DateTime fechaLimite = fechaActual.AddDays(30);

            if (fechaVencimiento.Value.Date < fechaActual)
            {
                return "Vencido";
            }

            if (fechaVencimiento.Value.Date <= fechaLimite)
            {
                return "Próximo a vencer";
            }

            return "Vigente";
        }

        // =====================================================
        // VALIDACIONES
        // =====================================================
        private static Dictionary<string, string[]> ValidarRequest(
            EmpresaRequest request,
            bool requiereId)
        {
            var errores = new Dictionary<string, string[]>();

            if (requiereId && request.Id <= 0)
            {
                errores["Id"] = new[]
                {
                    "El identificador de la empresa no es válido."
                };
            }

            if (string.IsNullOrWhiteSpace(request.RazonSocial))
            {
                errores["RazonSocial"] = new[]
                {
                    "La razón social es obligatoria."
                };
            }
            else if (request.RazonSocial.Length > 250)
            {
                errores["RazonSocial"] = new[]
                {
                    "La razón social no puede exceder 250 caracteres."
                };
            }

            if (string.IsNullOrWhiteSpace(request.NombreCorto))
            {
                errores["NombreCorto"] = new[]
                {
                    "El nombre corto es obligatorio."
                };
            }
            else if (request.NombreCorto.Length > 150)
            {
                errores["NombreCorto"] = new[]
                {
                    "El nombre corto no puede exceder 150 caracteres."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Rfc))
            {
                errores["Rfc"] = new[]
                {
                    "El RFC es obligatorio."
                };
            }
            else
            {
                var atributoRfc = new RegularExpressionAttribute(
                    @"^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$");

                if (request.Rfc.Length is < 12 or > 13 ||
                    !atributoRfc.IsValid(request.Rfc))
                {
                    errores["Rfc"] = new[]
                    {
                        "El formato del RFC no es válido."
                    };
                }
            }

            if (!string.IsNullOrWhiteSpace(request.CorreoBancos))
            {
                var atributoCorreo = new EmailAddressAttribute();

                if (!atributoCorreo.IsValid(request.CorreoBancos))
                {
                    errores["CorreoBancos"] = new[]
                    {
                        "El correo electrónico no es válido."
                    };
                }
            }

            if (request.FechaConstitucion.HasValue &&
                request.FechaConstitucion.Value.Date > DateTime.Today)
            {
                errores["FechaConstitucion"] = new[]
                {
                    "La fecha de constitución no puede ser futura."
                };
            }

            return errores;
        }

        // =====================================================
        // LISTAR ACCIONISTAS DE UNA EMPRESA
        // GET ?handler=Accionistas&empresaId=1
        // =====================================================
        public async Task<IActionResult> OnGetAccionistasAsync(int empresaId)
        {
            if (empresaId <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador de la empresa no es válido."
                });
            }

            bool empresaExiste = await _context.EbEmpresas
                .AsNoTracking()
                .AnyAsync(x => x.Id == empresaId);

            if (!empresaExiste)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa solicitada."
                });
            }

            var accionistas = await _context.EbAccionistas
                .AsNoTracking()
                .Where(x => x.EmpresaId == empresaId)
                .OrderByDescending(x => x.PorcentajeParticipacion)
                .ThenBy(x => x.NombreCompleto)
                .Select(x => new
                {
                    id = x.Id,
                    empresaId = x.EmpresaId,
                    nombreCompleto = x.NombreCompleto,
                    rfc = x.Rfc,
                    porcentajeParticipacion = x.PorcentajeParticipacion,
                    nacionalidad = x.Nacionalidad,
                    esRepresentanteLegal = x.EsRepresentanteLegal,
                    deshabilitado = x.Deshabilitado,
                    fechaCreacion = x.FechaCreacion
                })
                .ToListAsync();

            decimal porcentajeTotal = accionistas.Sum(
                x => x.porcentajeParticipacion);

            return new JsonResult(new
            {
                success = true,
                data = accionistas,
                resumen = new
                {
                    totalAccionistas = accionistas.Count,
                    porcentajeTotal,
                    porcentajeDisponible = 100m - porcentajeTotal
                }
            });
        }

        // =====================================================
        // CONSULTAR ACCIONISTA
        // GET ?handler=Accionista&id=1
        // =====================================================
        public async Task<IActionResult> OnGetAccionistaAsync(int id)
        {
            if (id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador del accionista no es válido."
                });
            }

            var accionista = await _context.EbAccionistas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (accionista == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró el accionista solicitado."
                });
            }

            return new JsonResult(new
            {
                success = true,
                data = new
                {
                    id = accionista.Id,
                    empresaId = accionista.EmpresaId,
                    nombreCompleto = accionista.NombreCompleto,
                    rfc = accionista.Rfc,
                    porcentajeParticipacion =
                        accionista.PorcentajeParticipacion,
                    nacionalidad = accionista.Nacionalidad,
                    esRepresentanteLegal =
                        accionista.EsRepresentanteLegal,
                    deshabilitado = accionista.Deshabilitado
                }
            });
        }

        // =====================================================
        // CREAR ACCIONISTA
        // POST ?handler=CrearAccionista
        // =====================================================
        public async Task<IActionResult> OnPostCrearAccionistaAsync(
            [FromBody] AccionistaRequest request)
        {
            NormalizarAccionistaRequest(request);

            Dictionary<string, string[]> errores =
                await ValidarAccionistaRequestAsync(
                    request,
                    requiereId: false);

            if (errores.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Revisa la información del accionista.",
                    errors = errores
                });
            }

            bool empresaExiste = await _context.EbEmpresas
                .AnyAsync(x => x.Id == request.EmpresaId);

            if (!empresaExiste)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa seleccionada."
                });
            }

            string usuarioId = ObtenerUsuarioId();

            var accionista = new EbAccionista
            {
                EmpresaId = request.EmpresaId,
                NombreCompleto = request.NombreCompleto,
                Rfc = request.Rfc,
                PorcentajeParticipacion =
                    request.PorcentajeParticipacion,
                Nacionalidad = request.Nacionalidad,
                EsRepresentanteLegal =
                    request.EsRepresentanteLegal,
                Deshabilitado = false,
                Eliminado = false,
                FechaCreacion = DateTime.Now,
                UsuarioCreacionId = usuarioId
            };

            _context.EbAccionistas.Add(accionista);
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "El accionista se registró correctamente.",
                id = accionista.Id
            });
        }

        // =====================================================
        // EDITAR ACCIONISTA
        // POST ?handler=EditarAccionista
        // =====================================================
        public async Task<IActionResult> OnPostEditarAccionistaAsync(
            [FromBody] AccionistaRequest request)
        {
            NormalizarAccionistaRequest(request);

            Dictionary<string, string[]> errores =
                await ValidarAccionistaRequestAsync(
                    request,
                    requiereId: true);

            if (errores.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Revisa la información del accionista.",
                    errors = errores
                });
            }

            var accionista = await _context.EbAccionistas
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (accionista == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró el accionista que deseas editar."
                });
            }

            if (accionista.EmpresaId != request.EmpresaId)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El accionista no pertenece a la empresa seleccionada."
                });
            }

            accionista.NombreCompleto = request.NombreCompleto;
            accionista.Rfc = request.Rfc;
            accionista.PorcentajeParticipacion =
                request.PorcentajeParticipacion;
            accionista.Nacionalidad = request.Nacionalidad;
            accionista.EsRepresentanteLegal =
                request.EsRepresentanteLegal;
            accionista.FechaActualizacion = DateTime.Now;
            accionista.UsuarioActualizacionId =
                ObtenerUsuarioId();

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "El accionista se actualizó correctamente."
            });
        }

        // =====================================================
        // HABILITAR / DESHABILITAR ACCIONISTA
        // POST ?handler=CambiarEstatusAccionista
        // =====================================================
        public async Task<IActionResult>
            OnPostCambiarEstatusAccionistaAsync(
                [FromBody] AccionistaIdRequest request)
        {
            if (request.Id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador del accionista no es válido."
                });
            }

            var accionista = await _context.EbAccionistas
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (accionista == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró el accionista solicitado."
                });
            }

            accionista.Deshabilitado =
                !accionista.Deshabilitado;

            accionista.FechaActualizacion = DateTime.Now;
            accionista.UsuarioActualizacionId =
                ObtenerUsuarioId();

            await _context.SaveChangesAsync();

            string mensaje = accionista.Deshabilitado
                ? "El accionista se deshabilitó correctamente."
                : "El accionista se habilitó correctamente.";

            return new JsonResult(new
            {
                success = true,
                message = mensaje,
                deshabilitado = accionista.Deshabilitado
            });
        }

        // =====================================================
        // ELIMINAR ACCIONISTA LÓGICAMENTE
        // POST ?handler=EliminarAccionista
        // =====================================================
        public async Task<IActionResult> OnPostEliminarAccionistaAsync(
            [FromBody] AccionistaIdRequest request)
        {
            if (request.Id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador del accionista no es válido."
                });
            }

            var accionista = await _context.EbAccionistas
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (accionista == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró el accionista solicitado."
                });
            }

            accionista.Eliminado = true;
            accionista.Deshabilitado = true;
            accionista.FechaActualizacion = DateTime.Now;
            accionista.UsuarioActualizacionId =
                ObtenerUsuarioId();

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "El accionista se eliminó correctamente."
            });
        }

        private async Task<Dictionary<string, string[]>>
    ValidarAccionistaRequestAsync(
        AccionistaRequest request,
        bool requiereId)
        {
            var errores = new Dictionary<string, string[]>();

            if (requiereId && request.Id <= 0)
            {
                errores["Id"] = new[]
                {
            "El identificador del accionista no es válido."
        };
            }

            if (request.EmpresaId <= 0)
            {
                errores["EmpresaId"] = new[]
                {
            "La empresa es obligatoria."
        };
            }

            if (string.IsNullOrWhiteSpace(request.NombreCompleto))
            {
                errores["NombreCompleto"] = new[]
                {
            "El nombre completo es obligatorio."
        };
            }
            else if (request.NombreCompleto.Length > 250)
            {
                errores["NombreCompleto"] = new[]
                {
            "El nombre no puede exceder 250 caracteres."
        };
            }

            if (!string.IsNullOrWhiteSpace(request.Rfc))
            {
                var validadorRfc = new RegularExpressionAttribute(
                    @"^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$");

                if (request.Rfc.Length is < 12 or > 13 ||
                    !validadorRfc.IsValid(request.Rfc))
                {
                    errores["Rfc"] = new[]
                    {
                "El formato del RFC no es válido."
            };
                }
            }

            if (request.PorcentajeParticipacion <= 0)
            {
                errores["PorcentajeParticipacion"] = new[]
                {
            "El porcentaje debe ser mayor que cero."
        };
            }
            else if (request.PorcentajeParticipacion > 100)
            {
                errores["PorcentajeParticipacion"] = new[]
                {
            "El porcentaje no puede ser mayor que 100."
        };
            }

            if (!string.IsNullOrWhiteSpace(request.Nacionalidad) &&
                request.Nacionalidad.Length > 100)
            {
                errores["Nacionalidad"] = new[]
                {
            "La nacionalidad no puede exceder 100 caracteres."
        };
            }

            if (request.EmpresaId > 0 &&
                request.PorcentajeParticipacion > 0 &&
                request.PorcentajeParticipacion <= 100)
            {
                decimal porcentajeRegistrado =
                    await _context.EbAccionistas
                        .AsNoTracking()
                        .Where(x =>
                            x.EmpresaId == request.EmpresaId &&
                            x.Id != request.Id &&
                            !x.Deshabilitado)
                        .SumAsync(x =>
                            (decimal?)x.PorcentajeParticipacion)
                        ?? 0m;

                decimal porcentajeFinal =
                    porcentajeRegistrado +
                    request.PorcentajeParticipacion;

                if (porcentajeFinal > 100m)
                {
                    decimal porcentajeDisponible =
                        100m - porcentajeRegistrado;

                    errores["PorcentajeParticipacion"] = new[]
                    {
                $"La participación total no puede superar el 100 %. " +
                $"Actualmente hay {porcentajeRegistrado:N4} % registrado " +
                $"y quedan {porcentajeDisponible:N4} % disponibles."
            };
                }
            }

            return errores;
        }

        private static void NormalizarAccionistaRequest(AccionistaRequest request)
        {
            request.NombreCompleto =
                request.NombreCompleto?.Trim() ?? string.Empty;

            request.Rfc = NormalizarOpcional(request.Rfc)?
                .ToUpperInvariant();

            request.Nacionalidad =
                NormalizarOpcional(request.Nacionalidad);
        }

        private static void NormalizarRequest(EmpresaRequest request)
        {
            request.RazonSocial =
                request.RazonSocial?.Trim() ?? string.Empty;

            request.NombreCorto =
                request.NombreCorto?.Trim() ?? string.Empty;

            request.Rfc =
                request.Rfc?.Trim().ToUpperInvariant() ?? string.Empty;

            request.Nivel = NormalizarOpcional(request.Nivel);

            request.ActividadComercial =
                NormalizarOpcional(request.ActividadComercial);

            request.TelefonoBancos =
                NormalizarOpcional(request.TelefonoBancos);

            request.CorreoBancos =
                NormalizarOpcional(request.CorreoBancos);

            request.NumeroEscritura =
                NormalizarOpcional(request.NumeroEscritura);

            request.DomicilioFiscal =
                NormalizarOpcional(request.DomicilioFiscal);

            request.Observaciones =
                NormalizarOpcional(request.Observaciones);
        }

        private static string? NormalizarOpcional(string? valor)
        {
            string? resultado = valor?.Trim();

            return string.IsNullOrWhiteSpace(resultado)
                ? null
                : resultado;
        }

        private string ObtenerUsuarioId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? "SYSTEM";
        }

        // =====================================================
        // REQUESTS AJAX
        // =====================================================
        public class EmpresaRequest
        {
            public int Id { get; set; }

            public string RazonSocial { get; set; } = string.Empty;

            public string NombreCorto { get; set; } = string.Empty;

            public string Rfc { get; set; } = string.Empty;

            public string? Nivel { get; set; }

            public string? ActividadComercial { get; set; }

            public string? TelefonoBancos { get; set; }

            public string? CorreoBancos { get; set; }

            public DateTime? FechaConstitucion { get; set; }

            public string? NumeroEscritura { get; set; }

            public string? DomicilioFiscal { get; set; }

            public string? Observaciones { get; set; }
        }

        public class EmpresaIdRequest
        {
            public int Id { get; set; }
        }

        public class AccionistaRequest
        {
            public int Id { get; set; }

            public int EmpresaId { get; set; }

            public string NombreCompleto { get; set; }
                = string.Empty;

            public string? Rfc { get; set; }

            public decimal PorcentajeParticipacion { get; set; }

            public string? Nacionalidad { get; set; }

            public bool EsRepresentanteLegal { get; set; }
        }

        public class AccionistaIdRequest
        {
            public int Id { get; set; }
        }

        public class CargarDocumentoRequest
        {
            public int EmpresaId { get; set; }

            public int TipoDocumentoId { get; set; }

            public IFormFile? Archivo { get; set; }

            public DateTime? FechaVencimiento { get; set; }

            public string? Observaciones { get; set; }
        }
    }
}