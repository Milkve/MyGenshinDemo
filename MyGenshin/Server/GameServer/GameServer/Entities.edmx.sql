
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 03/08/2021 15:47:55
-- Generated from EDMX file: E:\MMMMMMM\MyGenshinDemo\MyGenshin\Server\GameServer\GameServer\Entities.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ExtremeWorld];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserPlayer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_UserPlayer];
GO
IF OBJECT_ID(N'[dbo].[FK_PlayerCharacter]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Characters] DROP CONSTRAINT [FK_PlayerCharacter];
GO
IF OBJECT_ID(N'[dbo].[FK_CharacterItem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CharacterItems] DROP CONSTRAINT [FK_CharacterItem];
GO
IF OBJECT_ID(N'[dbo].[FK_TGoodsLimitTCharacter]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GoodsLimits] DROP CONSTRAINT [FK_TGoodsLimitTCharacter];
GO
IF OBJECT_ID(N'[dbo].[FK_TCharacterTCharacterEquips]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CharacterEquip] DROP CONSTRAINT [FK_TCharacterTCharacterEquips];
GO
IF OBJECT_ID(N'[dbo].[FK_TCharacterTQuest]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Quests] DROP CONSTRAINT [FK_TCharacterTQuest];
GO
IF OBJECT_ID(N'[dbo].[FK_TCharacterTFriend]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Friends] DROP CONSTRAINT [FK_TCharacterTFriend];
GO
IF OBJECT_ID(N'[dbo].[FK_TCharacterTMessage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_TCharacterTMessage];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[Players]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Players];
GO
IF OBJECT_ID(N'[dbo].[Characters]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Characters];
GO
IF OBJECT_ID(N'[dbo].[CharacterItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CharacterItems];
GO
IF OBJECT_ID(N'[dbo].[GoodsLimits]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GoodsLimits];
GO
IF OBJECT_ID(N'[dbo].[CharacterEquip]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CharacterEquip];
GO
IF OBJECT_ID(N'[dbo].[Quests]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Quests];
GO
IF OBJECT_ID(N'[dbo].[Friends]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Friends];
GO
IF OBJECT_ID(N'[dbo].[Messages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Messages];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(50)  NOT NULL,
    [Password] nvarchar(50)  NOT NULL,
    [RegisterDate] datetime  NULL,
    [Player_ID] int  NOT NULL
);
GO

-- Creating table 'Players'
CREATE TABLE [dbo].[Players] (
    [ID] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'Characters'
CREATE TABLE [dbo].[Characters] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [TID] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Class] int  NOT NULL,
    [MapID] int  NOT NULL,
    [MapPosX] int  NOT NULL,
    [MapPosY] int  NOT NULL,
    [MapPosZ] int  NOT NULL,
    [MapDirection] int  NOT NULL,
    [Gold] bigint  NOT NULL,
    [Equiped] varbinary(max)  NOT NULL,
    [Level] int  NOT NULL,
    [Exp] bigint  NOT NULL,
    [Player_ID] int  NOT NULL
);
GO

-- Creating table 'CharacterItems'
CREATE TABLE [dbo].[CharacterItems] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ItemID] int  NOT NULL,
    [Count] int  NOT NULL,
    [CharacterID] int  NOT NULL,
    [Expiration] time  NULL
);
GO

-- Creating table 'GoodsLimits'
CREATE TABLE [dbo].[GoodsLimits] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [GoodsID] int  NOT NULL,
    [Purchased] smallint  NOT NULL,
    [TCharacter_ID] int  NOT NULL
);
GO

-- Creating table 'CharacterEquip'
CREATE TABLE [dbo].[CharacterEquip] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TCharacterID] int  NOT NULL,
    [Property] varbinary(max)  NOT NULL,
    [IsDelete] bit  NOT NULL,
    [TemplateID] int  NOT NULL
);
GO

-- Creating table 'Quests'
CREATE TABLE [dbo].[Quests] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TCharacterID] int  NOT NULL,
    [QuestID] int  NOT NULL,
    [Target1] int  NOT NULL,
    [Target2] int  NOT NULL,
    [Target3] int  NOT NULL,
    [Status] int  NOT NULL
);
GO

-- Creating table 'Friends'
CREATE TABLE [dbo].[Friends] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FriendID] int  NOT NULL,
    [FriendName] nvarchar(max)  NOT NULL,
    [FriendClass] int  NOT NULL,
    [FriendLevel] int  NOT NULL,
    [TCharacterID] int  NOT NULL
);
GO

-- Creating table 'Messages'
CREATE TABLE [dbo].[Messages] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] int  NOT NULL,
    [TCharacterID] int  NOT NULL,
    [FromID] int  NOT NULL,
    [Status] int  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Message] nvarchar(max)  NOT NULL,
    [Items] varbinary(max)  NOT NULL,
    [Equips] varbinary(max)  NOT NULL,
    [Gold] int  NOT NULL,
    [Exp] int  NOT NULL,
    [Time] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Players'
ALTER TABLE [dbo].[Players]
ADD CONSTRAINT [PK_Players]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Characters'
ALTER TABLE [dbo].[Characters]
ADD CONSTRAINT [PK_Characters]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'CharacterItems'
ALTER TABLE [dbo].[CharacterItems]
ADD CONSTRAINT [PK_CharacterItems]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GoodsLimits'
ALTER TABLE [dbo].[GoodsLimits]
ADD CONSTRAINT [PK_GoodsLimits]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CharacterEquip'
ALTER TABLE [dbo].[CharacterEquip]
ADD CONSTRAINT [PK_CharacterEquip]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Quests'
ALTER TABLE [dbo].[Quests]
ADD CONSTRAINT [PK_Quests]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Friends'
ALTER TABLE [dbo].[Friends]
ADD CONSTRAINT [PK_Friends]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [PK_Messages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Player_ID] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_UserPlayer]
    FOREIGN KEY ([Player_ID])
    REFERENCES [dbo].[Players]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserPlayer'
CREATE INDEX [IX_FK_UserPlayer]
ON [dbo].[Users]
    ([Player_ID]);
GO

-- Creating foreign key on [Player_ID] in table 'Characters'
ALTER TABLE [dbo].[Characters]
ADD CONSTRAINT [FK_PlayerCharacter]
    FOREIGN KEY ([Player_ID])
    REFERENCES [dbo].[Players]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PlayerCharacter'
CREATE INDEX [IX_FK_PlayerCharacter]
ON [dbo].[Characters]
    ([Player_ID]);
GO

-- Creating foreign key on [CharacterID] in table 'CharacterItems'
ALTER TABLE [dbo].[CharacterItems]
ADD CONSTRAINT [FK_CharacterItem]
    FOREIGN KEY ([CharacterID])
    REFERENCES [dbo].[Characters]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CharacterItem'
CREATE INDEX [IX_FK_CharacterItem]
ON [dbo].[CharacterItems]
    ([CharacterID]);
GO

-- Creating foreign key on [TCharacter_ID] in table 'GoodsLimits'
ALTER TABLE [dbo].[GoodsLimits]
ADD CONSTRAINT [FK_TGoodsLimitTCharacter]
    FOREIGN KEY ([TCharacter_ID])
    REFERENCES [dbo].[Characters]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TGoodsLimitTCharacter'
CREATE INDEX [IX_FK_TGoodsLimitTCharacter]
ON [dbo].[GoodsLimits]
    ([TCharacter_ID]);
GO

-- Creating foreign key on [TCharacterID] in table 'CharacterEquip'
ALTER TABLE [dbo].[CharacterEquip]
ADD CONSTRAINT [FK_TCharacterTCharacterEquips]
    FOREIGN KEY ([TCharacterID])
    REFERENCES [dbo].[Characters]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TCharacterTCharacterEquips'
CREATE INDEX [IX_FK_TCharacterTCharacterEquips]
ON [dbo].[CharacterEquip]
    ([TCharacterID]);
GO

-- Creating foreign key on [TCharacterID] in table 'Quests'
ALTER TABLE [dbo].[Quests]
ADD CONSTRAINT [FK_TCharacterTQuest]
    FOREIGN KEY ([TCharacterID])
    REFERENCES [dbo].[Characters]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TCharacterTQuest'
CREATE INDEX [IX_FK_TCharacterTQuest]
ON [dbo].[Quests]
    ([TCharacterID]);
GO

-- Creating foreign key on [TCharacterID] in table 'Friends'
ALTER TABLE [dbo].[Friends]
ADD CONSTRAINT [FK_TCharacterTFriend]
    FOREIGN KEY ([TCharacterID])
    REFERENCES [dbo].[Characters]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TCharacterTFriend'
CREATE INDEX [IX_FK_TCharacterTFriend]
ON [dbo].[Friends]
    ([TCharacterID]);
GO

-- Creating foreign key on [TCharacterID] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_TCharacterTMessage]
    FOREIGN KEY ([TCharacterID])
    REFERENCES [dbo].[Characters]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TCharacterTMessage'
CREATE INDEX [IX_FK_TCharacterTMessage]
ON [dbo].[Messages]
    ([TCharacterID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------