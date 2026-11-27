BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261126222419_HU6_AdquisicionesAprobadoresPorArea'
)
BEGIN
    ALTER TABLE [ADQ_ConfiguracionAprobacionPresupuestal] ADD [AreaId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261126222419_HU6_AdquisicionesAprobadoresPorArea'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_ADQ_Configuracion_Area_Orden] ON [ADQ_ConfiguracionAprobacionPresupuestal] ([AreaId], [Orden]) WHERE [AreaId] IS NOT NULL AND [Eliminado] = 0');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261126222419_HU6_AdquisicionesAprobadoresPorArea'
)
BEGIN
    ALTER TABLE [ADQ_ConfiguracionAprobacionPresupuestal] ADD CONSTRAINT [FK_ADQ_ConfiguracionAprobacionPresupuestal_Areas_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [Areas] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261126222419_HU6_AdquisicionesAprobadoresPorArea'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261126222419_HU6_AdquisicionesAprobadoresPorArea', N'8.0.7');
END;
GO

COMMIT;
GO

