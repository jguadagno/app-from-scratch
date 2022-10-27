SET IDENTITY_INSERT Contacts.dbo.AddressTypes ON
GO

INSERT INTO Contacts.dbo.AddressTypes (AddressTypeId, Type, Description) VALUES (1, N'Home', N'Home Address');
INSERT INTO Contacts.dbo.AddressTypes (AddressTypeId, Type, Description) VALUES (2, N'Work', N'Work/Office Address');

SET IDENTITY_INSERT Contacts.dbo.AddressTypes OFF
GO