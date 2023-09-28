:: Create new tables
sqlcmd -S BL4 -d BL_Enterprise_North -E -i create_tbl_AllocationGroups.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i create_tbl_InventoryAuditLog.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i create_tbl_DBVersion.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i create_tbl_CustomerClasses.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i create_tbl_CompanyInfo.sql
::update Ship Details
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_shipDetailsSerialToString.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tbl_ShipDetails_AlterScanlineIndex.sql
::update WholesaleCustomers
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tblCustomerWholesale_AddBillingInfo.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tblCustomerWholesale_AddCustomerClassID.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tblCustomerWholesale_AddUseSameAddress.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tblCustomerWholesale_Inavtive_toBit.sql
:: update Order Details
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tblOrderDetails_2_AddQuanAllocated.sql
:: update Wholesale Orders
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tblOrdersWholesale_AddAllocatedBit.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tblOrdersWholesale_AddOrderStatus.sql
:: update Products
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tblProducts_AddALU.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tblProducts_AddCompanyCode.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tblProducts_AddFixedPrice.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tblProducts_AddInactive.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tblProducts_DropExtraPrices.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i alter_tblProducts_AddIsCredit.sql
:: Create usp
sqlcmd -S BL4 -d BL_Enterprise_North -E -i usp_ProductIDExists.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i uspRptWholesaleOrderTotals.sql
:: Insert new data
sqlcmd -S BL4 -d BL_Enterprise_North -E -i insert_tbl_allocationGroups_addDefaults.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i InsertDefaultCustomerClasses.sql
:: set new columns
sqlcmd -S BL4 -d BL_Enterprise_North -E -i setCustomerClassId.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i setIsGrocerBit.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i update_OrdersWholesale_SetOrderStatus.sql
sqlcmd -S BL4 -d BL_Enterprise_North -E -i update_tblCustomerWholesale_UpdateBillingAddress.sql
:: Delete
sqlcmd -S BL4 -d BL_Enterprise_North -E -i DeletePaymentsWithoutADate.sql