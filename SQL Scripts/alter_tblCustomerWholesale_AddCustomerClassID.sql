USE New_BL_Orders
ALTER TABLE tblCustomerWholesale
ADD CustomerClassID INT NULL
GO
ALTER TABLE [dbo].[tblCustomerWholesale]  WITH CHECK ADD  CONSTRAINT [FK_tblCustomerWholesale_tbl_CustomerClasses] FOREIGN KEY([CustomerClassID])
REFERENCES [dbo].[tbl_CustomerClasses] ([ID])
GO
ALTER TABLE [dbo].[tblCustomerWholesale] CHECK CONSTRAINT [FK_tblCustomerWholesale_tbl_CustomerClasses]
GO