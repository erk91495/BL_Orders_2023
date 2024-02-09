USE [BL_Enterprise]

DELETE FROM [dbo].[tbl_ProductCategories] WHERE CategoryID <=17
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
	  (6, 'Breast Roast', 1, 550),
	  (7, 'DLM Turkeys',1, 560),
	  (8, 'DLM Bone-In Breast', 1, 570),
	  (9, 'ABF Deli', 0, 600),
	  (10, 'Commodity Deli', 0, 610),
	  (11, 'Smoked Parts', 0, 700),
	  (12, 'Raw Parts', 0,800),
	  (13, 'Raw Ground', 0, 810),
	  (14, 'Raw Breast Meat', 0, 900),
	  (15, 'Smoked Bone-In Breast', 0, 1000),
	  (16, 'Raw Tenders', 0, 1100),
	  (17, 'Bulk Raw Turkey', 0, 1200)

GO

SET IDENTITY_INSERT [dbo].[tbl_ProductCategories] OFF
GO