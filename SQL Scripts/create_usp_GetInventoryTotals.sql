-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
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
CREATE PROCEDURE usp_GetInventoryTotals
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT COALESCE(s.ProductID, a.ProductID) AS ProductID, s.Quantity, ISNULL(a.ManualAdjustments,0) AS ManualAdjustments, (s.Quantity + ISNULL(a.ManualAdjustments,0)) AS Total, ISNULL(a.LastAdjustment, 0) AS LastAdjustment, a.SortIndex 
FROM 
	(SELECT ProductID, COUNT(ID) AS Quantity FROM tbl_LiveInventory GROUP BY ProductID) AS s 
LEFT JOIN tbl_InventoryAdjustments a ON s.ProductID = a.ProductID
ORDER BY SortIndex
END
GO
