

ALTER TABLE [dbo].[tblCustomerWholesale] ADD [isGrocer] BIT

UPDATE [dbo].[tblCustomerWholesale]
   SET [isGrocer] = 1
 WHERE Grocer = -1
GO
UPDATE [dbo].[tblCustomerWholesale]
   SET [isGrocer] = 0
 WHERE Grocer != -1
GO
