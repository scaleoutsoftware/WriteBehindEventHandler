SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Table [dbo].[SampleObjectTable]    Script Date: 6/6/2018 4:57:26 PM ******/
CREATE TABLE [dbo].[SampleObjectTable](
	[ID] [int] NOT NULL,
	[IntVal] [int] NULL,
	[DoubleVal] [float] NULL,
	[StringVal] [nchar](255) NULL
 CONSTRAINT [PK_SampleObjectTable] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  StoredProcedure [dbo].[AddOrUpdate]    Script Date: 6/6/2018 4:57:26 PM ******/
CREATE PROCEDURE [dbo].[AddOrUpdate]
		@ID int,
		@IntVal int,
		@DoubleVal float,
		@StringVal nchar(255)
AS
UPDATE [dbo].[SampleObjectTable]
	SET IntVal=@IntVal, DoubleVal=@DoubleVal, StringVal=@StringVal
	WHERE ID=@ID
IF @@ROWCOUNT = 0 AND @@ERROR = 0
BEGIN
    INSERT INTO [dbo].[SampleObjectTable] ([ID],[IntVal],[DoubleVal],[StringVal])
	     VALUES (@ID, @IntVal, @DoubleVal, @StringVal)
END
GO

/****** Object:  StoredProcedure [dbo].[Delete]    Script Date: 6/6/2018 4:57:26 PM ******/
Create PROCEDURE [dbo].[Delete]
		@ID int
AS
DELETE FROM [dbo].[SampleObjectTable] WHERE ID=@ID