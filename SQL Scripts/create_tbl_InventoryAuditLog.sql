/*
   Monday, July 24, 20238:10:27 AM
   User: 
   Server: ERIC-PC
   Database: New_BL_Orders
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
CREATE TABLE dbo.tbl_InventoryAuditLog
	(
	ID int NOT NULL IDENTITY (1, 1),
	TransactionDate datetime NOT NULL,
	WorkstationName nvarchar(50) NULL,
	UserName nvarchar(50) NULL,
	ProductID int NOT NULL,
	StartingQuantity int NOT NULL,
	AdjustmentQuantity int NOT NULL,
	EndingQuantity int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_InventoryAuditLog SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
