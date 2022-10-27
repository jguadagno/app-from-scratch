# SQL Server Table Creation Scripts

These are the database scripts for the application.

- [SQL Server Table Creation Scripts](#sql-server-table-creation-scripts)
  - [Part 1 - Contacts](#part-1---contacts)
  - [Part 2 - Logging](#part-2---logging)
  - [Part 3 - Microsoft Entra](#part-3---microsoft-entra)

## Part 1 - Contacts

This is used for most of the application

```sql
use Contacts
go
create table dbo.AddressTypes
(
    AddressTypeId int identity
        primary key,
    Type          nvarchar(255) not null,
    Description   nvarchar(255) not null
)
go

create table dbo.Contacts
(
    ContactId    int identity
        primary key,
    FirstName    nvarchar(255) not null,
    MiddleName   nvarchar(255),
    LastName     nvarchar(255),
    EmailAddress nvarchar(255),
    ImageUrl     nvarchar(512),
    Birthday     datetime,
    Anniversary  datetime
)
go

create table dbo.Addresses
(
    AddressId        int identity
        primary key,
    StreetAddress    nvarchar(255) not null,
    SecondaryAddress nvarchar(255),
    Unit             nvarchar(255),
    City             nvarchar(255) not null,
    State            nvarchar(255) not null,
    Country          nvarchar(255) not null,
    PostalCode       nvarchar(255),
    AddressTypeId    int
        constraint FK_Addresses_AddressTypes_AddressTypeId
            references dbo.AddressTypes
            on delete cascade,
    ContactId        int
        constraint FK_Addresses_Contacts_ContactId
            references dbo.Contacts
            on delete cascade
)
go

create index IX_Addresses_AddressTypeId
    on dbo.Addresses (AddressTypeId)
go

create index IX_Addresses_ContactId
    on dbo.Addresses (ContactId)
go

create table dbo.PhoneTypes
(
    PhoneTypeId int identity
        primary key,
    Type        nvarchar(255) not null,
    Description nvarchar(255) not null
)
go

create table dbo.Phones
(
    PhoneId     int identity
        primary key,
    PhoneNumber nvarchar(255) not null,
    Extension   nvarchar(255),
    PhoneTypeId int
        constraint FK_Phones_PhoneTypes_PhoneTypeId
            references dbo.PhoneTypes
            on delete cascade,
    ContactId   int
        constraint FK_Phones_Contacts_ContactId
            references dbo.Contacts
            on delete cascade
)
go

create index IX_Phones_ContactId
    on dbo.Phones (ContactId)
go

create index IX_Phones_PhoneTypeId
    on dbo.Phones (PhoneTypeId)
go

```

## Part 2 - Logging

This is used for the database logging

```sql
use Contacts
go

create table Log
(
    Id          int identity
        constraint [PK_dbo.Log]
            primary key,
    MachineName nvarchar(50)  not null,
    Logged      datetime      not null,
    Level       nvarchar(50)  not null,
    Message     nvarchar(max) not null,
    Logger      nvarchar(250),
    Callsite    nvarchar(max),
    Exception   nvarchar(max)
)
go
```

## Part 3 - Microsoft Entra

This portion is used for the application login caching

```sql
use Contacts
go

create table Cache
(
    Id                         nvarchar(449)  not null
        primary key,
    Value                      varbinary(max) not null,
    ExpiresAtTime              datetimeoffset not null,
    SlidingExpirationInSeconds bigint,
    AbsoluteExpiration         datetimeoffset
)
go

create index Index_ExpiresAtTime
    on Cache (ExpiresAtTime)
go
```
