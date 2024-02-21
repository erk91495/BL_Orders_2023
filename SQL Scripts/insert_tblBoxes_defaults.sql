USE [BL_Enterprise]
GO

DELETE FROM [dbo].[tbl_Boxes] WHERE ID <= 17
GO

SET IDENTITY_INSERT [dbo].[tbl_Boxes] ON
INSERT INTO [dbo].[tbl_Boxes]
           (ID
		   ,[BoxName]
           ,[Ti_Hi]
           ,[BoxLength]
           ,[BoxWidth]
           ,[BoxHeight])
     VALUES
           (1,'B&L C-RSC ABF', 10,15.5,11.5,9),
           (2,'B&L B-RSC ABF', 12,14,10,7.75),
		   (3,'DLM C-RSC ABF', 10,15.5,11.5,9),
           (4,'DLM B-RSC ABF', 12,14,10,7.75),
		   (5,'C-RSC Plain', 10,15.5,11.5,9),
		   (6,'B-RSC Plain', 12,14,10,7.75),

		   (7,'12x8x7',16,12,8,7),
		   (8,'Breast Box', 14,10.4375,9,6),
		   (9,'Cooked Breast Box', 10,14.75,10.5,6.375),
		   (10,'Parts Box', 7,19.5,13,5.875),
		   (11,'Smoked Parts Box',7,19,12.5,7),
		   (12,'Large Can Box',8,16.5,12.5,5),
		   (13,'Small Can Box',15,12.25,9.5,5),
		   (14,'DLM Ham Box',14,10.5,8,7.25),
		   (15,'Bulk Bacon Box',8,14.5,12.75,6.5),
		   (16,'NON-GMO C-RSC ABF', 10,15.5,11.5,9),
           (17,'NON-GMO B-RSC ABF', 12,14,10,7.75)

SET IDENTITY_INSERT [dbo].[tbl_Boxes] OFF


