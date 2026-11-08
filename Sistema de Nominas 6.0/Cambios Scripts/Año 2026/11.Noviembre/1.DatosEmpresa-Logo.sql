-- Logo de la empresa guardado en la base de datos (DatosEmpresa.Logo, texto base64). Reemplaza RutaLogo.
IF COL_LENGTH('dbo.DatosEmpresa', 'Logo') IS NULL
    ALTER TABLE dbo.DatosEmpresa ADD Logo nvarchar(max) NULL;
GO
