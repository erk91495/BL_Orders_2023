/*
   Tuesday, April 9, 20249:48:11 AM
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
ALTER TABLE dbo.tbl_Discounts SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblProducts SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.tbl_DiscountProductMap
	(
	ID uniqueidentifier NOT NULL ROWGUIDCOL,
	ProductID int NOT NULL,
	DiscountID uniqueidentifier NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_DiscountProductMap ADD CONSTRAINT
	DF_tbl_DiscountProductMap_ID DEFAULT (newid()) FOR ID
GO
ALTER TABLE dbo.tbl_DiscountProductMap ADD CONSTRAINT
	PK_tbl_DiscountProductMap PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tbl_DiscountProductMap ADD CONSTRAINT
	FK_tbl_DiscountProductMap_tblProducts FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.tblProducts
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_DiscountProductMap ADD CONSTRAINT
	FK_tbl_DiscountProductMap_tbl_Discounts FOREIGN KEY
	(
	DiscountID
	) REFERENCES dbo.tbl_Discounts
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_DiscountProductMap SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
