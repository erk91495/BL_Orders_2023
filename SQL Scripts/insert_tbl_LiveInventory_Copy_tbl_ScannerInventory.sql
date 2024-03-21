USE [BL_Enterprise]
GO

SET IDENTITY_INSERT [dbo].[tbl_LiveInventory] ON
INSERT INTO [dbo].[tbl_LiveInventory]
           ([ID]
		   ,[LotCode]
           ,[ProductID]
           ,[PackDate]
           ,[NetWeight]
           ,[SerialNumber]
           ,[Scanline]
           ,[ScanDate])

SELECT      [ID]
		   ,[LotCode]
           ,[ProductID]
           ,[PackDate]
           ,[NetWeight]
           ,[SerialNumber]
           ,[Scanline]
           ,[ScanDate] FROM tbl_ScannerInventory
SET IDENTITY_INSERT [dbo].[tbl_LiveInventory] ON
GO


