USE [New_BL_Orders]
GO

INSERT INTO [dbo].[tbl_AllocationGroups]
           ([GroupName]
           ,[ProductIDs]
           ,[AllocationOrder])
     VALUES
           ('ABF Whole Turkeys', '610,613,615,617,619,621,623,625,627',0),
		   ('NON GMO Whole Turkeys', '612,616,620,624,626',1),
		   ('ABF Bone In Breast', '654,657,659',2),
		   ('NON GMO Bone In Breast', '656,658',3)

GO


