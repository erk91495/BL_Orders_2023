USE [BL_Enterprise]
GO

BEGIN TRANSACTION
DROP VIEW _OrdersWithMemos
DROP VIEW A_OrderDetails_Test
DROP VIEW Cust_mail_lbls_Grocers
DROP VIEW Cust_mail_lblsNonGrocers
DROP VIEW Cust_Order_Picks_Boxed
DROP VIEW Cust_Order_Picks_BoxedGrocer
DROP VIEW Cust_Order_Picks_Frozen
DROP VIEW Cust_Order_Picks_mpack_grocer
DROP VIEW Cust_Order_Picks_wholesale
DROP VIEW Cust_Order_Rpt_Wholesale
DROP VIEW Cust_Order_Wholesale
DROP VIEW Cust_Order_WholesaleRpt
DROP VIEW Cust_Retail
DROP VIEW Cust_Wholesale
DROP VIEW Cust_Wholesale_All
DROP VIEW Customer_Retail
DROP VIEW Customer_Retail_SearchName
DROP VIEW CustomerWholesale_JoinOrdersWholesale
DROP VIEW FillOrders
DROP VIEW FillOrders_All
DROP VIEW Graph
DROP VIEW Graph_Box_Grocer
DROP VIEW Graph_Boxed
DROP VIEW Graph_Fresh_Box
DROP VIEW Graph_Frozen
DROP VIEW Graph_RB
DROP VIEW Graph_RBGiftBoxed
DROP VIEW Graph_RBGrocerBoxed
DROP VIEW ImportFile_View
DROP VIEW Inventory_Boxed
DROP VIEW OrderDetails_2_Quantity
DROP VIEW OrderDetails_2_WithoutMatching_Orders_Wholesale
DROP VIEW OrderDetails_CustomerWholesale
DROP VIEW OrderDetails_GetWholesalePrice
DROP VIEW OrderDetails_Retail
DROP VIEW OrderDetails_Wholesale
DROP VIEW OrderDetails_WsaleTotals
DROP VIEW OrderDetailsWholesaleJoin
DROP VIEW Orders_FillBoxTurkey
DROP VIEW Orders_FillOther
DROP VIEW Orders_FillTurkey
DROP VIEW Orders_OrdrDtl_Aggregate
DROP VIEW Orders_Retail
DROP VIEW OrdersFor_subfrmOrders_2
DROP VIEW PaymentMethods
DROP VIEW Payments
DROP VIEW PickCross
DROP VIEW Picks_Boxed
DROP VIEW Picks_Frozen
DROP VIEW Picks_GrocerBoxed
DROP VIEW Picks_Wholesale
DROP VIEW Picks_Wholesale_by_row
DROP VIEW Picks_WholesaleBox_by_row
DROP VIEW PicksRpt_Boxed
DROP VIEW PicksRpt_Frozen
DROP VIEW PicksRpt_GrocerBoxed
DROP VIEW PicksRpt_MpackGrocer
DROP VIEW PicksRpt_RB
DROP VIEW PicksRpt_RBGrocerBox
DROP VIEW PicksRpt_Wholesale
DROP VIEW PriceFactors
DROP VIEW PriceList
DROP VIEW Product_ID
DROP VIEW Product_ID_Retail
DROP VIEW Products
DROP VIEW RetailCustomerZip
DROP VIEW [RetailOrders-New]
DROP VIEW Search_WholesaleCustomer
DROP VIEW SelectionYear
DROP VIEW sfFillOrders
DROP VIEW sfOrders_FillAll
DROP VIEW ShipCustomer
DROP VIEW ShipDetails
DROP VIEW ShipDetails_NotConsolidated
DROP VIEW ShipDetails_QuanRcvd
DROP VIEW ShipDetails_SumProducts
DROP VIEW ShipDetailsCombined
DROP VIEW ShipScanRmvCustomer
DROP VIEW Switchboard_Items

DROP PROCEDURE OrderDateGet
DROP PROCEDURE usp_Cust_Order_Picks_BoxedGrocer
DROP PROCEDURE uspAggregateOrdersCheckPaid
DROP PROCEDURE uspCheckDuplicateProductID
DROP PROCEDURE uspCompanyData_PalletListGet
DROP PROCEDURE uspCompanyData_PrintPalletListGet
DROP PROCEDURE uspCompanyData_PrintPalletListUpdate
DROP PROCEDURE uspCompanyNameGet
DROP PROCEDURE uspCreateTableForExport_RetailCustomer
DROP PROCEDURE uspCreateTableForExportAll_RetailCustomer
DROP PROCEDURE uspCustomerInfoGet
DROP PROCEDURE uspCustomerInfoGetByDate
DROP PROCEDURE uspCustomerMessageGet
DROP PROCEDURE uspCustomerNameGet
DROP PROCEDURE uspCustomerWholesaleAll2_Get
DROP PROCEDURE uspCustomerWholesaleAllGet
DROP PROCEDURE uspCustomerWholesaleGet
DROP PROCEDURE uspCustomerWholesaleInactiveGet
DROP PROCEDURE uspCustomerWholesaleListForSpecificYear_Truncate
DROP PROCEDURE uspCustomerWholesaleListForSpecificYearGet
DROP PROCEDURE uspCustomerWholesaleListForSpecificYearPut
DROP PROCEDURE uspCustomerWholesaleSingleProdPerPalletGet
DROP PROCEDURE uspCustOrder_WsaleRptGet
DROP PROCEDURE uspCustOrder_WsaleRptGet2
DROP PROCEDURE uspCustOrder_WsaleRptGet3
DROP PROCEDURE uspCustOrder_WsaleSalesLastGet
DROP PROCEDURE uspCustOrderRpt_SltByNum
DROP PROCEDURE uspCustPriceGet
DROP PROCEDURE uspExtendedPrice_OutOfState_GrandTotalGet
DROP PROCEDURE uspExtendedPriceCustIDTotalGet
DROP PROCEDURE uspExtendedPriceCustomerTotalGet
DROP PROCEDURE uspExtendedPriceCustomerTotalGet2
DROP PROCEDURE uspExtendedPriceGet
DROP PROCEDURE uspExtendedPriceGet_TEST
DROP PROCEDURE uspExtendedPriceGet_WsaleOrder
DROP PROCEDURE uspExtendedPriceGrandTotalGet
DROP PROCEDURE uspExtendedPriceTotalGet
DROP PROCEDURE uspGetAggregateOrdersCustomers
DROP PROCEDURE uspGetAggregateOrdersInvoiceTotals
DROP PROCEDURE uspGetAggregateOrdersItemTotals
DROP PROCEDURE uspGetAggregateWholesaleUnpaid
DROP PROCEDURE uspGetTotalBreastQuanOrdered
DROP PROCEDURE uspInsertAggregateID
DROP PROCEDURE uspInventoryAdjustQuanOnHandUpdate
DROP PROCEDURE uspInventoryAllGet
DROP PROCEDURE uspInventoryBoxedAdjustmentTotalGet
DROP PROCEDURE uspInventoryBoxedAdjustQuanUpdate
DROP PROCEDURE uspInventoryBoxedAllGet
DROP PROCEDURE uspInventoryBoxedAllUpdate
DROP PROCEDURE uspInventoryBoxedGet
DROP PROCEDURE uspInventoryBoxedUpdate
DROP PROCEDURE uspInventoryBoxedWholeTurkeyTotalGet
DROP PROCEDURE uspInventoryGet
DROP PROCEDURE uspInvoiceTotalsGet
DROP PROCEDURE uspInvRangeTotals_BoxedUpdate
DROP PROCEDURE uspInvRangeTotals_BoxedUpdateRB
DROP PROCEDURE uspInvRangeTotals_BoxGrocerUpdate
DROP PROCEDURE uspInvRangeTotals_RBBoxedUpdate
DROP PROCEDURE uspInvRangeTotals_RBUpdate
DROP PROCEDURE uspInvVsOrdersGiftBoxByDateGet
DROP PROCEDURE uspInvVsOrdersGiftBoxByOrderGet
DROP PROCEDURE uspInvVsOrdersNGMOBoxedByDateGet
DROP PROCEDURE uspInvVsOrdersNGMOBoxedByOrderIDGet
DROP PROCEDURE uspInvVsOrdersRoastsBreastsBoxedByDateGet
DROP PROCEDURE uspInvVsOrdersRoastsBreastsBoxedByOrderIDGet
DROP PROCEDURE uspInvVsOrdersRoastsBreastsByDateGet
DROP PROCEDURE uspInvVsOrdersRoastsBreastsGrocerBoxedByDateGet
DROP PROCEDURE uspInvVsOrdersRoastsBreastsGrocerBoxedByOrderIDGet
DROP PROCEDURE uspMailLabelsGrocerGet
DROP PROCEDURE uspMailLabelsNonGrocerGet
DROP PROCEDURE uspMemo_Total_Get
DROP PROCEDURE uspNGMOPicks_Boxed_2_Get
DROP PROCEDURE uspNGMOPicks_Boxed_2_Insert
DROP PROCEDURE uspNGMOPicks_Boxed_2_Truncate
DROP PROCEDURE uspNGMOPicks_Boxed_3_Get
DROP PROCEDURE uspNGMOPicks_Boxed_3_Insert
DROP PROCEDURE uspNGMOPicks_Boxed_3_Truncate
DROP PROCEDURE uspNGMOPicks_Boxed_Get
DROP PROCEDURE uspNGMOPicks_Boxed_Insert
DROP PROCEDURE uspNGMOPicks_Boxed_QuantityGet
DROP PROCEDURE uspNGMOPicks_Boxed_Truncate
DROP PROCEDURE uspNGMOPicks_Boxed_Update
DROP PROCEDURE uspNGMOWholeTurkeysGiftBoxCheck
DROP PROCEDURE uspOrderDetails_2_ActualCustPriceUpdate
DROP PROCEDURE uspOrderDetails_2_ActualCustPriceUpdate2
DROP PROCEDURE uspOrderDetails_2_OrderGet
DROP PROCEDURE uspOrderDetails_2QuantityForTotalsGet
DROP PROCEDURE uspOrderDetails_2QuantityGet
DROP PROCEDURE uspOrderDetails_ActualCustPrice_Zero
DROP PROCEDURE uspOrderDetails_ActualCustPriceUpdate
DROP PROCEDURE uspOrderDetails_ByCustGet
DROP PROCEDURE uspOrderDetails_ByCustGet2
DROP PROCEDURE uspOrderDetails_ByDateGet
DROP PROCEDURE uspOrderDetails_ByDateGet2
DROP PROCEDURE uspOrderDetails_ByOrderIDGet
DROP PROCEDURE uspOrderDetails_ByPickupDateGet
DROP PROCEDURE uspOrderDetails_NonGMO_TurkeysGet
DROP PROCEDURE uspOrderDetails_OrderID_Update
DROP PROCEDURE uspOrderDetails_OrdersWholesaleJoinGet
DROP PROCEDURE uspOrderDetails_PalletLoadingGet
DROP PROCEDURE uspOrderDetails_ReportQuarterly
DROP PROCEDURE uspOrderDetails_ReportQuarterly_TEST
DROP PROCEDURE uspOrderDetails_Retail
DROP PROCEDURE uspOrderDetails_rptEndOfPeriodRecapAllGet
DROP PROCEDURE uspOrderDetails_rptEndOfPeriodRecapBonelessRoastGet
DROP PROCEDURE uspOrderDetails_rptEndOfPeriodRecapBoxedGet
DROP PROCEDURE uspOrderDetails_rptEndOfPeriodRecapBreastGet
DROP PROCEDURE uspOrderDetails_rptEndOfPeriodRecapFrozenGet
DROP PROCEDURE uspOrderDetails_rptEndOfPeriodRecapNotBoxedGet
DROP PROCEDURE uspOrderDetails_rptOrdersWholesaleFrozen
DROP PROCEDURE uspOrderDetails_rptOutOfStateRecapGet
DROP PROCEDURE uspOrderDetails_rptOutOfStateRecapGet_Aggregate
DROP PROCEDURE uspOrderDetails_rptPickList_All
DROP PROCEDURE uspOrderDetails_rptPickList_GrocerBoxed_SelectByNum_Get
DROP PROCEDURE uspOrderDetails_rptPickList_IndividualBoxed_SelectByNum_Get
DROP PROCEDURE uspOrderDetails_rptReportQuarterly_610sGet
DROP PROCEDURE uspOrderDetails_rptReportQuarterly_710sGet
DROP PROCEDURE uspOrderDetails_rptReportQuarterly_810sGet
DROP PROCEDURE uspOrderDetails_rptReportQuarterly_AllGet
DROP PROCEDURE uspOrderDetails_rptReportQuarterlyGet
DROP PROCEDURE uspOrderDetails_rptWholesaleOrderTotals
DROP PROCEDURE uspOrderDetails_rptWholesaleSalesLast
DROP PROCEDURE uspOrderDetails_rptWholesaleSalesLast_610s
DROP PROCEDURE uspOrderDetails_rptWholesaleSalesLast_710s
DROP PROCEDURE uspOrderDetails_rptWholesaleSalesLast_810s
DROP PROCEDURE uspOrderDetails_subfrmOrderDetails_2
DROP PROCEDURE uspOrderDetails_subfrmOrderDetails_2_2
DROP PROCEDURE uspOrderDetails_subrptPalletLoading_ByOrderIDGet
DROP PROCEDURE uspOrderDetails_subrptPalletLoadingGet
DROP PROCEDURE uspOrderDetails22_OrderID_Update
DROP PROCEDURE uspOrderDetails600sByOrderIDGet
DROP PROCEDURE uspOrderDetails600sGet
DROP PROCEDURE uspOrderDetailsWholesale_2Get
DROP PROCEDURE uspOrderDetailsWholesale_BreastRoastsGet
DROP PROCEDURE uspOrderDetailsWholesale_BreastsGet
DROP PROCEDURE uspOrderDetailsWholesale_CannedProductsGet
DROP PROCEDURE uspOrderDetailsWholesale_RoastsGet
DROP PROCEDURE uspOrderDetailsWholesale_rptInvoiceSub_2_1Get
DROP PROCEDURE uspOrderDetailsWholesale_rptInvoiceSub3Get
DROP PROCEDURE uspOrderDetailsWholesale_TurkeysGet
DROP PROCEDURE [uspOrderDetailsWholesale_UPC-KPC_Get]
DROP PROCEDURE uspOrderDetailsWholesaleGet
DROP PROCEDURE uspOrderDetailsWholesaleInsert
DROP PROCEDURE uspOrderDetailsWholesaleUpdate
DROP PROCEDURE uspOrderIDGet
DROP PROCEDURE uspOrderPaidGet
DROP PROCEDURE uspOrders_Retail
DROP PROCEDURE uspOrders_Retail_ByYearGet
DROP PROCEDURE uspOrders_RetailClosest
DROP PROCEDURE uspOrders_subfrmOrders_2
DROP PROCEDURE uspOrdersFillGet
DROP PROCEDURE uspOrdersForSpecificYearGet
DROP PROCEDURE uspOrdersScanRemoveGet
DROP PROCEDURE uspOrdersWholesale_CodesGet
DROP PROCEDURE uspOrdersWholesale_FilledUpdate
DROP PROCEDURE uspOrdersWholesale_Get
DROP PROCEDURE uspOrdersWholesale_Get2
DROP PROCEDURE uspOrdersWholesale_Get22
DROP PROCEDURE uspOrdersWholesale_MarkShippedFilled_Update
DROP PROCEDURE uspOrdersWholesale_MemoGet
DROP PROCEDURE uspOrdersWholesale_NotPaidGet
DROP PROCEDURE uspOrdersWholesale_QuanRcvd
DROP PROCEDURE uspOrdersWholesale_ShippingGet
DROP PROCEDURE uspOrdersWholesale2_OrderID_Update
DROP PROCEDURE uspOrdersWholesaleIsPaidUpdate
DROP PROCEDURE uspOrdersWholesaleScanRemoveGet
DROP PROCEDURE uspOrdersWholesaleUpdate
DROP PROCEDURE uspOrdersWholesaleUpdateShipped
DROP PROCEDURE [uspOrderTotals_Wholesale-BoxedGet]
DROP PROCEDURE [uspOrderTotals_Wholesale-BoxedUpdate]
DROP PROCEDURE [uspOrderTotals_Wholesale-BoxedUpdateRB]
DROP PROCEDURE [uspOrderTotals_Wholesale-BoxGrocerUpdate]
DROP PROCEDURE [uspOrderTotals_Wholesale-RBBoxedUpdate]
DROP PROCEDURE [uspOrderTotals_Wholesale-RBUpdate]
DROP PROCEDURE uspOrderValidate
DROP PROCEDURE uspOutstandingBalanceGet
DROP PROCEDURE uspPalletLoading_Delete
DROP PROCEDURE uspPalletLoading_PalletIDUpdate
DROP PROCEDURE uspPalletLoading_PalletsTotalUpdate
DROP PROCEDURE uspPalletLoading_ShippingUpdate
DROP PROCEDURE uspPalletLoading_Truncate
DROP PROCEDURE uspPalletLoading_Update
DROP PROCEDURE uspPalletLoading600sGet
DROP PROCEDURE uspPalletLoadingAll_Update
DROP PROCEDURE uspPalletLoadingByOrderIDGet
DROP PROCEDURE uspPalletLoadingByPalletIDGet
DROP PROCEDURE uspPalletLoadingCountGet
DROP PROCEDURE uspPalletLoadingGet
DROP PROCEDURE uspPalletLoadingInsert
DROP PROCEDURE uspPalletLoadingMixedOrderGet
DROP PROCEDURE uspPalletLoadingRB_ByOrderID_DescGet
DROP PROCEDURE uspPalletLoadingRB_ByOrderIDGet
DROP PROCEDURE uspPalletLoadingRB_Delete
DROP PROCEDURE uspPalletLoadingRB_DescGet
DROP PROCEDURE uspPalletLoadingRB_PalletIDUpdate
DROP PROCEDURE uspPalletLoadingRB_PalletsTotalUpdate
DROP PROCEDURE uspPalletLoadingRB_PalletsUpdate
DROP PROCEDURE uspPalletLoadingRB_ShippingUpdate
DROP PROCEDURE uspPalletLoadingRB_Truncate
DROP PROCEDURE uspPalletLoadingRB_Update
DROP PROCEDURE uspPalletLoadingRBAll_ByOrderIDGet
DROP PROCEDURE uspPalletLoadingRBAll_Update
DROP PROCEDURE uspPalletLoadingRBAllGet
DROP PROCEDURE uspPalletLoadingRBByPalletIDGet
DROP PROCEDURE uspPalletLoadingRBCountGet
DROP PROCEDURE uspPalletLoadingRBGet
DROP PROCEDURE uspPalletLoadingRBInsert
DROP PROCEDURE uspPalletLoadingRBMixedOrderGet
DROP PROCEDURE uspPaymentIDGet
DROP PROCEDURE uspPaymentsGet
DROP PROCEDURE uspPaymentsGet_ByDateRange
DROP PROCEDURE uspPaymentTotalsGet
DROP PROCEDURE uspPicks_Boxed_2_Truncate
DROP PROCEDURE uspPicks_Boxed_2Get
DROP PROCEDURE uspPicks_Boxed_2Insert
DROP PROCEDURE uspPicks_Boxed_2Update
DROP PROCEDURE uspPicks_Boxed_3_Truncate
DROP PROCEDURE uspPicks_Boxed_3Get
DROP PROCEDURE uspPicks_Boxed_3Insert
DROP PROCEDURE uspPicks_Boxed_3Update
DROP PROCEDURE uspPicks_Boxed_By_OrderID_Get
DROP PROCEDURE uspPicks_Boxed_Delete
DROP PROCEDURE uspPicks_Boxed_GetSelected
DROP PROCEDURE uspPicks_Boxed_Truncate
DROP PROCEDURE uspPicks_Boxed_Update
DROP PROCEDURE uspPicks_BoxedByOrderIDGet
DROP PROCEDURE uspPicks_BoxedGet
DROP PROCEDURE uspPicks_BoxedGrocer_By_OrderID_Get
DROP PROCEDURE uspPicks_BoxedGrocer_Delete
DROP PROCEDURE uspPicks_BoxedGrocer_GetSelected
DROP PROCEDURE uspPicks_BoxedGrocer_Truncate
DROP PROCEDURE uspPicks_BoxedGrocer_Update
DROP PROCEDURE uspPicks_BoxedGrocer2_Truncate
DROP PROCEDURE uspPicks_BoxedGrocer2Get
DROP PROCEDURE uspPicks_BoxedGrocer2Insert
DROP PROCEDURE uspPicks_BoxedGrocer3_Truncate
DROP PROCEDURE uspPicks_BoxedGrocer3Get
DROP PROCEDURE uspPicks_BoxedGrocer3Insert
DROP PROCEDURE uspPicks_BoxedGrocerByOrderIDGet
DROP PROCEDURE uspPicks_BoxedGrocerGet
DROP PROCEDURE uspPicks_BoxedGrocerInsert
DROP PROCEDURE uspPicks_BoxedGrocerQuantityGet
DROP PROCEDURE uspPicks_BoxedInsert
DROP PROCEDURE uspPicks_BoxedQuantityGet
DROP PROCEDURE uspPicks_CombinedBox_Truncate
DROP PROCEDURE uspPicks_CombinedBoxed_Insert
DROP PROCEDURE uspPicks_CombinedBoxGet
DROP PROCEDURE uspPicks_GetSelected
DROP PROCEDURE uspPicks_NGMOGrocerCombined_ByOrderID_Get
DROP PROCEDURE uspPicks_NGMOGrocerCombined_Get
DROP PROCEDURE uspPicks_RB_2_Truncate
DROP PROCEDURE uspPicks_RB_2Get
DROP PROCEDURE uspPicks_RB_2Insert
DROP PROCEDURE uspPicks_RB_3_Truncate
DROP PROCEDURE uspPicks_RB_3Get
DROP PROCEDURE uspPicks_RB_3Insert
DROP PROCEDURE uspPicks_RB_SelectByOrderID_Get
DROP PROCEDURE uspPicks_RB_Truncate
DROP PROCEDURE uspPicks_RBBoxGrocer_2_Truncate
DROP PROCEDURE uspPicks_RBBoxGrocer_3_Truncate
DROP PROCEDURE uspPicks_RBBoxGrocer_Truncate
DROP PROCEDURE uspPicks_RBDelete
DROP PROCEDURE uspPicks_RBGet
DROP PROCEDURE uspPicks_RBGrocer_2Insert
DROP PROCEDURE uspPicks_RBGrocer_3Insert
DROP PROCEDURE uspPicks_RBGrocer_Insert
DROP PROCEDURE uspPicks_RBGrocerBox_2Get
DROP PROCEDURE uspPicks_RBGrocerBox_3Get
DROP PROCEDURE uspPicks_RBGrocerBox_SelectByOrderID_Get
DROP PROCEDURE uspPicks_RBGrocerBoxGet
DROP PROCEDURE uspPicks_RBGrocerDelete
DROP PROCEDURE uspPicks_RBGrocerQuantityGet
DROP PROCEDURE uspPicks_RBGrocerUpdate
DROP PROCEDURE uspPicks_RBInsert
DROP PROCEDURE uspPicks_RBQuantityGet
DROP PROCEDURE uspPicks_RBUpdate
DROP PROCEDURE uspPicksRB_BoxedByOrderIDGet
DROP PROCEDURE uspPicksRB_BoxedGrocerByOrderIDGet
DROP PROCEDURE uspPriceFactorsGet
DROP PROCEDURE uspProduct_FixedWeight_Get
DROP PROCEDURE uspProduct_KPCCode_Get
DROP PROCEDURE uspProduct_NoPerCase_Get
DROP PROCEDURE uspProduct_Package_Get
DROP PROCEDURE uspProduct_PriceGet
DROP PROCEDURE uspProductIDRetailValidate
DROP PROCEDURE uspProductIDValidate
DROP PROCEDURE uspProductNameGet
DROP PROCEDURE uspProductNameRetailGet
DROP PROCEDURE uspProducts_UPC_Code_Get
DROP PROCEDURE uspReportGrocerBoxGet
DROP PROCEDURE uspRetailCustByNameGet
DROP PROCEDURE uspRetailCustomer_For_DaytonDailyNews_Get
DROP PROCEDURE uspRetailCustomer_LastFirstZipGet
DROP PROCEDURE uspRetailCustomer_LastPurchaseUpdate
DROP PROCEDURE uspRetailCustomer_NewCustomerInsert
DROP PROCEDURE uspRetailCustomerAddress_Update
DROP PROCEDURE uspRetailCustomerAllGet
DROP PROCEDURE uspRetailCustomerFullNameGet
DROP PROCEDURE uspRetailCustomerGet
DROP PROCEDURE uspRetailCustomerPut
DROP PROCEDURE uspRetailCustomerStreetAddressGet
DROP PROCEDURE uspRetailCustomerStreetAddressUpdate
DROP PROCEDURE uspRetailOrderDetails_byDateGet
DROP PROCEDURE uspRetailOrderDetails_byPhoneGet
DROP PROCEDURE uspRetailOrderDetails_SalesSummarybyDateGet
DROP PROCEDURE uspRetailOrderDetailsPut
DROP PROCEDURE uspRetailOrderNumGet
DROP PROCEDURE uspRetailOrderPut
DROP PROCEDURE uspRetailOrders_UnmatchedSelect
DROP PROCEDURE uspRetailOrderTotals_byDateGet
DROP PROCEDURE uspRetailPriorOrder_Get
DROP PROCEDURE uspRetailSoldDistributed_Get
DROP PROCEDURE uspRetailSoldDistributed_Put
DROP PROCEDURE uspRetailSoldDistributed_Update
DROP PROCEDURE uspScannerInventoryDupeScanlineCheck
DROP PROCEDURE uspScanRemoveDetails_2Get
DROP PROCEDURE uspScanRemoveDetailsGet
DROP PROCEDURE uspScanRemoveUpdate
DROP PROCEDURE uspShipDetails_ByDateGet
DROP PROCEDURE uspShipDetails_ByProductGet
DROP PROCEDURE uspShipDetails_BySerialNumberGet
DROP PROCEDURE uspShipDetails_ConsolidatedUpdate
DROP PROCEDURE uspShipDetails_PartialGet
DROP PROCEDURE uspShipDetails_rptInvoiceNumGet
DROP PROCEDURE uspShipDetails_SetAsConsolidated
DROP PROCEDURE uspShipDetails_subfrmFillOrdersGet
DROP PROCEDURE uspShipDetailsAggregateGet
DROP PROCEDURE uspShipDetailsCombined_AddRecord
DROP PROCEDURE uspShipDetailsConsolidate
DROP PROCEDURE uspShipDetailsConsolidateThis
DROP PROCEDURE uspShipDetailsDelete
DROP PROCEDURE uspShipDetailsDeleteRecord
DROP PROCEDURE uspShipDetailsDupeScanlineCheck
DROP PROCEDURE uspShipDetailsGet
DROP PROCEDURE uspShipDetailsGet2
DROP PROCEDURE uspShipDetailsGroupBy
DROP PROCEDURE uspShipDetailsJoinGet
DROP PROCEDURE uspShipDetailsNotConsolidatedGet
DROP PROCEDURE uspShipDetailsNotConsolidatedGetThis
DROP PROCEDURE uspShipDetailsNotShippedGet
DROP PROCEDURE uspShipDetailsNotShippedGetThis
DROP PROCEDURE uspShipDetailsReportGet
DROP PROCEDURE uspShipScanRmvCustomerInsert
DROP PROCEDURE uspTotalsForWsaleRecapReports_Get
DROP PROCEDURE uspTotalsForWsaleRecapReports_Insert
DROP PROCEDURE uspTotalsForWsaleRecapReports_Truncate
DROP PROCEDURE uspTruncateAggregateID
DROP PROCEDURE uspUpdate_TempInsert
DROP PROCEDURE uspWebImport_Insert
DROP PROCEDURE uspWebImportFull_Insert
DROP PROCEDURE uspWholeTurkeys2Get
DROP PROCEDURE uspWholeTurkeys3Get
DROP PROCEDURE uspWholeTurkeysAllCategoriesGet
DROP PROCEDURE uspWholeTurkeysGet
DROP PROCEDURE uspWholeTurkeysGiftBoxCheck
DROP PROCEDURE uspWholeTurkeysGiftBoxGet

DROP FUNCTION fn_diagramobjects
DROP FUNCTION udf_ABF_NetCalc
DROP FUNCTION udf_ABF_PriceCalc
DROP FUNCTION udf_ABF_RBNetCalc
DROP FUNCTION udf_ABF_RBPriceCalc
DROP FUNCTION udf_ABF_RBSumGet
DROP FUNCTION udf_BR_NetCalc
DROP FUNCTION udf_BR_PriceCalc
DROP FUNCTION udf_DLane_NetCalc
DROP FUNCTION udf_DLane_PriceCalc
DROP FUNCTION udf_DLane_RBNetCalc
DROP FUNCTION udf_DLane_RBPriceCalc
DROP FUNCTION udf_DLane_RBSumGet
DROP FUNCTION udf_NonGMO_NetCalc
DROP FUNCTION udf_NonGMO_RBPriceCalc
DROP FUNCTION udf_NonGMO_RBSumGet
DROP FUNCTION udf_RB_NetCalc
DROP FUNCTION udf_RB_PriceCalc
DROP FUNCTION udf_SBI_NetCalc
DROP FUNCTION udf_SBI_PriceCalc
DROP FUNCTION udf_SBR_PriceCalc
DROP FUNCTION udf_T_NetCalc
DROP FUNCTION udf_T_PriceCalc
DROP FUNCTION udf_T610_NetCalc
DROP FUNCTION udf_T610_PriceCalc
DROP FUNCTION udf_T710_NetCalc
DROP FUNCTION udf_T710_PriceCalc
DROP FUNCTION udf_T810_NetCalc
DROP FUNCTION udf_T810_PriceCalc
DROP FUNCTION udfBonelessRoastSumGet
DROP FUNCTION udfBreastRoastSumGet
DROP FUNCTION udfCustomerPriceCalc
DROP FUNCTION udfCustPriceGet
DROP FUNCTION udfExtendedPriceCalc
DROP FUNCTION udfExtendedPriceGet
DROP FUNCTION udfExtendedPriceGet2
DROP FUNCTION udfNetWeightCalc
DROP FUNCTION udfOrderDetailsInfoGet
DROP FUNCTION udfProductNameGet
DROP FUNCTION udfProductNameGetScalar
DROP FUNCTION udfProductNameRetailGet
DROP FUNCTION udfProductNameWholesaleGet
DROP FUNCTION udfProductsPackageGet
DROP FUNCTION udfRoastBreastSumGet
DROP FUNCTION udfRoastsBreastRetailSumGet
DROP FUNCTION udfRoastsBreastSumGet
DROP FUNCTION udfShipMethodGet
DROP FUNCTION udfSmokeBoneInBreastSumGet
DROP FUNCTION udfWholeSmokeTurkeySumGet
DROP FUNCTION udfWholeTurkeySumGet

DROP TABLE tbl_DailyTotals
DROP TABLE tbl_DailyTotals_Temp
DROP TABLE tbl_InvRangeTotals
DROP TABLE [tbl_InvRangeTotals_(Boxed)]
DROP TABLE [tbl_InvRangeTotals_(BoxGrocer)]
DROP TABLE [tbl_InvRangeTotals_(Fresh-Box)]
DROP TABLE [tbl_InvRangeTotals_(Frozen)]
DROP TABLE [tbl_InvRangeTotals_(RB)]
DROP TABLE [tbl_InvRangeTotals_(RBBoxed)]
DROP TABLE tbl_NGMOPicks_Boxed
DROP TABLE tbl_NGMOPicks_Boxed_2
DROP TABLE tbl_NGMOPicks_Boxed_3
DROP TABLE tbl_PackageType
DROP TABLE tbl_PalletLoading
DROP TABLE [tbl_PalletLoading_(RB)]
DROP TABLE tbl_PickCross
DROP TABLE tbl_Picks_Boxed
DROP TABLE tbl_Picks_boxed_2
DROP TABLE tbl_Picks_boxed_3
DROP TABLE tbl_Picks_BoxGrocer
DROP TABLE tbl_Picks_BoxGrocer_2
DROP TABLE tbl_Picks_BoxGrocer_3
DROP TABLE tbl_Picks_Combined
DROP TABLE tbl_Picks_CombinedBox
DROP TABLE tbl_Picks_RB
DROP TABLE tbl_Picks_RB_2
DROP TABLE tbl_Picks_RB_3
DROP TABLE tbl_Picks_RBBoxGrocer
DROP TABLE tbl_Picks_RBBoxGrocer_2
DROP TABLE tbl_Picks_RBBoxGrocer_3
DROP TABLE tbl_Picks_Wholesale
DROP TABLE tbl_Picks_Wholesale_2
DROP TABLE tbl_Picks_Wholesale_3
DROP TABLE tbl_Picks_WholesaleBoxByRow
DROP TABLE tbl_Picks_WholesaleByRow
DROP TABLE tbl_ScanRemove
DROP TABLE tbl_ShipCustomer
DROP TABLE tbl_ShipDetailsCombined
DROP TABLE tbl_ShipScanRmvCustomer
DROP TABLE tbl_TotalsForWsaleRecapReports
DROP TABLE tbl_WebImport
DROP TABLE tblCustomerWholesaleListForSpecificYear
DROP TABLE tblOrders_BoxTurkeys
DROP TABLE tblOrders_Other
DROP TABLE tblOrders_Turkeys
DROP TABLE tblOrdersWholesale_2
DROP TABLE [tblOrderTotals_(BoxGrocer)]
DROP TABLE [tblOrderTotals_(Fresh-Box)]
DROP TABLE [tblOrderTotals_(Frozen)]
DROP TABLE [tblOrderTotals_(Wholesale)]
DROP TABLE [tblOrderTotals_(Wholesale-Boxed)]
DROP TABLE [tblOrderTotals_(Wholesale-RB)]
DROP TABLE [tblOrderTotals_(Wholesale-RBBoxed)]
DROP TABLE tblPriceFactors
DROP TABLE tblRetailOrderDetails
DROP TABLE tblRetailOrders
DROP TABLE tblRetailCustomerForExport
DROP TABLE tblRetailCustomer
DROP TABLE tblRetailProducts
DROP TABLE tblRetailSoldDistributed
DROP TABLE tblSelectionYear
DROP TABLE tblSwitchboard_Items
DROP TABLE tblUpdate_Temp
DROP TABLE tbl_PicksAndOrdered
COMMIT