USE [BL_Enterprise]
GO
TRUNCATE TABLE [tbl_DBVersion]
GO
INSERT INTO [tbl_DBVersion] (Version_Major,Version_Minor,Version_Build) VALUES(2,0,3)
GO
USE [BL_Enterprise_North]
GO
TRUNCATE TABLE [tbl_DBVersion]
GO
INSERT INTO [tbl_DBVersion] (Version_Major,Version_Minor,Version_Build) VALUES(2,0,3)
GO