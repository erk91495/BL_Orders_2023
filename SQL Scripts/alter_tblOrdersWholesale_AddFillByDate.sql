/*
   Friday, July 14, 20231:44:58 PM
   User: 
   Server: BL4
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
ALTER TABLE dbo.tblOrdersWholesale ADD
	 FillByDate DateTime NOT NULL CONSTRAINT DF_MyTable_MyColumn  DEFAULT(GETDATE())
GO
UPDATE dbo.tblOrdersWholesale SET FillByDate = PickupDate
GO
ALTER TABLE [dbo].[tblOrdersWholesale] DROP CONSTRAINT DF_MyTable_MyColumn
GO
ALTER TABLE dbo.tblOrdersWholesale SET (LOCK_ESCALATION = TABLE)
GO

COMMIT
