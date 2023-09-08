GO

UPDATE [dbo].[tblCustomerWholesale]
   SET [Inactive] = 1
 WHERE [Inactive] != 0
GO

ALTER TABLE [dbo].[tblCustomerWholesale] DROP CONSTRAINT [DF_tblCustomerWholesale_Inactive]
GO

ALTER TABLE [dbo].[tblCustomerWholesale]
ALTER COLUMN [Inactive] BIT
GO

ALTER TABLE [dbo].[tblCustomerWholesale] ADD  CONSTRAINT [DF_tblCustomerWholesale_Inactive]  DEFAULT ((0)) FOR [Inactive]
GO

