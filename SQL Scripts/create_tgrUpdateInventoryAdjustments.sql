-- ================================================
-- Template generated from Template Explorer using:
-- Create Trigger (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- See additional Create Trigger templates for more
-- examples of different Trigger statements.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[tgrUpdateInventoryAdjustments]
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
           ,[EndingQuantity])
     
           SELECT GETDATE()
           ,HOST_NAME()
           ,ORIGINAL_LOGIN()
           ,i.ProductID
           ,d.ManualAdjustments
           ,i.LastAdjustment
           ,i.ManualAdjustments
		   FROM inserted i JOIN deleted d ON i.ProductID = d.ProductID


END
GO
