/*
   Monday, June 26, 20232:08:27 PM
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
ALTER TABLE dbo.tblProducts
	DROP CONSTRAINT DF__tblProduc__Price__118A8A8C
GO
ALTER TABLE dbo.tblProducts
	DROP CONSTRAINT DF__tblProduc__Price__1372D2FE
GO
ALTER TABLE dbo.tblProducts
	DROP CONSTRAINT DF__tblProduc__Price__1466F737
GO
ALTER TABLE dbo.tblProducts
	DROP CONSTRAINT DF__tblProduc__Price__155B1B70
GO
ALTER TABLE dbo.tblProducts
	DROP CONSTRAINT DF__tblProduc__Price__164F3FA9
GO
ALTER TABLE dbo.tblProducts
	DROP CONSTRAINT DF__tblProduc__Price__174363E2
GO
ALTER TABLE dbo.tblProducts
	DROP COLUMN [Price_(Co-op)], [Price_(Distributor)], [Price_(Retail)], [Price_(DutchCreek)], [Price_(Other)], [Price_(PerCan)]
GO
ALTER TABLE dbo.tblProducts SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
