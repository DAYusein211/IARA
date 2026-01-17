-- USERS
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(30) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
CREATE INDEX idx_users_role ON Users(Role);

-- SHIPS
CREATE TABLE Ships (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    RegistrationNumber NVARCHAR(50) NOT NULL UNIQUE,
    OwnerId INT NOT NULL,
    EnginePower DECIMAL(10,2) NOT NULL,
    FuelType NVARCHAR(30) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_ships_owner FOREIGN KEY (OwnerId) REFERENCES Users(Id)
);
CREATE INDEX idx_ships_owner_id ON Ships(OwnerId);

-- PERMITS
CREATE TABLE Permits (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ShipId INT NOT NULL,
    ValidFrom DATETIME2 NOT NULL,
    ValidTo DATETIME2 NOT NULL,
    AllowedGear NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_permits_ship FOREIGN KEY (ShipId) REFERENCES Ships(Id)
);
CREATE INDEX idx_permits_ship_id ON Permits(ShipId);

-- FISHING_TRIPS
CREATE TABLE FishingTrips (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ShipId INT NOT NULL,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2 NULL,
    FuelUsed DECIMAL(10,2) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_trips_ship FOREIGN KEY (ShipId) REFERENCES Ships(Id)
);
CREATE INDEX idx_trips_ship_id ON FishingTrips(ShipId);

-- CATCHES
CREATE TABLE Catches (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FishingTripId INT NOT NULL,
    FishType NVARCHAR(100) NOT NULL,
    QuantityKg DECIMAL(10,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_catches_trip FOREIGN KEY (FishingTripId) REFERENCES FishingTrips(Id)
);
CREATE INDEX idx_catches_trip_id ON Catches(FishingTripId);

-- TICKETS
CREATE TABLE Tickets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ValidFrom DATETIME2 NOT NULL,
    ValidTo DATETIME2 NOT NULL,
    TicketType NVARCHAR(30) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_tickets_user FOREIGN KEY (UserId) REFERENCES Users(Id)
);
CREATE INDEX idx_tickets_user_id ON Tickets(UserId);

-- INSPECTIONS
CREATE TABLE Inspections (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InspectorId INT NOT NULL,
    TargetType NVARCHAR(30) NOT NULL,
    TargetId INT NOT NULL,
    InspectionDate DATETIME2 NOT NULL,
    Result NVARCHAR(30) NOT NULL,
    Notes NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_inspections_inspector FOREIGN KEY (InspectorId) REFERENCES Users(Id)
);
CREATE INDEX idx_inspections_inspector_id ON Inspections(InspectorId);

-- FINES
CREATE TABLE Fines (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InspectionId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Reason NVARCHAR(255) NOT NULL,
    IsPaid BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_fines_inspection FOREIGN KEY (InspectionId) REFERENCES Inspections(Id)
);
CREATE INDEX idx_fines_inspection_id ON Fines(InspectionId);