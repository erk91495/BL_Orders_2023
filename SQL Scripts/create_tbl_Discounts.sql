/*
   Tuesday, April 9, 20249:43:58 AM
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
CREATE TABLE dbo.tbl_Discounts
	(
	ID uniqueidentifier NOT NULL ROWGUIDCOL,
	Description nvarchar(128),
	Type int NOT NULL,
	Modifier float(53) NOT NULL,
	StartDate datetime NULL,
	EndDate datetime NULL,
	Inactive bit NOT NULL DEFAULT(0)
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_Discounts ADD CONSTRAINT
	DF_tbl_Discounts_ID DEFAULT (newid()) FOR ID
GO
ALTER TABLE dbo.tbl_Discounts ADD CONSTRAINT
	PK_tbl_Discounts PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tbl_Discounts SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
