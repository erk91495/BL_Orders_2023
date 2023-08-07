USE [New_BL_Orders]
GO

SET IDENTITY_INSERT [dbo].[tbl_AllocationGroups] ON
INSERT INTO [dbo].[tbl_AllocationGroups]
           ([ID]
		   ,[GroupName]
           ,[ProductIDs]
           ,[AllocationOrder])
     VALUES
           (1,'ABF Whole Turkeys', '610,613,615,617,619,621,623,625,627',0),
		   (2,'NON GMO Whole Turkeys', '612,616,620,624,626',1),
		   (3,'ABF Bone In Breast', '654,657,659',2),
		   (4,'NON GMO Bone In Breast', '656,658',3)
SET IDENTITY_INSERT [dbo].[tbl_AllocationGroups] OFF
GO


