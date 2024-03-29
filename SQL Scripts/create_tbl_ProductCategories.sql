USE BL_Enterprise
/*
   Tuesday, February 6, 20244:23:04 PM
   User: 
   Server: ERIC-PC
   Database: BL_Enterprise
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.tbl_ProductCategories
	(
	CategoryID int NOT NULL IDENTITY (1, 1),
	CategoryName nvarchar(30) NOT NULL,
	ShowTotalsOnReports bit NOT NULL,
	DisplayIndex smallint NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_ProductCategories ADD CONSTRAINT
	PK_tbl_ProductCategories PRIMARY KEY CLUSTERED 
	(
	CategoryID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tbl_ProductCategories SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
