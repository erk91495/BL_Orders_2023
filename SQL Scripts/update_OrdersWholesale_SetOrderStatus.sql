
UPDATE tblOrdersWholesale SET
OrderStatus = 999
GO

UPDATE tblOrdersWholesale SET
OrderStatus = 4
WHERE Filled = -1 AND Paid = 1 AND Shipped = 1
GO

UPDATE tblOrdersWholesale SET
OrderStatus = 3
WHERE Filled = -1  AND Paid = 0 AND Shipped = 1
GO

UPDATE tblOrdersWholesale SET
OrderStatus = 2
WHERE Filled = -1 AND Paid = 0 AND Shipped = 0
GO

UPDATE tblOrdersWholesale SET
OrderStatus = 1
WHERE Filled = 0 AND Printed = 1 AND Paid = 0 AND Shipped = 0
GO

UPDATE tblOrdersWholesale SET
OrderStatus = 0
WHERE Filled = 0 AND Printed = 0 AND Paid = 0 AND Shipped = 0
GO