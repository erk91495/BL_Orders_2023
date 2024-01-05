USE [BL_Enterprise]
GO

/****** Object:  Trigger [dbo].[trgOrdersLastUpdateDate]    Script Date: 1/5/2024 10:23:46 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[tgrOrdersLastUpdateDate] 
ON [dbo].[tblOrdersWholesale] 
AFTER INSERT, UPDATE
AS
BEGIN
SET NOCOUNT ON;

	UPDATE [dbo].[tblOrdersWholesale] 
	SET LastEditTime = GetDate(),
	    LastEditor = HOST_NAME()
	FROM dbo.tblUpdate_Temp 
	INNER JOIN Inserted i
	ON dbo.tblOrdersWholesale.OrderID = i.OrderID
END
GO

ALTER TABLE [dbo].[tblUpdate_Temp] ENABLE TRIGGER [tgrOrdersLastUpdateDate]
GO


