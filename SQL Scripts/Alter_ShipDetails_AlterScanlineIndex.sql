USE [New_BL_Orders]
GO

/****** Object:  Index [tbl_ShipDetails$Scanline]    Script Date: 8/15/2023 3:32:32 PM ******/
DROP INDEX [tbl_ShipDetails$Scanline] ON [dbo].[tbl_ShipDetails]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [tbl_ShipDetails$Scanline]    Script Date: 8/15/2023 3:32:32 PM ******/
CREATE NONCLUSTERED INDEX [tbl_ShipDetails$Scanline] ON [dbo].[tbl_ShipDetails]
(
	[Scanline] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO


