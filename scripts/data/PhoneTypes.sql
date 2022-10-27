SET IDENTITY_INSERT Contacts.dbo.PhoneTypes ON
GO

INSERT INTO Contacts.dbo.PhoneTypes (PhoneTypeId, Type, Description) VALUES (1, N'Mobile', N'Mobile/Cell');
INSERT INTO Contacts.dbo.PhoneTypes (PhoneTypeId, Type, Description) VALUES (2, N'Office', N'Office');
INSERT INTO Contacts.dbo.PhoneTypes (PhoneTypeId, Type, Description) VALUES (3, N'Fax', N'Fax');
INSERT INTO Contacts.dbo.PhoneTypes (PhoneTypeId, Type, Description) VALUES (4, N'Home', N'Home');

SET IDENTITY_INSERT Contacts.dbo.PhoneTypes OFF
GO