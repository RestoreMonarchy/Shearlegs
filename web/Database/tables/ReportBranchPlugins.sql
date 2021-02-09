﻿CREATE TABLE dbo.ReportBranchPlugins
(
	Id INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_ReportBranchPlugins PRIMARY KEY,
	BranchId INT NOT NULL CONSTRAINT FK_ReportBranchPlugins_ReportId FOREIGN KEY REFERENCES dbo.ReportBranches(Id),
	Version VARCHAR(255) NOT NULL,
	Changelog NVARCHAR(MAX) NULL,
	Content VARBINARY(MAX) NOT NULL,
	TemplateContent VARBINARY(MAX) NULL,
	TemplateMimeType VARCHAR(255) NULL,
	TemplateFileName NVARCHAR(255) NULL,
	CreateDate DATETIME2(0) NOT NULL CONSTRAINT DF_ReportBranchPlugins_CreateDate DEFAULT SYSDATETIME()
);
