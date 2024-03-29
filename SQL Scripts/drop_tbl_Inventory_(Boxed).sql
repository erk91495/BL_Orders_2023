USE [BL_Enterprise]
GO

/****** Object:  Table [dbo].[tbl_Inventory_(Boxed)]    Script Date: 3/29/2024 8:32:29 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tbl_Inventory_(Boxed)]') AND type in (N'U'))
DROP TABLE [dbo].[tbl_Inventory_(Boxed)]
GO


