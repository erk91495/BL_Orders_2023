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
CREATE PROCEDURE [dbo].[usp_GetProductTotals]
	-- Add the parameters for the stored procedure here
	@startDate DateTime,
	@endDate DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT p.ProductID, p.ProductName, ISNULL(SUM(d.QuanRcvd),0) AS QuantityReceived, ISNULL(SUM(d.PickWeight),0) AS NetWeight, SUM(ISNULL(d.PickWeight, 0) * ISNULL(d.ActualCustPrice,0)) AS ExtPrice
	FROM tblProducts p 
	LEFT JOIN 
	(SELECT d.OrderID, d.ProdID, d.QuanRcvd, d.PickWeight, o.ActualCustPrice 
	FROM tblOrderDetails_2 o 
	LEFT JOIN tbl_ShipDetails d ON d.OrderID = o.OrderID AND d.ProdID = o.ProductID 
	LEFT JOIN tblOrdersWholesale orders ON orders.OrderID = o.OrderID  WHERE orders.PickupDate BETWEEN @startDate AND @endDate) d 
	ON p.ProductID = d.ProdID
	WHERE p.Inactive <> 1 GROUP BY p.ProductID, p.ProductName
	ORDER BY ProductID
END
GO
