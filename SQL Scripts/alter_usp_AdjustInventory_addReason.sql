USE [BL_Enterprise]
GO

/****** Object:  StoredProcedure [dbo].[usp_AdjustInventory]    Script Date: 5/8/2024 4:36:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[usp_AdjustInventory]
	-- Add the parameters for the stored procedure here
	@ProductID int,
	@Adjustment int,
	@Reason nvarchar(255) NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF
    -- Insert statements for procedure here
	UPDATE tbl_InventoryAdjustments SET 
		ManualAdjustments = @Adjustment + ManualAdjustments
		,LastAdjustment = @Adjustment,
		LastAdjustmentReason = @Reason
	WHERE ProductID = @ProductID
END
GO


