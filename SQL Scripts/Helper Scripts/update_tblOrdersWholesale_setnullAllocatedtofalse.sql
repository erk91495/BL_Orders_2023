USE [BL_Enterprise]
GO

UPDATE [dbo].[tblOrdersWholesale]
   SET [Allocated] = 0
  WHERE [Allocated] IS NULL
GO


