/****** Object:  Table [dbo].[tbl_CustomerClasses]    Script Date: 6/26/2023 1:27:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_CustomerClasses](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerClass] [nchar](30) NOT NULL,
	[DiscountPercent] [decimal](5, 2) NOT NULL,
 CONSTRAINT [PK_tbl_CustomerClasses] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


