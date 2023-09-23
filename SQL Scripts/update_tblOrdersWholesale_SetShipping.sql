Use BL_Enterprise

update [tblOrdersWholesale]
set Shipping = 3 
Where Shipping = 1
go

update [tblOrdersWholesale]
set Shipping = 1 
Where Shipping = 2
go

update [tblOrdersWholesale]
set Shipping = 2 
Where Shipping = 3
go