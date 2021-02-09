CREATE TABLE dbo.ReportBranchPluginLibraries
(
	Id INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_ReportBranchPluginLibraries PRIMARY KEY,
	PluginId INT NOT NULL CONSTRAINT FK_ReportBranchPluginLibraries_PluginId FOREIGN KEY REFERENCES dbo.ReportBranchPlugins(Id),
	Name NVARCHAR(255) NOT NULL,
	Content VARBINARY(MAX) NOT NULL
)
