
DROP INDEX [tbl_ShipDetails$PackageSerialNumber] ON [dbo].[tbl_ShipDetails]
GO

ALTER TABLE [dbo].[tbl_ShipDetails] DROP CONSTRAINT [DF__tbl_ShipD__Packa__679450C0]
GO

ALTER TABLE [dbo].[tbl_ShipDetails]
ALTER COLUMN PackageSerialNumber NVarchar(20)
GO


ALTER TABLE [dbo].[tbl_ShipDetails] ADD  DEFAULT ((0)) FOR [PackageSerialNumber]
GO

CREATE NONCLUSTERED INDEX [tbl_ShipDetails$PackageSerialNumber] ON [dbo].[tbl_ShipDetails]
(
	[PackageSerialNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
