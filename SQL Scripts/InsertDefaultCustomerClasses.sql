SET IDENTITY_INSERT [dbo].[tbl_CustomerClasses] ON 

INSERT [dbo].[tbl_CustomerClasses] ([ID], [CustomerClass], [DiscountPercent]) VALUES (1, N'Wholesale                     ', 0)
INSERT [dbo].[tbl_CustomerClasses] ([ID], [CustomerClass], [DiscountPercent]) VALUES (2, N'CO-OP                         ', 5)
INSERT [dbo].[tbl_CustomerClasses] ([ID], [CustomerClass], [DiscountPercent]) VALUES (3, N'Distributor                   ', 10)
INSERT [dbo].[tbl_CustomerClasses] ([ID], [CustomerClass], [DiscountPercent]) VALUES (4, N'Dutch Creek                   ', 15)
INSERT [dbo].[tbl_CustomerClasses] ([ID], [CustomerClass], [DiscountPercent]) VALUES (5, N'Other                         ', 20)
SET IDENTITY_INSERT [dbo].[tbl_CustomerClasses] OFF
GO
