USE [New_BL_Orders]
GO

/****** Object:  Table [dbo].[tbl_DBVersion]    Script Date: 5/25/2023 8:18:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_DBVersion](
	[Version_Major] [int] NOT NULL,
	[Version_Minor] [int] NOT NULL,
	[Version_Build] [int] NOT NULL
) ON [PRIMARY]
GO


USE [New_BL_Orders]
GO

INSERT INTO [dbo].[tbl_DBVersion]
           ([Version_Major]
           ,[Version_Minor]
           ,[Version_Build])
     VALUES
           (0,0,1)
GO
