USE [BL_Enterprise]
GO

/****** Object:  Trigger [dbo].[tgrUpdateInventoryAdjustments]    Script Date: 5/8/2024 4:20:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TRIGGER [dbo].[tgrUpdateInventoryAdjustments]
   ON  [dbo].[tbl_InventoryAdjustments]
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
INSERT INTO [dbo].[tbl_InventoryAuditLog]
           ([TransactionDate]
           ,[WorkstationName]
           ,[UserName]
           ,[ProductID]
           ,[StartingQuantity]
           ,[AdjustmentQuantity]
           ,[EndingQuantity]
		   ,[AdjustmentReason])
     
           SELECT GETDATE()
           ,HOST_NAME()
           ,ORIGINAL_LOGIN()
           ,i.ProductID
           ,d.ManualAdjustments
           ,i.LastAdjustment
           ,i.ManualAdjustments
		   ,i.LastAdjustmentReason
		   FROM inserted i JOIN deleted d ON i.ProductID = d.ProductID


END
GO


