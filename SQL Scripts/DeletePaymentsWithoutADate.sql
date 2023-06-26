USE [New_BL_Orders]
GO

DELETE
  FROM [dbo].[tblPayments]
  WHERE PaymentDate IS NULL

GO


