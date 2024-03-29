/*
   Wednesday, July 26, 202311:43:55 AM
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
CREATE TABLE dbo.tbl_AllocationGroups
	(
	ID int NOT NULL IDENTITY (1, 1),
	GroupName nvarchar(50) NOT NULL,
	ProductIDs nvarchar(255) NOT NULL,
	AllocationOrder int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_AllocationGroups ADD CONSTRAINT
	PK_tbl_AllocationGroups PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tbl_AllocationGroups SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
