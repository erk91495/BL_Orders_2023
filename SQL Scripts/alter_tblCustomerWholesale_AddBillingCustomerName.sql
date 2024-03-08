/*
   Monday, May 1, 20239:54:36 AM
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
ALTER TABLE dbo.tblCustomerWholesale ADD
	BillingCustomerName nvarchar(30) NULL
GO
ALTER TABLE dbo.tblCustomerWholesale SET (LOCK_ESCALATION = TABLE)
GO
UPDATE [dbo].[tblCustomerWholesale]
   SET 
      [BillingCustomerName] = CustomerName
GO
COMMIT
