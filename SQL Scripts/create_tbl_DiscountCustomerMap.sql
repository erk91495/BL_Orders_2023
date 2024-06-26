/*
   Tuesday, April 9, 20249:49:28 AM
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
CREATE TABLE dbo.tbl_DiscountCustomerMap
	(
	ID uniqueidentifier NOT NULL ROWGUIDCOL,
	CustomerID int NOT NULL,
	DiscountID uniqueidentifier NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_DiscountCustomerMap ADD CONSTRAINT
	DF_tbl_DiscountCustomerMap_ID DEFAULT (newid()) FOR ID
GO
ALTER TABLE dbo.tbl_DiscountCustomerMap ADD CONSTRAINT
	PK_tbl_DiscountCustomerMap PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tbl_DiscountCustomerMap ADD CONSTRAINT
	FK_tbl_DiscountCustomerMap_tblCustomerWholesale FOREIGN KEY
	(
	CustomerID
	) REFERENCES dbo.tblCustomerWholesale
	(
	CustID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_DiscountCustomerMap ADD CONSTRAINT
	FK_tbl_DiscountCustomerMap_tbl_Discounts FOREIGN KEY
	(
	DiscountID
	) REFERENCES dbo.tbl_Discounts
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_DiscountCustomerMap SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
