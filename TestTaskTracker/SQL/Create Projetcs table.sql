USE [Tasks]
GO

/****** Object:  Table [dbo].[Tasks]    Script Date: 31.01.2022 8:59:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Projects](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED (
	[Id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [FK_Projects_Tasks] FOREIGN KEY (Id)
 REFERENCES [dbo].[Tasks] (Id)
) ON [PRIMARY]  -- About  ON [PRIMARY] https://stackoverflow.com/questions/2798213/what-does-on-primary-mean
GO