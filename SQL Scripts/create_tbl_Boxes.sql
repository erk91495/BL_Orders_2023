USE [BL_Enterprise]
GO

/****** Object:  Table [dbo].[tbl_Boxes]    Script Date: 12/1/2023 3:17:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_Boxes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[BoxName] [nvarchar](100) NOT NULL,
	[Ti_Hi] [int] NOT NULL,
	[BoxLength] [float] NULL,
	[BoxWidth] [float] NULL,
	[BoxHeight] [float] NULL,
 CONSTRAINT [PK_tbl_Boxes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


