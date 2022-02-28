USE [Tasks]
GO

ALTER TABLE [dbo].[Tasks]
DROP CONSTRAINT FK_Tasks_Projetcs;
GO

/****** Object:  Table [dbo].[Tasks]    Script Date: 01.02.2022 21:19:31 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND type in (N'U'))
DROP TABLE [dbo].[Tasks]
GO

/****** Object:  Table [dbo].[Tasks]    Script Date: 01.02.2022 21:19:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Tasks](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](50) NULL,
	[Id_Project] [bigint] NULL
	CONSTRAINT FK_Tasks_Projetcs FOREIGN KEY (Id_Project)
    REFERENCES [Tasks] (id)
 CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


