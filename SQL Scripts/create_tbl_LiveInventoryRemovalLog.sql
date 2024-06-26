/*
   Friday, March 29, 20248:36:42 AM
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
ALTER TABLE dbo.tbl_LiveInventory SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.tbl_LiveInventoryRemovalLog
	(
	ID uniqueidentifier NOT NULL ROWGUIDCOL,
	Scanline nvarchar(70) NOT NULL,
	RemovalReasonID uniqueidentifier NOT NULL,
	LiveInventoryID int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_LiveInventoryRemovalLog ADD CONSTRAINT
	DF_tbl_LiveInventoryRemovalLog_ID DEFAULT (newid()) FOR ID
GO
ALTER TABLE dbo.tbl_LiveInventoryRemovalLog ADD CONSTRAINT
	PK_tbl_LiveInventoryRemovalLog PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tbl_LiveInventoryRemovalLog ADD CONSTRAINT
	FK_tbl_LiveInventoryRemovalLog_tbl_LiveInventory FOREIGN KEY
	(
	LiveInventoryID
	) REFERENCES dbo.tbl_LiveInventory
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.tbl_LiveInventoryRemovalLog ADD CONSTRAINT
	FK_tbl_LiveInventoryRemovalLog_tbl_LiveInventoryRemovalReason FOREIGN KEY
	(
	RemovalReasonID
	) REFERENCES dbo.tbl_LiveInventoryRemovalReason
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_LiveInventoryRemovalLog SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
