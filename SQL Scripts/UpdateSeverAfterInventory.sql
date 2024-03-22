ALTER TABLE [dbo].[tbl_LiveInventory] DROP CONSTRAINT [FK_tbl_LiveInventory_tbl_LotCodes]
GO
/* -----------------------------------------------*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_LiveInventory ADD
	RemovedFromInventory bit NOT NULL CONSTRAINT DF_tbl_LiveInventory_RemovedFromInventory DEFAULT 0
GO
CREATE NONCLUSTERED INDEX IX_tbl_LiveInventory ON dbo.tbl_LiveInventory
	(
	RemovedFromInventory DESC
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_LiveInventory SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
/* -----------------------------------------------*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_ShipDetails ADD
	LiveInventoryID int NULL
GO
CREATE NONCLUSTERED COLUMNSTORE INDEX IX_tbl_ShipDetails ON dbo.tbl_ShipDetails
	(
	SD_ID
	) ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_ShipDetails ADD CONSTRAINT
	FK_tbl_ShipDetails_tbl_LiveInventory FOREIGN KEY
	(
	LiveInventoryID
	) REFERENCES dbo.tbl_LiveInventory
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_ShipDetails SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
/* -----------------------------------------------*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblProducts SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE dbo.tbl_InventoryAdjustments
	(
	ProductID int NOT NULL,
	ManualAdjustments int NOT NULL,
	LastAdjustment int NOT NUll,
	SortIndex smallint NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_InventoryAdjustments ADD CONSTRAINT
	PK_tbl_InventoryAdjustments PRIMARY KEY CLUSTERED 
	(
	ProductID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tbl_InventoryAdjustments ADD CONSTRAINT
	FK_tbl_InventoryAdjustments_tblProducts FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.tblProducts
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_InventoryAdjustments SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
/* -----------------------------------------------*/
BEGIN TRANSACTION
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_LiveInventory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LotCode] [nvarchar](50) NOT NULL,
	[ProductID] [int] NOT NULL,
	[PackDate] [datetime] NULL,
	[NetWeight] [real] NULL,
	[SerialNumber] [nvarchar](20) NULL,
	[Scanline] [nvarchar](70) NOT NULL,
	[ScanDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_LiveInventory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbl_LiveInventory]  WITH CHECK ADD  CONSTRAINT [FK_tbl_LiveInventory_tbl_LotCodes] FOREIGN KEY([LotCode])
REFERENCES [dbo].[tbl_LotCodes] ([LotCode])
GO

ALTER TABLE [dbo].[tbl_LiveInventory] CHECK CONSTRAINT [FK_tbl_LiveInventory_tbl_LotCodes]
GO

ALTER TABLE [dbo].[tbl_LiveInventory]  WITH CHECK ADD  CONSTRAINT [FK_tbl_LiveInventory_tblProducts] FOREIGN KEY([ProductID])
REFERENCES [dbo].[tblProducts] ([ProductID])
GO

ALTER TABLE [dbo].[tbl_LiveInventory] CHECK CONSTRAINT [FK_tbl_LiveInventory_tblProducts]
GO
COMMIT
/* -----------------------------------------------*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[tgrUpdateInventoryAdjustments]
   ON  [dbo].[tbl_InventoryAdjustments]
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
INSERT INTO [dbo].[tbl_InventoryAuditLog]
           ([TransactionDate]
           ,[WorkstationName]
           ,[UserName]
           ,[ProductID]
           ,[StartingQuantity]
           ,[AdjustmentQuantity]
           ,[EndingQuantity])
     
           SELECT GETDATE()
           ,HOST_NAME()
           ,ORIGINAL_LOGIN()
           ,i.ProductID
           ,d.ManualAdjustments
           ,i.LastAdjustment
           ,i.ManualAdjustments
		   FROM inserted i JOIN deleted d ON i.ProductID = d.ProductID


END
GO
/* -----------------------------------------------*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE usp_AdjustInventory
	-- Add the parameters for the stored procedure here
	@ProductID int,
	@Adjustment int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF
    -- Insert statements for procedure here
	UPDATE tbl_InventoryAdjustments SET 
		ManualAdjustments = @Adjustment + ManualAdjustments
		,LastAdjustment = @Adjustment
	WHERE ProductID = @ProductID
END
GO
/* -----------------------------------------------*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Eric Landes>
-- Create date: <11/01/2023>
-- Description:	<Checks the LiveInventory Table for a scanline matching @param scanline>
-- =============================================
CREATE PROCEDURE [dbo].[usp_DuplicateInventoryScanlineCheck]
	-- Add the parameters for the stored procedure here
	@scanline nvarchar(70) 

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * From tbl_LiveInventory WHERE @scanline = Scanline
END
GO
/* -----------------------------------------------*/
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
	SELECT COALESCE(s.ProductID, a.ProductID) AS ProductID, ISNULL(s.Quantity, 0) AS Quantity, ISNULL(a.ManualAdjustments,0) AS ManualAdjustments, (ISNULL(s.Quantity,0) + ISNULL(a.ManualAdjustments, 0)) AS Total, ISNULL(a.LastAdjustment, 0) AS LastAdjustment, ISNULL(a.SortIndex, 32767) AS SortIndex
FROM 
	(SELECT ProductID, COUNT(ID) AS Quantity FROM tbl_LiveInventory WHERE RemovedFromInventory = 0 GROUP BY ProductID) AS s 
FULL OUTER JOIN tbl_InventoryAdjustments a ON s.ProductID = a.ProductID
ORDER BY SortIndex, ProductID
END
GO
/* -----------------------------------------------*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE usp_InLiveInventory
	-- Add the parameters for the stored procedure here
	@ProductID int,
	@PackDate DATETIME, 
	@SerialNumber NVARCHAR(20),
	@Scanline NVARCHAR(70)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
  FROM [dbo].[tbl_LiveInventory]
  WHERE (ProductID = @ProductID AND PackDate = @PackDate AND SerialNumber LIKE @SerialNumber) OR Scanline = @Scanline

END
GO
/* -----------------------------------------------*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [usp_ZeroLiveInventory]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE tbl_LiveInventory SET RemovedFromInventory = 1 WHERE RemovedFromInventory = 0
	UPDATE tbl_InventoryAdjustments SET ManualAdjustments = 0
END
GO
/* -----------------------------------------------*/
INSERT INTO tbl_InventoryAdjustments 
SELECT ProductID, QuantityOnHand, AdjustmentQuantity, SortIndex FROM [tbl_Inventory_(Boxed)]
GO
/* -----------------------------------------------*/
SET IDENTITY_INSERT [dbo].[tbl_LiveInventory] ON
INSERT INTO [dbo].[tbl_LiveInventory]
           ([ID]
		   ,[LotCode]
           ,[ProductID]
           ,[PackDate]
           ,[NetWeight]
           ,[SerialNumber]
           ,[Scanline]
           ,[ScanDate])

SELECT      [ID]
		   ,[LotCode]
           ,[ProductID]
           ,[PackDate]
           ,[NetWeight]
           ,[SerialNumber]
           ,[Scanline]
           ,[ScanDate] FROM tbl_ScannerInventory
SET IDENTITY_INSERT [dbo].[tbl_LiveInventory] ON
GO
/* -----------------------------------------------*/