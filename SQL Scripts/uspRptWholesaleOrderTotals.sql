
/****** Object:  StoredProcedure [dbo].[uspRptWholesaleOrderTotals]    Script Date: 6/16/2023 8:59:29 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		Eric Landes
-- Create date: Jun. 16 2023
-- Description:	Return Order Details for rptWholesaleOrderTotals
-- =============================================
CREATE PROCEDURE [dbo].[uspRptWholesaleOrderTotals] 
	@BegDate datetime,
	@EndDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT DISTINCT  t.ProductID, t.ProductName, CAST(SUM(t.QuanRcvd) as int) As TotalReceived, CAST(SUM(t.Quantity) as int) as TotalQuantity
FROM (
SELECT s.OrderID, p.ProductID, p.ProductName, s.QuanRcvd, null AS Quantity FROM dbo.tbl_ShipDetails s inner JOIN dbo.tblProducts p ON s.ProdID = p.ProductID
UNION ALL
(SELECT o.OrderID, p2.ProductID, p2.ProductName, null, o.Quantity FROM tblOrderDetails_2 o INNER JOIN dbo.tblProducts p2 ON p2.ProductID = o.ProductID)
) AS t INNER JOIN dbo.tblOrdersWholesale a on t.OrderID = a.OrderID
WHERE(a.PickupDate BETWEEN @BegDate AND @EndDate)
GROUP BY t.ProductID, t.ProductName
Order by t.ProductID

END
GO


