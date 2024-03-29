USE [BL_Enterprise]
GO

/****** Object:  Table [dbo].[tbl_LiveInventoryRemovalReason]    Script Date: 3/29/2024 8:43:57 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_LiveInventoryRemovalReason](
	[ID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[RemovalReason] [nvarchar](100) NULL,
 CONSTRAINT [PK_tbl_LiveInventoryRemovalReason] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbl_LiveInventoryRemovalReason] ADD  CONSTRAINT [DF_tbl_LiveInventoryRemovalReason_ID]  DEFAULT (newid()) FOR [ID]
GO


