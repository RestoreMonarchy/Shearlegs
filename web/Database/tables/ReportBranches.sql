CREATE TABLE dbo.ReportBranches
(
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	ReportId INT NOT NULL CONSTRAINT FK_ReportBranches_ReportId REFERENCES Reports(Id),
	Name VARCHAR(50) NOT NULL,
	Description VARCHAR(255) NOT NULL,
	PluginId INT NULL CONSTRAINT FK_ReportBranches_PluginId FOREIGN KEY REFERENCES dbo.ReportBranchPlugins(Id)
)
