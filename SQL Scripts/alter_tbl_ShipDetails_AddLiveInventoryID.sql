/*
   Monday, March 18, 202411:19:18 AM
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
ALTER TABLE dbo.tbl_ShipDetails ADD
	LiveInventoryID int NULL
GO
CREATE NONCLUSTERED COLUMNSTORE INDEX IX_tbl_ShipDetails ON dbo.tbl_ShipDetails
	(
	SD_ID
	) ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_ShipDetails ADD CONSTRAINT
	FK_tbl_ShipDetails_tbl_LiveInventory FOREIGN KEY
	(
	LiveInventoryID
	) REFERENCES dbo.tbl_LiveInventory
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_ShipDetails SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
