create database prueba
GO

USE [prueba]
GO

/****** Object:  Table [dbo].[Books]    Script Date: 28-05-2024 23:02:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Books](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BookTitle] [nvarchar](100) NOT NULL,
	[BookAuthor] [nvarchar](300) NOT NULL,
	[BookEditId] [int] NOT NULL,
	[BookYear] [int] NULL,
	[BookGenderId] [int] NOT NULL,
	[BookStateId] [int] NULL,
	[BookBC] [nvarchar](50) NOT NULL,
	[BookImage] [nvarchar](max) NULL,
	[BookCant] [int] NULL,
	[BookPrice] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Borrows]    Script Date: 28-05-2024 23:02:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Borrows](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BookId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[BorrowDate] [date] NOT NULL,
	[BorrowReturnDate] [date] NOT NULL,
	[BorrowStateId] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[BorrowStates]    Script Date: 28-05-2024 23:03:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [BorrowStates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StateName] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[BuyDetail]    Script Date: 28-05-2024 23:03:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [BuyDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BuyId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[QProduct] [int] NOT NULL,
	[ProductAmount] [int] NOT NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Buys]    Script Date: 28-05-2024 23:03:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Buys](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdUser] [int] NOT NULL,
	[ProductsQ] [int] NULL,
	[PhoneUser] [nvarchar](20) NULL,
	[DirUser] [nvarchar](100) NULL,
	[IdTransaction] [nvarchar](50) NULL,
	[BuySubtotal] [int] NULL,
	[BuyIVA] [int] NULL,
	[BuyTotalAmount] [int] NULL,
	[BuyDate] [datetime] NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Cart]    Script Date: 28-05-2024 23:03:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Cart](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[IdUser] [int] NULL,
	[IdBook] [int] NULL,
	[Q] [int] NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[EditBook]    Script Date: 28-05-2024 23:04:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [EditBook](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EditName] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Genders]    Script Date: 28-05-2024 23:04:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Genders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GenderName] [nvarchar](40) NOT NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Rol]    Script Date: 28-05-2024 23:04:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Rol](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RolName] [nvarchar](50) NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 28-05-2024 23:04:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[UserLName] [nvarchar](50) NOT NULL,
	[UserEmail] [nvarchar](100) NULL,
	[UserPass] [nvarchar](100) NOT NULL,
	[UserPhone] [nvarchar](20) NULL,
	[UserDir] [nvarchar](100) NULL,
	[UserDate] [date] NOT NULL,
	[RolId] [int] NULL
) ON [PRIMARY]
GO


