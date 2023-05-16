USE [New_BL_Orders]
GO

UPDATE [dbo].[tblCustomerWholesale]
   SET 
      [BillingAddress] = Address
      ,[BillingCity] = City
      ,[BillingState] = State
      ,[BillingZipCode] = ZipCode
GO


