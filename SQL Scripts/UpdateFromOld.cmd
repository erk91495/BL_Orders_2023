:: Create new tables
sqlcmd -S Eric-PC -d BL_Enterprise -E -i create_tbl_AllocationGroups.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i create_tbl_InventoryAuditLog.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i create_tbl_DBVersion.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i create_tbl_CustomerClasses.sql
::update Ship Details
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_shipDetailsSerialToString.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_tbl_ShipDetails_AlterScanlineIndex.sql
::update WholesaleCustomers
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_tblCustomerWholesale_AddBillingInfo.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_tblCustomerWholesale_AddCustomerClassID.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_tblCustomerWholesale_AddUseSameAddress.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_tblCustomerWholesale_Inavtive_toBit.sql
:: update Order Details
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_tblOrderDetails_2_AddQuanAllocated.sql
:: update Wholesale Orders
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_tblOrdersWholesale_AddAllocatedBit.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_tblOrdersWholesale_AddOrderStatus.sql
:: update Products
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_tblProducts_AddALU.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_tblProducts_AddCompanyCode.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_tblProducts_AddFixedPrice.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_tblProducts_AddInactive.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i alter_tblProducts_DropExtraPrices.sql
:: Create usp
sqlcmd -S Eric-PC -d BL_Enterprise -E -i usp_ProductIDExists.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i uspRptWholesaleOrderTotals.sql
:: Insert new data
sqlcmd -S Eric-PC -d BL_Enterprise -E -i insert_tbl_allocationGroups_addDefaults.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i InsertDefaultCustomerClasses.sql
:: set new columns
sqlcmd -S Eric-PC -d BL_Enterprise -E -i setCustomerClassId.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i setIsGrocerBit.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i update_OrdersWholesale_SetOrderStatus.sql
sqlcmd -S Eric-PC -d BL_Enterprise -E -i update_tblCustomerWholesale_UpdateBillingAddress.sql
:: Delete
sqlcmd -S Eric-PC -d BL_Enterprise -E -i DeletePaymentsWithoutADate.sql