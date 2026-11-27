BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261126205416_HU6_AdquisicionesRolesAreas'
)
BEGIN
    CREATE TABLE [ADQ_RolesAreas] (
        [Id] int NOT NULL IDENTITY,
        [RolId] nvarchar(450) NOT NULL,
        [AreaId] int NOT NULL,
        [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [FechaCreacion] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        [FechaModificacion] datetime2 NULL,
        [UsuarioModificacionId] nvarchar(450) NULL,
        [Eliminado] bit NOT NULL DEFAULT CAST(0 AS bit),
        CONSTRAINT [PK_ADQ_RolesAreas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ADQ_RolesAreas_Areas_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [Areas] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ADQ_RolesAreas_AspNetRoles_RolId] FOREIGN KEY ([RolId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261126205416_HU6_AdquisicionesRolesAreas'
)
BEGIN
    CREATE INDEX [IX_ADQ_RolesAreas_Alcance] ON [ADQ_RolesAreas] ([AreaId], [Activo], [Eliminado]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261126205416_HU6_AdquisicionesRolesAreas'
)
BEGIN
    CREATE UNIQUE INDEX [UX_ADQ_RolesAreas_Rol_Area] ON [ADQ_RolesAreas] ([RolId], [AreaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261126205416_HU6_AdquisicionesRolesAreas'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261126205416_HU6_AdquisicionesRolesAreas', N'8.0.7');
END;
GO

COMMIT;
GO

