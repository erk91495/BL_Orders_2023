USE [New_BL_Orders]
GO

UPDATE [dbo].[tblCustomerWholesale]
   SET [IsGrocer] = 1
 WHERE Grocer = -1
GO
UPDATE [dbo].[tblCustomerWholesale]
   SET [IsGrocer] = 0
 WHERE Grocer != -1
GO
