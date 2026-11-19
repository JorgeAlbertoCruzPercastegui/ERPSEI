BEGIN TRANSACTION;
GO

CREATE TABLE [TH_FolioConsecutivos] (
    [Id] int NOT NULL IDENTITY,
    [Tipo] int NOT NULL,
    [Anio] int NOT NULL,
    [UltimoConsecutivo] int NOT NULL,
    [FechaActualizacionUtc] datetime2 NOT NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_TH_FolioConsecutivos] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_TH_Folios_Anio] CHECK ([Anio] >= 2000),
    CONSTRAINT [CK_TH_Folios_Consecutivo] CHECK ([UltimoConsecutivo] >= 0)
);
GO

CREATE TABLE [TH_Solicitudes] (
    [Id] bigint NOT NULL IDENTITY,
    [Folio] nvarchar(40) NOT NULL,
    [Tipo] int NOT NULL,
    [Estatus] int NOT NULL,
    [EmpleadoId] int NOT NULL,
    [SolicitanteUserId] nvarchar(450) NOT NULL,
    [NumeroEmpleado] nvarchar(40) NOT NULL,
    [NombreSolicitante] nvarchar(250) NOT NULL,
    [AreaSolicitante] nvarchar(200) NULL,
    [OficinaSolicitante] nvarchar(200) NULL,
    [AtendidoPorUserId] nvarchar(450) NULL,
    [FechaEnvioUtc] datetime2 NOT NULL,
    [FechaInicioAtencionUtc] datetime2 NULL,
    [FechaRespuestaUtc] datetime2 NULL,
    [FechaCierreUtc] datetime2 NULL,
    [FechaActualizacionUtc] datetime2 NULL,
    [DiasObjetivoSla] int NULL,
    [FechaLimiteSlaUtc] datetime2 NULL,
    [RespondidaAutomaticamente] bit NOT NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_TH_Solicitudes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TH_Solicitudes_AspNetUsers_AtendidoPorUserId] FOREIGN KEY ([AtendidoPorUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TH_Solicitudes_AspNetUsers_SolicitanteUserId] FOREIGN KEY ([SolicitanteUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TH_Solicitudes_Empleados_EmpleadoId] FOREIGN KEY ([EmpleadoId]) REFERENCES [Empleados] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TH_HistorialSolicitudes] (
    [Id] bigint NOT NULL IDENTITY,
    [SolicitudId] bigint NOT NULL,
    [UsuarioId] nvarchar(450) NULL,
    [RolEjecutor] nvarchar(100) NOT NULL,
    [Accion] nvarchar(100) NOT NULL,
    [EstatusAnterior] int NULL,
    [EstatusNuevo] int NULL,
    [Comentario] nvarchar(2000) NULL,
    [FechaHoraUtc] datetime2 NOT NULL,
    [EjecutadoPorSistema] bit NOT NULL,
    [IpOrigen] nvarchar(64) NULL,
    CONSTRAINT [PK_TH_HistorialSolicitudes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TH_HistorialSolicitudes_AspNetUsers_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TH_HistorialSolicitudes_TH_Solicitudes_SolicitudId] FOREIGN KEY ([SolicitudId]) REFERENCES [TH_Solicitudes] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TH_Respuestas] (
    [Id] bigint NOT NULL IDENTITY,
    [SolicitudId] bigint NOT NULL,
    [TipoRespuesta] nvarchar(40) NOT NULL,
    [Contenido] nvarchar(4000) NULL,
    [UsuarioRespuestaId] nvarchar(450) NULL,
    [EsAutomatica] bit NOT NULL,
    [EsRespuestaFinal] bit NOT NULL,
    [FechaRespuestaUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_TH_Respuestas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TH_Respuestas_AspNetUsers_UsuarioRespuestaId] FOREIGN KEY ([UsuarioRespuestaId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TH_Respuestas_TH_Solicitudes_SolicitudId] FOREIGN KEY ([SolicitudId]) REFERENCES [TH_Solicitudes] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TH_Documentos] (
    [Id] bigint NOT NULL IDENTITY,
    [SolicitudId] bigint NOT NULL,
    [RespuestaId] bigint NULL,
    [NombreOriginal] nvarchar(260) NOT NULL,
    [NombreAlmacenado] nvarchar(100) NULL,
    [TipoContenido] nvarchar(150) NULL,
    [TamanoBytes] bigint NULL,
    [Origen] nvarchar(30) NOT NULL,
    [DriveId] nvarchar(300) NULL,
    [DriveItemId] nvarchar(500) NULL,
    [RutaAlmacenamiento] nvarchar(1000) NULL,
    [HashSha256] nvarchar(64) NULL,
    [Anio] int NULL,
    [Periodo] nvarchar(50) NULL,
    [Formato] nvarchar(15) NULL,
    [UsuarioCargaId] nvarchar(450) NULL,
    [FechaRegistroUtc] datetime2 NOT NULL,
    [EntregadoAlSolicitante] bit NOT NULL,
    [FechaEntregaUtc] datetime2 NULL,
    CONSTRAINT [PK_TH_Documentos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TH_Documentos_AspNetUsers_UsuarioCargaId] FOREIGN KEY ([UsuarioCargaId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TH_Documentos_TH_Respuestas_RespuestaId] FOREIGN KEY ([RespuestaId]) REFERENCES [TH_Respuestas] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TH_Documentos_TH_Solicitudes_SolicitudId] FOREIGN KEY ([SolicitudId]) REFERENCES [TH_Solicitudes] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_TH_Documentos_DriveId_DriveItemId] ON [TH_Documentos] ([DriveId], [DriveItemId]);
GO

CREATE INDEX [IX_TH_Documentos_RespuestaId] ON [TH_Documentos] ([RespuestaId]);
GO

CREATE INDEX [IX_TH_Documentos_SolicitudId_FechaRegistroUtc] ON [TH_Documentos] ([SolicitudId], [FechaRegistroUtc]);
GO

CREATE INDEX [IX_TH_Documentos_UsuarioCargaId] ON [TH_Documentos] ([UsuarioCargaId]);
GO

CREATE UNIQUE INDEX [IX_TH_FolioConsecutivos_Tipo_Anio] ON [TH_FolioConsecutivos] ([Tipo], [Anio]);
GO

CREATE INDEX [IX_TH_HistorialSolicitudes_SolicitudId_FechaHoraUtc] ON [TH_HistorialSolicitudes] ([SolicitudId], [FechaHoraUtc]);
GO

CREATE INDEX [IX_TH_HistorialSolicitudes_UsuarioId] ON [TH_HistorialSolicitudes] ([UsuarioId]);
GO

CREATE INDEX [IX_TH_Respuestas_SolicitudId_FechaRespuestaUtc] ON [TH_Respuestas] ([SolicitudId], [FechaRespuestaUtc]);
GO

CREATE INDEX [IX_TH_Respuestas_UsuarioRespuestaId] ON [TH_Respuestas] ([UsuarioRespuestaId]);
GO

CREATE INDEX [IX_TH_Solicitudes_AtendidoPorUserId] ON [TH_Solicitudes] ([AtendidoPorUserId]);
GO

CREATE INDEX [IX_TH_Solicitudes_EmpleadoId_FechaEnvioUtc] ON [TH_Solicitudes] ([EmpleadoId], [FechaEnvioUtc]);
GO

CREATE UNIQUE INDEX [IX_TH_Solicitudes_Folio] ON [TH_Solicitudes] ([Folio]);
GO

CREATE INDEX [IX_TH_Solicitudes_SolicitanteUserId] ON [TH_Solicitudes] ([SolicitanteUserId]);
GO

CREATE INDEX [IX_TH_Solicitudes_Tipo_Estatus_FechaEnvioUtc] ON [TH_Solicitudes] ([Tipo], [Estatus], [FechaEnvioUtc]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261119202350_AddPortalSolicitudesTalentoHumano', N'8.0.7');
GO

COMMIT;
GO

