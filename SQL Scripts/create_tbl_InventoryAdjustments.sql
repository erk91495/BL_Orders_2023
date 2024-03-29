/*
   Monday, March 11, 202411:17:39 AM
   User: 
   Server: Eric-Pc
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
ALTER TABLE dbo.tblProducts SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE dbo.tbl_InventoryAdjustments
	(
	ProductID int NOT NULL,
	ManualAdjustments int NOT NULL,
	LastAdjustment int NOT NUll,
	SortIndex smallint NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_InventoryAdjustments ADD CONSTRAINT
	PK_tbl_InventoryAdjustments PRIMARY KEY CLUSTERED 
	(
	ProductID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tbl_InventoryAdjustments ADD CONSTRAINT
	FK_tbl_InventoryAdjustments_tblProducts FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.tblProducts
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_InventoryAdjustments SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
