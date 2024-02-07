USE [BL_Enterprise]

DELETE FROM [dbo].[tbl_ProductCategories] WHERE CategoryID <=6
GO

SET IDENTITY_INSERT [dbo].[tbl_ProductCategories] ON
GO

INSERT INTO [dbo].[tbl_ProductCategories] 
			(CategoryID
			,CategoryName
			,ShowTotalsOnReports
			,DisplayIndex)
	  VALUES
	  (1, 'Whole Turkeys',1,500),
	  (2, 'NON GMO Turkeys', 1, 510),
	  (3, 'Roasts', 1, 520),
	  (4, 'Bone-In Breast', 1, 530),
	  (5, 'NON GMO Breast', 1, 540),
	  (6, 'Breast Roast', 1, 550)
GO

SET IDENTITY_INSERT [dbo].[tbl_ProductCategories] OFF
GO