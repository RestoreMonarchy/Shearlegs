CREATE TABLE dbo.ReportBranchParameters
(
	Id INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_ReportBranchParameters PRIMARY KEY,
	BranchId INT CONSTRAINT FK_ReportBranchParameters_BranchId FOREIGN KEY REFERENCES dbo.ReportBranches(Id),
	Name NVARCHAR(255) NOT NULL,
	InputType VARCHAR(255) NOT NULL,
	IsMandatory BIT NOT NULL
);
