

UPDATE [dbo].[tblCustomerWholesale]
   SET [CustomerClassID] = (select ID from tbl_CustomerClasses WHERE CustomerClass = Class)
GO

UPDATE
[dbo].[tblCustomerWholesale]
   SET [CustomerClassID] = 1 
   WHERE [CustomerClassID] is NULL
GO

