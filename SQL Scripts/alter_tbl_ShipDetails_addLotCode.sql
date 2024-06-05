/*
   Wednesday, June 5, 202411:34:33 AM
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
ALTER TABLE dbo.tbl_LotCodes SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_ShipDetails ADD
	LotCode nvarchar(50) NULL
GO
ALTER TABLE dbo.tbl_ShipDetails SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

UPDATE dbo.tbl_ShipDetails 
	SET tbl_ShipDetails.LotCode = tbl_LiveInventory.LotCode 
	FROM tbl_ShipDetails INNER JOIN tbl_LiveInventory ON tbl_ShipDetails.LiveInventoryID = tbl_LiveInventory.ID
	GO