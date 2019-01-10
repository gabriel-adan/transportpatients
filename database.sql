--
-- File generated with SQLiteStudio v3.1.1 on mar. jul. 24 19:47:16 2018
--
-- Text encoding used: System

-- Table: Customer
CREATE TABLE Customer (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, Name VARCHAR (25) NOT NULL);

-- Table: DayOfWeek
CREATE TABLE DayOfWeek (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, Name VARCHAR (25) NOT NULL UNIQUE);

-- Table: Driver
CREATE TABLE Driver (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, Name VARCHAR (25) NOT NULL);

-- Table: Hour
CREATE TABLE Hour (
    Place       VARCHAR (25) NOT NULL,
    EntryTime   TIME         NOT NULL,
    ExitTime    TIME         NOT NULL,
    DayOfWeekId INTEGER      REFERENCES DayOfWeek (Id) 
                             NOT NULL,
    CustomerId  INTEGER      REFERENCES Customer (Id) 
                             NOT NULL,
                             PRIMARY KEY(EntryTime, ExitTime, DayOfWeekId, CustomerId)
);

-- Table: Transport
CREATE TABLE Transport (CustomerId INTEGER REFERENCES Customer (Id) NOT NULL, EntryTime TIME NOT NULL, ExitTime TIME NOT NULL, IsCanceled BOOLEAN NOT NULL DEFAULT (0), EntryDriverId INTEGER DEFAULT NULL, ExitDriverId INTEGER DEFAULT NULL, DayOfWeekId INTEGER NOT NULL, PRIMARY KEY (CustomerId, EntryTime, ExitTime, EntryDriverId, ExitDriverId, DayOfWeekId));

-- Trigger: hour_customer_delete
CREATE TRIGGER hour_customer_delete 
BEFORE DELETE ON Customer
FOR EACH ROW
BEGIN
    DELETE FROM Hour WHERE CustomerId = OLD.Id;
END;

-- Trigger: transport_hour_delete
CREATE TRIGGER transport_hour_delete 
BEFORE DELETE ON Hour
FOR EACH ROW
BEGIN
    DELETE FROM Transport WHERE CustomerId = OLD.CustomerId AND EntryTime = OLD.EntryTime AND ExitTime = OLD.ExitTime AND DayOfWeekId = OLD.DayOfWeekId;
END;

-- Trigger: transport_hour_insert
CREATE TRIGGER transport_hour_insert 
BEFORE INSERT ON Hour
FOR EACH ROW
BEGIN
    INSERT INTO Transport(CustomerId, EntryTime, ExitTime, EntryDriverId, ExitDriverId, DayOfWeekId, IsCanceled)
    VALUES (NEW.CustomerId, NEW.EntryTime, NEW.ExitTime, NULL, NULL, NEW.DayOfWeekId, 0);
END;

-- Trigger: transport_hour_update
CREATE TRIGGER transport_hour_update
BEFORE UPDATE ON Hour
FOR EACH ROW
BEGIN
    UPDATE Transport SET CustomerId = NEW.CustomerId, EntryTime = NEW.EntryTime, ExitTime = NEW.ExitTime, DayOfWeekId = NEW.DayOfWeekId
    WHERE CustomerId = OLD.CustomerId AND EntryTime = OLD.EntryTime AND ExitTime = OLD.ExitTime AND DayOfWeekId = OLD.DayOfWeekId;
END;
