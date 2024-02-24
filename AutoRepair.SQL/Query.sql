-- -- -- -- -- -- -- --
-- CREATING DATABASE --
-- -- -- -- -- -- -- --
create database AutoRepair;
go

-- -- -- -- -- -- --
-- USE CREATED DB --
-- -- -- -- -- -- --
use AutoRepair;
go

-- -- -- -- -- -- -- --
-- CREATING ENTITIES --
-- -- -- -- -- -- -- --
-- Role: 4 roles
-- Role 1: admin
-- Role 2: accountant
-- Role 3: master
-- Role 4: client
create table [Role] (
	[ID] uniqueidentifier primary key default newid() not null,
	[Name] nvarchar(15) not null,
	[Description] nvarchar(50) not null
);
go

-- Email: example@exapmle.example
-- PhoneNumber: 12345678900
create table [User] (
	[ID] uniqueidentifier primary key default newid() not null,
	[Email] nvarchar(100) not null,
	[Password] nvarchar(100) not null,
	[RoleID] uniqueidentifier not null,
	foreign key ([RoleID]) references [Role]([ID])
);
go

create table [Info] (
	[ID] uniqueidentifier primary key default newid() not null,
	[LastName] nvarchar(50) not null,
	[FirstName] nvarchar(50) not null,
	[MiddleName] nvarchar(50),
	[PhoneNumber] nvarchar(12) not null,
	[Address] nvarchar(100) not null
);
go

-- isInRoyalProgram: true or false
create table [Client] (
	[ID] uniqueidentifier primary key default newid() not null,
	[InfoID] uniqueidentifier not null,
	[isInLoyalProgram] bit not null,
	[isActive] bit not null,
	[UserID] uniqueidentifier not null,
	foreign key ([UserID]) references [User]([ID]) on delete cascade,
	foreign key ([InfoID]) references [Info]([ID]) on delete cascade
);
go

create table [Car] (
	[ID] uniqueidentifier primary key default newid() not null,
	[Name] nvarchar(50) not null,
	[Color] nvarchar(20) not null,
	[LicensePlate] nvarchar(9) not null,
	[isActive] bit not null,
	[OwnerID] uniqueidentifier not null,
	foreign key ([OwnerID]) references [Client]([ID])
);
go

create table [Accountant] (
	[ID] uniqueidentifier primary key default newid() not null,
	[InfoID] uniqueidentifier not null,
	[isActive] bit not null,
	[UserID] uniqueidentifier not null,
	foreign key ([InfoID]) references [Info]([ID]),
	foreign key ([UserID]) references [User]([ID])
);
go

create table [Specialty] (
	ID uniqueidentifier primary key default newid() not null,
	[Name] nvarchar(50) not null,
	[Description] nvarchar(100) not null
);
go

create table [Master] (
	[ID] uniqueidentifier primary key default newid() not null,
	[InfoID] uniqueidentifier not null,
	[Balance] money not null,
	[isActive] bit not null,
	[UserID] uniqueidentifier not null,
	[SpecialtyID] uniqueidentifier,
	foreign key ([InfoID]) references [Info]([ID]) on delete cascade,
	foreign key ([UserID]) references [User]([ID]),
	foreign key ([SpecialtyID]) references [Specialty]([ID])
);
go

create table [Service] (
	[ID] uniqueidentifier primary key default newid() not null,
	[Name] nvarchar(50) not null,
	[Description] nvarchar(100) not null,
	[Price] money not null,
	[SpecialtyID] uniqueidentifier,
	foreign key ([SpecialtyID]) references [Specialty]([ID])
);
go

-- Status: 5 steps
-- setp 0: Rejected
-- step 1: Waiting...
-- setp 2: Accepted!
-- step 3: Repairing...
-- step 4: Finished
create table [Status] (
	[ID] uniqueidentifier primary key default newid() not null,
	[Name] nvarchar(20) not null
);
go

-- Date: YYYY-MM-DD
create table [Order] (
  [ID] uniqueidentifier primary key default newid() not null,
  [ClientID] uniqueidentifier not null,
  [CarID] uniqueidentifier,
  [MasterID] uniqueidentifier,
  [ServiceID] uniqueidentifier not null,
  [Date] date not null,
  [StatusID] uniqueidentifier not null,
  [isPaidOut] bit not null,
  foreign key ([ClientID]) references [Client]([ID]),
  foreign key ([CarID]) references [Car]([ID]) on delete cascade,
  foreign key ([MasterID]) references [Master]([ID]),
  foreign key ([ServiceID]) references [Service]([ID]),
  foreign key ([StatusID]) references [Status]([ID])
);
go

-- -- -- -- -- -- -- -- -- -- --
-- INSERTING DATA IN ENTITIES --
-- -- -- -- -- -- -- -- -- -- --
insert into [Status] ([Name]) values
('Rejected'),
('Waiting...'),
('Accepted!'),
('Repairing...'),
('Finished');
go

insert into [Role] ([Name], [Description]) values
('admin','Write description here.'),
('accountant','Write description here.'),
('master','Write description here.'),
('client','Write description here.');
go

insert into [User] ([Email], [Password], [RoleID]) values
(
	'admin@admin', 
	'admin', 
	(select top 1 [ID] from [Role] where [Name] = 'admin')
);