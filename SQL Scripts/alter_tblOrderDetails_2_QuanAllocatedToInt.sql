/*
   Friday, July 14, 20231:36:31 PM
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

UPDATE tblOrderDetails_2 SET QuanAllocated = 0 WHERE QuanAllocated is NULL
GO
ALTER TABLE dbo.tblOrderDetails_2 ALTER COLUMN
	QuanAllocated int NOT NULL
ALTER TABLE [dbo].tblOrderDetails_2 ADD  DEFAULT ((0)) FOR QuanAllocated
GO
COMMIT
