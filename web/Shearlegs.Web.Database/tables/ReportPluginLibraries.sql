﻿CREATE TABLE dbo.ReportPluginLibraries
(
	Id INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_ReportPluginLibraries PRIMARY KEY,
	PluginId INT NOT NULL CONSTRAINT FK_ReportPluginLibraries_PluginId FOREIGN KEY REFERENCES dbo.ReportPlugins(Id),
	Name NVARCHAR(255) NOT NULL,
	Content VARBINARY(MAX) NOT NULL
)
