BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729183246_CrearModuloExpedientesBancarios'
)
BEGIN
    CREATE TABLE [EB_Empresas] (
        [Id] int NOT NULL IDENTITY,
        [RazonSocial] nvarchar(250) NOT NULL,
        [NombreCorto] nvarchar(150) NOT NULL,
        [Rfc] nvarchar(13) NOT NULL,
        [Nivel] nvarchar(100) NULL,
        [ActividadComercial] nvarchar(500) NULL,
        [TelefonoBancos] nvarchar(30) NULL,
        [CorreoBancos] nvarchar(200) NULL,
        [FechaConstitucion] datetime2 NULL,
        [NumeroEscritura] nvarchar(200) NULL,
        [DomicilioFiscal] nvarchar(500) NULL,
        [Observaciones] nvarchar(1000) NULL,
        [Deshabilitado] bit NOT NULL DEFAULT CAST(0 AS bit),
        [Eliminado] bit NOT NULL DEFAULT CAST(0 AS bit),
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaActualizacion] datetime2 NULL,
        [UsuarioCreacionId] nvarchar(450) NOT NULL,
        [UsuarioActualizacionId] nvarchar(450) NULL,
        CONSTRAINT [PK_EB_Empresas] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729183246_CrearModuloExpedientesBancarios'
)
BEGIN
    CREATE TABLE [EB_TiposDocumento] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(200) NOT NULL,
        [Categoria] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [EsObligatorio] bit NOT NULL DEFAULT CAST(1 AS bit),
        [RequiereFechaVencimiento] bit NOT NULL DEFAULT CAST(0 AS bit),
        [PermiteMultiplesArchivos] bit NOT NULL DEFAULT CAST(0 AS bit),
        [Orden] int NOT NULL,
        [Deshabilitado] bit NOT NULL DEFAULT CAST(0 AS bit),
        [Eliminado] bit NOT NULL DEFAULT CAST(0 AS bit),
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaActualizacion] datetime2 NULL,
        [UsuarioCreacionId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_EB_TiposDocumento] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729183246_CrearModuloExpedientesBancarios'
)
BEGIN
    CREATE TABLE [EB_Accionistas] (
        [Id] int NOT NULL IDENTITY,
        [EmpresaId] int NOT NULL,
        [NombreCompleto] nvarchar(250) NOT NULL,
        [Rfc] nvarchar(13) NULL,
        [PorcentajeParticipacion] decimal(7,4) NOT NULL,
        [Nacionalidad] nvarchar(100) NULL,
        [EsRepresentanteLegal] bit NOT NULL,
        [Deshabilitado] bit NOT NULL DEFAULT CAST(0 AS bit),
        [Eliminado] bit NOT NULL DEFAULT CAST(0 AS bit),
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaActualizacion] datetime2 NULL,
        [UsuarioCreacionId] nvarchar(450) NOT NULL,
        [UsuarioActualizacionId] nvarchar(450) NULL,
        CONSTRAINT [PK_EB_Accionistas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EB_Accionistas_EB_Empresas_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [EB_Empresas] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729183246_CrearModuloExpedientesBancarios'
)
BEGIN
    CREATE TABLE [EB_Documentos] (
        [Id] int NOT NULL IDENTITY,
        [EmpresaId] int NOT NULL,
        [TipoDocumentoId] int NOT NULL,
        [NombreOriginal] nvarchar(300) NOT NULL,
        [NombreAlmacenado] nvarchar(300) NOT NULL,
        [RutaArchivo] nvarchar(500) NOT NULL,
        [Extension] nvarchar(20) NOT NULL,
        [MimeType] nvarchar(150) NOT NULL,
        [TamanoBytes] bigint NOT NULL,
        [Version] int NOT NULL DEFAULT 1,
        [FechaCarga] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaVencimiento] datetime2 NULL,
        [Estado] nvarchar(50) NOT NULL DEFAULT N'Vigente',
        [Observaciones] nvarchar(1000) NULL,
        [EsVersionActual] bit NOT NULL DEFAULT CAST(1 AS bit),
        [Eliminado] bit NOT NULL DEFAULT CAST(0 AS bit),
        [FechaEliminacion] datetime2 NULL,
        [UsuarioCargaId] nvarchar(450) NOT NULL,
        [UsuarioEliminacionId] nvarchar(450) NULL,
        CONSTRAINT [PK_EB_Documentos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EB_Documentos_EB_Empresas_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [EB_Empresas] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EB_Documentos_EB_TiposDocumento_TipoDocumentoId] FOREIGN KEY ([TipoDocumentoId]) REFERENCES [EB_TiposDocumento] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729183246_CrearModuloExpedientesBancarios'
)
BEGIN
    CREATE INDEX [IX_EB_Accionistas_EmpresaId] ON [EB_Accionistas] ([EmpresaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729183246_CrearModuloExpedientesBancarios'
)
BEGIN
    CREATE INDEX [IX_EB_Documentos_EmpresaId] ON [EB_Documentos] ([EmpresaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729183246_CrearModuloExpedientesBancarios'
)
BEGIN
    CREATE INDEX [IX_EB_Documentos_Expediente] ON [EB_Documentos] ([EmpresaId], [TipoDocumentoId], [EsVersionActual]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729183246_CrearModuloExpedientesBancarios'
)
BEGIN
    CREATE INDEX [IX_EB_Documentos_TipoDocumentoId] ON [EB_Documentos] ([TipoDocumentoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729183246_CrearModuloExpedientesBancarios'
)
BEGIN
    CREATE INDEX [IX_EB_Empresas_NombreCorto] ON [EB_Empresas] ([NombreCorto]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729183246_CrearModuloExpedientesBancarios'
)
BEGIN
    CREATE INDEX [IX_EB_Empresas_RazonSocial] ON [EB_Empresas] ([RazonSocial]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729183246_CrearModuloExpedientesBancarios'
)
BEGIN
    CREATE UNIQUE INDEX [UX_EB_Empresas_Rfc] ON [EB_Empresas] ([Rfc]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729183246_CrearModuloExpedientesBancarios'
)
BEGIN
    CREATE UNIQUE INDEX [UX_EB_TiposDocumento_Nombre_Categoria] ON [EB_TiposDocumento] ([Nombre], [Categoria]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729183246_CrearModuloExpedientesBancarios'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260729183246_CrearModuloExpedientesBancarios', N'8.0.7');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] ON;
    EXEC(N'INSERT INTO [EB_TiposDocumento] ([Id], [Categoria], [Descripcion], [EsObligatorio], [FechaActualizacion], [FechaCreacion], [Nombre], [Orden], [UsuarioCreacionId])
    VALUES (1, N''Fiscal'', N''Constancia de Situación Fiscal vigente de la empresa.'', CAST(1 AS bit), NULL, ''2026-07-29T00:00:00.0000000'', N''Constancia de Situación Fiscal'', 1, N''SYSTEM'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'RequiereFechaVencimiento', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] ON;
    EXEC(N'INSERT INTO [EB_TiposDocumento] ([Id], [Categoria], [Descripcion], [EsObligatorio], [FechaActualizacion], [FechaCreacion], [Nombre], [Orden], [RequiereFechaVencimiento], [UsuarioCreacionId])
    VALUES (2, N''Fiscal'', N''Certificado de firma electrónica vigente.'', CAST(1 AS bit), NULL, ''2026-07-29T00:00:00.0000000'', N''Certificado FIEL'', 2, CAST(1 AS bit), N''SYSTEM'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'RequiereFechaVencimiento', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'RequiereFechaVencimiento', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] ON;
    EXEC(N'INSERT INTO [EB_TiposDocumento] ([Id], [Categoria], [Descripcion], [EsObligatorio], [FechaActualizacion], [FechaCreacion], [Nombre], [Orden], [PermiteMultiplesArchivos], [RequiereFechaVencimiento], [UsuarioCreacionId])
    VALUES (3, N''Domicilio'', N''Comprobante de domicilio fiscal o comercial.'', CAST(1 AS bit), NULL, ''2026-07-29T00:00:00.0000000'', N''Comprobante de domicilio'', 3, CAST(1 AS bit), CAST(1 AS bit), N''SYSTEM'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'RequiereFechaVencimiento', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] ON;
    EXEC(N'INSERT INTO [EB_TiposDocumento] ([Id], [Categoria], [Descripcion], [EsObligatorio], [FechaActualizacion], [FechaCreacion], [Nombre], [Orden], [UsuarioCreacionId])
    VALUES (4, N''Corporativo'', N''Acta constitutiva de la sociedad.'', CAST(1 AS bit), NULL, ''2026-07-29T00:00:00.0000000'', N''Acta constitutiva'', 4, N''SYSTEM'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] ON;
    EXEC(N'INSERT INTO [EB_TiposDocumento] ([Id], [Categoria], [Descripcion], [FechaActualizacion], [FechaCreacion], [Nombre], [Orden], [PermiteMultiplesArchivos], [UsuarioCreacionId])
    VALUES (5, N''Corporativo'', N''Reformas, protocolizaciones o instrumentos adicionales.'', NULL, ''2026-07-29T00:00:00.0000000'', N''Actas o instrumentos adicionales'', 5, CAST(1 AS bit), N''SYSTEM''),
    (6, N''Legal'', N''Poderes notariales vigentes de representantes o apoderados.'', NULL, ''2026-07-29T00:00:00.0000000'', N''Poder notarial'', 6, CAST(1 AS bit), N''SYSTEM'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'RequiereFechaVencimiento', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] ON;
    EXEC(N'INSERT INTO [EB_TiposDocumento] ([Id], [Categoria], [Descripcion], [EsObligatorio], [FechaActualizacion], [FechaCreacion], [Nombre], [Orden], [PermiteMultiplesArchivos], [RequiereFechaVencimiento], [UsuarioCreacionId])
    VALUES (7, N''Accionistas'', N''Identificación oficial de los accionistas.'', CAST(1 AS bit), NULL, ''2026-07-29T00:00:00.0000000'', N''INE de accionistas'', 7, CAST(1 AS bit), CAST(1 AS bit), N''SYSTEM'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'RequiereFechaVencimiento', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] ON;
    EXEC(N'INSERT INTO [EB_TiposDocumento] ([Id], [Categoria], [Descripcion], [EsObligatorio], [FechaActualizacion], [FechaCreacion], [Nombre], [Orden], [PermiteMultiplesArchivos], [UsuarioCreacionId])
    VALUES (8, N''Accionistas'', N''Constancia de Situación Fiscal de cada accionista.'', CAST(1 AS bit), NULL, ''2026-07-29T00:00:00.0000000'', N''CSF de accionistas'', 8, CAST(1 AS bit), N''SYSTEM'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'RequiereFechaVencimiento', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] ON;
    EXEC(N'INSERT INTO [EB_TiposDocumento] ([Id], [Categoria], [Descripcion], [EsObligatorio], [FechaActualizacion], [FechaCreacion], [Nombre], [Orden], [PermiteMultiplesArchivos], [RequiereFechaVencimiento], [UsuarioCreacionId])
    VALUES (9, N''Accionistas'', N''Comprobante de domicilio de cada accionista.'', CAST(1 AS bit), NULL, ''2026-07-29T00:00:00.0000000'', N''Comprobante de domicilio de accionistas'', 9, CAST(1 AS bit), CAST(1 AS bit), N''SYSTEM'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'RequiereFechaVencimiento', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] ON;
    EXEC(N'INSERT INTO [EB_TiposDocumento] ([Id], [Categoria], [Descripcion], [FechaActualizacion], [FechaCreacion], [Nombre], [Orden], [UsuarioCreacionId])
    VALUES (10, N''Corporativo'', N''Hoja membretada vigente de la empresa.'', NULL, ''2026-07-29T00:00:00.0000000'', N''Hoja membretada'', 10, N''SYSTEM'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] ON;
    EXEC(N'INSERT INTO [EB_TiposDocumento] ([Id], [Categoria], [Descripcion], [EsObligatorio], [FechaActualizacion], [FechaCreacion], [Nombre], [Orden], [UsuarioCreacionId])
    VALUES (11, N''Corporativo'', N''Organigrama actualizado de la empresa.'', CAST(1 AS bit), NULL, ''2026-07-29T00:00:00.0000000'', N''Organigrama'', 11, N''SYSTEM'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] ON;
    EXEC(N'INSERT INTO [EB_TiposDocumento] ([Id], [Categoria], [Descripcion], [EsObligatorio], [FechaActualizacion], [FechaCreacion], [Nombre], [Orden], [PermiteMultiplesArchivos], [UsuarioCreacionId])
    VALUES (12, N''Financiero'', N''Última declaración anual o mensual disponible.'', CAST(1 AS bit), NULL, ''2026-07-29T00:00:00.0000000'', N''Declaración anual o mensual'', 12, CAST(1 AS bit), N''SYSTEM'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'RequiereFechaVencimiento', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] ON;
    EXEC(N'INSERT INTO [EB_TiposDocumento] ([Id], [Categoria], [Descripcion], [EsObligatorio], [FechaActualizacion], [FechaCreacion], [Nombre], [Orden], [RequiereFechaVencimiento], [UsuarioCreacionId])
    VALUES (13, N''Fiscal'', N''Constancia de opinión de cumplimiento emitida por el SAT.'', CAST(1 AS bit), NULL, ''2026-07-29T00:00:00.0000000'', N''Opinión de cumplimiento SAT'', 13, CAST(1 AS bit), N''SYSTEM'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'EsObligatorio', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'RequiereFechaVencimiento', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] ON;
    EXEC(N'INSERT INTO [EB_TiposDocumento] ([Id], [Categoria], [Descripcion], [FechaActualizacion], [FechaCreacion], [Nombre], [Orden], [PermiteMultiplesArchivos], [UsuarioCreacionId])
    VALUES (14, N''Evidencias'', N''Imágenes o evidencias solicitadas por instituciones bancarias.'', NULL, ''2026-07-29T00:00:00.0000000'', N''Prueba de vida'', 14, CAST(1 AS bit), N''SYSTEM''),
    (15, N''Otros'', N''Documentación adicional requerida por la institución.'', NULL, ''2026-07-29T00:00:00.0000000'', N''Otro documento'', 15, CAST(1 AS bit), N''SYSTEM'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Categoria', N'Descripcion', N'FechaActualizacion', N'FechaCreacion', N'Nombre', N'Orden', N'PermiteMultiplesArchivos', N'UsuarioCreacionId') AND [object_id] = OBJECT_ID(N'[EB_TiposDocumento]'))
        SET IDENTITY_INSERT [EB_TiposDocumento] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729184514_AgregarCatalogoInicialExpedientesBancarios'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260729184514_AgregarCatalogoInicialExpedientesBancarios', N'8.0.7');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731213951_CrearBitacoraDocumentalExpedientes'
)
BEGIN
    CREATE TABLE [EB_BitacoraDocumentos] (
        [Id] bigint NOT NULL IDENTITY,
        [EmpresaId] int NOT NULL,
        [DocumentoId] int NULL,
        [TipoDocumentoId] int NULL,
        [Accion] nvarchar(50) NOT NULL,
        [UsuarioId] nvarchar(450) NULL,
        [NombreUsuario] nvarchar(250) NULL,
        [NombreDocumento] nvarchar(250) NULL,
        [Banco] nvarchar(50) NULL,
        [FechaEvento] datetime2 NOT NULL,
        [DireccionIp] nvarchar(64) NULL,
        [Navegador] nvarchar(1000) NULL,
        [Exitoso] bit NOT NULL,
        [Detalle] nvarchar(1000) NULL,
        [VersionDocumento] int NULL,
        CONSTRAINT [PK_EB_BitacoraDocumentos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EB_BitacoraDocumentos_EB_Documentos_DocumentoId] FOREIGN KEY ([DocumentoId]) REFERENCES [EB_Documentos] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_EB_BitacoraDocumentos_EB_Empresas_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [EB_Empresas] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EB_BitacoraDocumentos_EB_TiposDocumento_TipoDocumentoId] FOREIGN KEY ([TipoDocumentoId]) REFERENCES [EB_TiposDocumento] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731213951_CrearBitacoraDocumentalExpedientes'
)
BEGIN
    CREATE INDEX [IX_EB_BitacoraDocumentos_Accion_Fecha] ON [EB_BitacoraDocumentos] ([Accion], [FechaEvento]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731213951_CrearBitacoraDocumentalExpedientes'
)
BEGIN
    CREATE INDEX [IX_EB_BitacoraDocumentos_Banco_Fecha] ON [EB_BitacoraDocumentos] ([Banco], [FechaEvento]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731213951_CrearBitacoraDocumentalExpedientes'
)
BEGIN
    CREATE INDEX [IX_EB_BitacoraDocumentos_DocumentoId] ON [EB_BitacoraDocumentos] ([DocumentoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731213951_CrearBitacoraDocumentalExpedientes'
)
BEGIN
    CREATE INDEX [IX_EB_BitacoraDocumentos_Empresa_Fecha] ON [EB_BitacoraDocumentos] ([EmpresaId], [FechaEvento]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731213951_CrearBitacoraDocumentalExpedientes'
)
BEGIN
    CREATE INDEX [IX_EB_BitacoraDocumentos_FechaEvento] ON [EB_BitacoraDocumentos] ([FechaEvento]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731213951_CrearBitacoraDocumentalExpedientes'
)
BEGIN
    CREATE INDEX [IX_EB_BitacoraDocumentos_TipoDocumentoId] ON [EB_BitacoraDocumentos] ([TipoDocumentoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731213951_CrearBitacoraDocumentalExpedientes'
)
BEGIN
    CREATE INDEX [IX_EB_BitacoraDocumentos_Usuario_Fecha] ON [EB_BitacoraDocumentos] ([UsuarioId], [FechaEvento]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731213951_CrearBitacoraDocumentalExpedientes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260731213951_CrearBitacoraDocumentalExpedientes', N'8.0.7');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260804183525_CrearPermisosComplianceUsuarios'
)
BEGIN
    CREATE TABLE [EB_PermisosComplianceUsuarios] (
        [Id] int NOT NULL IDENTITY,
        [UsuarioId] nvarchar(450) NOT NULL,
        [PuedeVisualizar] bit NOT NULL DEFAULT CAST(0 AS bit),
        [PuedeCrearCargar] bit NOT NULL DEFAULT CAST(0 AS bit),
        [PuedeModificar] bit NOT NULL DEFAULT CAST(0 AS bit),
        [PuedeEliminar] bit NOT NULL DEFAULT CAST(0 AS bit),
        [PuedeDescargar] bit NOT NULL DEFAULT CAST(0 AS bit),
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaModificacion] datetime2 NULL,
        [UsuarioModificacionId] nvarchar(450) NULL,
        CONSTRAINT [PK_EB_PermisosComplianceUsuarios] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260804183525_CrearPermisosComplianceUsuarios'
)
BEGIN
    CREATE INDEX [IX_EB_PermisosComplianceUsuarios_FechaModificacion] ON [EB_PermisosComplianceUsuarios] ([FechaModificacion]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260804183525_CrearPermisosComplianceUsuarios'
)
BEGIN
    CREATE UNIQUE INDEX [UX_EB_PermisosComplianceUsuarios_UsuarioId] ON [EB_PermisosComplianceUsuarios] ([UsuarioId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260804183525_CrearPermisosComplianceUsuarios'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260804183525_CrearPermisosComplianceUsuarios', N'8.0.7');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805182607_AgregarBitacoraActividadEmpresas'
)
BEGIN
    CREATE TABLE [EB_BitacoraEmpresas] (
        [Id] bigint NOT NULL IDENTITY,
        [EmpresaId] int NOT NULL,
        [Accion] nvarchar(80) NOT NULL,
        [UsuarioId] nvarchar(450) NOT NULL,
        [NombreUsuario] nvarchar(250) NOT NULL,
        [FechaEvento] datetime2 NOT NULL DEFAULT (GETDATE()),
        [DireccionIp] nvarchar(64) NULL,
        [Navegador] nvarchar(1000) NULL,
        [Exitoso] bit NOT NULL DEFAULT CAST(1 AS bit),
        [Detalle] nvarchar(2000) NULL,
        CONSTRAINT [PK_EB_BitacoraEmpresas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EB_BitacoraEmpresas_EB_Empresas_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [EB_Empresas] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805182607_AgregarBitacoraActividadEmpresas'
)
BEGIN
    CREATE INDEX [IX_EB_BitacoraEmpresas_Accion_FechaEvento] ON [EB_BitacoraEmpresas] ([Accion], [FechaEvento]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805182607_AgregarBitacoraActividadEmpresas'
)
BEGIN
    CREATE INDEX [IX_EB_BitacoraEmpresas_EmpresaId] ON [EB_BitacoraEmpresas] ([EmpresaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805182607_AgregarBitacoraActividadEmpresas'
)
BEGIN
    CREATE INDEX [IX_EB_BitacoraEmpresas_FechaEvento] ON [EB_BitacoraEmpresas] ([FechaEvento]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805182607_AgregarBitacoraActividadEmpresas'
)
BEGIN
    CREATE INDEX [IX_EB_BitacoraEmpresas_UsuarioId] ON [EB_BitacoraEmpresas] ([UsuarioId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260805182607_AgregarBitacoraActividadEmpresas'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260805182607_AgregarBitacoraActividadEmpresas', N'8.0.7');
END;
GO

COMMIT;
GO

