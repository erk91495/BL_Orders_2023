INSERT INTO tbl_InventoryAdjustments 
SELECT ProductID, QuantityOnHand, AdjustmentQuantity, SortIndex FROM [tbl_Inventory_(Boxed)]