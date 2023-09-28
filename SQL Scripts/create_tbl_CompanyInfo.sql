USE [BL_Enterprise_North]
/****** Object:  Table [dbo].[tbl_CompanyInfo]    Script Date: 9/26/2023 3:19:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_CompanyInfo](
	[LongCompanyName] [varchar](255) NOT NULL,
	[ShortCompanyName] [varchar](100) NOT NULL,
	[StreetAddress] [varchar](255) NOT NULL,
	[City] [varchar](255) NOT NULL,
	[State] [varchar](20) NOT NULL,
	[ShortState] [varchar](2) NOT NULL,
	[ShortZipCode] [varchar](5) NOT NULL,
	[LongZipCode] [nchar](10) NOT NULL,
	[Phone] [nchar](10) NOT NULL,
	[Fax] [nchar](10) NOT NULL,
	[Website] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[ID] [int] NOT NULL,
 CONSTRAINT [PK_tbl_CompanyInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbl_CompanyInfo] ADD  CONSTRAINT [DF_tbl_CompanyInfo_ID]  DEFAULT ((1)) FOR [ID]
GO


