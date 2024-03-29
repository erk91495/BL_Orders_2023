/*
   Monday, March 18, 202411:29:34 AM
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
ALTER TABLE dbo.tbl_LiveInventory ADD
	RemovedFromInventory bit NOT NULL CONSTRAINT DF_tbl_LiveInventory_RemovedFromInventory DEFAULT 0
GO
CREATE NONCLUSTERED INDEX IX_tbl_LiveInventory ON dbo.tbl_LiveInventory
	(
	RemovedFromInventory DESC
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_LiveInventory SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
