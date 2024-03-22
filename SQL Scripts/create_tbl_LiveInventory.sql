USE [BL_Enterprise]
GO

/****** Object:  Table [dbo].[tbl_ScannerInventory]    Script Date: 3/11/2024 4:44:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_LiveInventory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LotCode] [nvarchar](50) NOT NULL,
	[ProductID] [int] NOT NULL,
	[PackDate] [datetime] NULL,
	[NetWeight] [real] NULL,
	[SerialNumber] [nvarchar](20) NULL,
	[Scanline] [nvarchar](70) NOT NULL,
	[ScanDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_LiveInventory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbl_LiveInventory]  WITH CHECK ADD  CONSTRAINT [FK_tbl_LiveInventory_tbl_LotCodes] FOREIGN KEY([LotCode])
REFERENCES [dbo].[tbl_LotCodes] ([LotCode])
GO

ALTER TABLE [dbo].[tbl_LiveInventory] CHECK CONSTRAINT [FK_tbl_LiveInventory_tbl_LotCodes]
GO

ALTER TABLE [dbo].[tbl_LiveInventory]  WITH CHECK ADD  CONSTRAINT [FK_tbl_LiveInventory_tblProducts] FOREIGN KEY([ProductID])
REFERENCES [dbo].[tblProducts] ([ProductID])
GO

ALTER TABLE [dbo].[tbl_LiveInventory] CHECK CONSTRAINT [FK_tbl_LiveInventory_tblProducts]
GO


