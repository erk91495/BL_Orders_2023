USE BL_Enterprise
/*
   Tuesday, February 6, 20244:26:18 PM
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
ALTER TABLE dbo.tbl_ProductCategories SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblProducts ADD
	CategoryID int NULL
GO
ALTER TABLE dbo.tblProducts ADD CONSTRAINT
	FK_tblProducts_tbl_ProductCategories FOREIGN KEY
	(
	CategoryID
	) REFERENCES dbo.tbl_ProductCategories
	(
	CategoryID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tblProducts SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
