BEGIN TRANSACTION;
GO

ALTER TABLE [Empresas] ADD [PaginaInternet] nvarchar(500) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261109221938_AgregarPaginaInternetEmpresas', N'8.0.7');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [ConfiguracionesNotificacionesCorreosDominios] (
    [Id] int NOT NULL IDENTITY,
    [Habilitado] bit NOT NULL DEFAULT CAST(1 AS bit),
    [DiasAnticipacion] int NOT NULL DEFAULT 10,
    [DiasEscalamiento] int NOT NULL DEFAULT 2,
    [RecordatoriosDiarios] bit NOT NULL DEFAULT CAST(1 AS bit),
    [NotificarVencidos] bit NOT NULL DEFAULT CAST(1 AS bit),
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificador] nvarchar(450) NULL,
    CONSTRAINT [PK_ConfiguracionesNotificacionesCorreosDominios] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [DestinatariosNotificacionesCorreosDominios] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NULL,
    [CorreoElectronico] nvarchar(256) NULL,
    [Nombre] nvarchar(150) NULL,
    [Activo] bit NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UsuarioCreador] nvarchar(450) NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificador] nvarchar(450) NULL,
    CONSTRAINT [PK_DestinatariosNotificacionesCorreosDominios] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DestinatariosNotificacionesCorreosDominios_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [HistorialNotificacionesCorreosDominios] (
    [Id] bigint NOT NULL IDENTITY,
    [CorreoDominioId] int NOT NULL,
    [FechaCaducacion] date NOT NULL,
    [FechaAviso] date NOT NULL,
    [DestinatarioClave] nvarchar(256) NOT NULL,
    [CorreoElectronico] nvarchar(256) NULL,
    [TipoAviso] nvarchar(30) NOT NULL,
    [Canal] nvarchar(30) NOT NULL,
    [Estado] nvarchar(30) NOT NULL,
    [Intentos] int NOT NULL,
    [FechaEnvio] datetime2 NULL,
    [Error] nvarchar(2000) NULL,
    CONSTRAINT [PK_HistorialNotificacionesCorreosDominios] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HistorialNotificacionesCorreosDominios_CorreosDominios_CorreoDominioId] FOREIGN KEY ([CorreoDominioId]) REFERENCES [CorreosDominios] ([Id]) ON DELETE NO ACTION
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DiasAnticipacion', N'DiasEscalamiento', N'FechaModificacion', N'Habilitado', N'NotificarVencidos', N'RecordatoriosDiarios', N'UsuarioModificador') AND [object_id] = OBJECT_ID(N'[ConfiguracionesNotificacionesCorreosDominios]'))
    SET IDENTITY_INSERT [ConfiguracionesNotificacionesCorreosDominios] ON;
INSERT INTO [ConfiguracionesNotificacionesCorreosDominios] ([Id], [DiasAnticipacion], [DiasEscalamiento], [FechaModificacion], [Habilitado], [NotificarVencidos], [RecordatoriosDiarios], [UsuarioModificador])
VALUES (1, 10, 2, NULL, CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DiasAnticipacion', N'DiasEscalamiento', N'FechaModificacion', N'Habilitado', N'NotificarVencidos', N'RecordatoriosDiarios', N'UsuarioModificador') AND [object_id] = OBJECT_ID(N'[ConfiguracionesNotificacionesCorreosDominios]'))
    SET IDENTITY_INSERT [ConfiguracionesNotificacionesCorreosDominios] OFF;
GO

CREATE INDEX [IX_DestinatariosNotificacionesCorreosDominios_CorreoElectronico] ON [DestinatariosNotificacionesCorreosDominios] ([CorreoElectronico]);
GO

CREATE INDEX [IX_DestinatariosNotificacionesCorreosDominios_UserId] ON [DestinatariosNotificacionesCorreosDominios] ([UserId]);
GO

CREATE UNIQUE INDEX [UX_CorreosDominios_Historial_NotificacionDiaria] ON [HistorialNotificacionesCorreosDominios] ([CorreoDominioId], [FechaCaducacion], [FechaAviso], [DestinatarioClave], [Canal]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261110195858_CorreosDominios_NotificacionesConfigurables', N'8.0.7');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [DestinatariosNotificacionesCorreosDominios] ADD [TipoDestinatario] nvarchar(30) NOT NULL DEFAULT N'Escalamiento';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261110211159_CorreosDominios_TiposDestinatarios', N'8.0.7');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [TimeIntegracion].[DepartamentoActividades] (
    [Id] bigint NOT NULL,
    [Nombre] nvarchar(191) NULL,
    [DepartamentoId] bigint NULL,
    [Descripcion] nvarchar(2000) NULL,
    [Estatus] tinyint NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_DepartamentoActividades] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_DepartamentoActividades_DepartamentoId] ON [TimeIntegracion].[DepartamentoActividades] ([DepartamentoId]);
GO

CREATE INDEX [IX_Actividades_DepartamentoActividadId] ON [TimeIntegracion].[Actividades] ([DepartamentoActividadId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261112191350_AddTimeDepartamentoActividades', N'8.0.7');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [BioTimeChecadas] (
    [Id] bigint NOT NULL,
    [EmpCode] nvarchar(40) NOT NULL,
    [FechaHora] datetime2 NOT NULL,
    [PunchState] nvarchar(20) NULL,
    [VerifyType] int NULL,
    [TerminalSn] nvarchar(100) NULL,
    [SincronizadoUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_BioTimeChecadas] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [BioTimeEmpleados] (
    [EmpCode] nvarchar(40) NOT NULL,
    [Nombre] nvarchar(200) NULL,
    [Apellidos] nvarchar(200) NULL,
    [Departamento] nvarchar(250) NULL,
    [Puesto] nvarchar(250) NULL,
    [ActualizadoUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_BioTimeEmpleados] PRIMARY KEY ([EmpCode])
);
GO

CREATE TABLE [BioTimeSincronizaciones] (
    [Id] bigint NOT NULL IDENTITY,
    [InicioUtc] datetime2 NOT NULL,
    [FinUtc] datetime2 NULL,
    [FechaInicioConsulta] datetime2 NOT NULL,
    [FechaFinConsulta] datetime2 NOT NULL,
    [Estado] nvarchar(30) NOT NULL,
    [EmpleadosProcesados] int NOT NULL,
    [ChecadasInsertadas] int NOT NULL,
    [Error] nvarchar(250) NULL,
    CONSTRAINT [PK_BioTimeSincronizaciones] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_BioTimeChecadas_EmpCode] ON [BioTimeChecadas] ([EmpCode]);
GO

CREATE INDEX [IX_BioTimeChecadas_FechaHora_EmpCode] ON [BioTimeChecadas] ([FechaHora], [EmpCode]);
GO

CREATE INDEX [IX_BioTimeSincronizaciones_InicioUtc] ON [BioTimeSincronizaciones] ([InicioUtc]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261119000901_AddBioTimeIntegration', N'8.0.7');
GO

COMMIT;
GO

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

