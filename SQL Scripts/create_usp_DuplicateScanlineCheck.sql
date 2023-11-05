use BL_Enterprise
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
-- Author:		<Eric Landes>
-- Create date: <11/01/2023>
-- Description:	<Checks the ShipDetails Table for a scanline matching @param scanline>
-- =============================================
CREATE PROCEDURE [dbo].[usp_DuplicateScanlineCheck]
	-- Add the parameters for the stored procedure here
	@scanline nvarchar(70) 

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * From tbl_ShipDetails WHERE @scanline = Scanline
END
GO
