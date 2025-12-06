------------------------------------------------
USE master;
GO

-- 1. DROP AND CREATE DATABASE
IF DB_ID(N'GroundShare') IS NOT NULL
BEGIN
    ALTER DATABASE GroundShare SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE GroundShare;
END
GO

CREATE DATABASE GroundShare;
GO

USE GroundShare;
GO

------------------------------------------------
-- 2. CREATE TABLES
------------------------------------------------

-- Users
CREATE TABLE [Users] (
    UserId       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FirstName    NVARCHAR(50)  NOT NULL,
    LastName     NVARCHAR(50)  NOT NULL,
    Email        NVARCHAR(255) NOT NULL UNIQUE,
    [Password]   NVARCHAR(50)  NOT NULL,
    DateOfBirth  DATE          NOT NULL,
    PhoneNumber  NVARCHAR(20)  NOT NULL,
    [Address]    NVARCHAR(255) NOT NULL,
    IsAdmin      BIT           DEFAULT 0
);
GO

-- Locations
CREATE TABLE [Locations] (
    LocationsId  INT IDENTITY(1,1) PRIMARY KEY,
    City         NVARCHAR(100) NOT NULL,
    Street       NVARCHAR(100) NOT NULL,
    HouseNumber  NVARCHAR(10)  NOT NULL,
    HouseType    NVARCHAR(50)  NOT NULL,
    Floor        INT           NULL
);
GO

-- Events
CREATE TABLE [Events] (
    EventsId       INT IDENTITY(1,1) PRIMARY KEY,
    StartDateTime  DATETIME2      NOT NULL,
    EndDateTime    DATETIME2      NULL,
    EventsType     NVARCHAR(100)  NOT NULL,
    PhotoUrl       NVARCHAR(500)  NULL,
    [Description]  NVARCHAR(1000) NULL,
    Municipality   NVARCHAR(100)  NOT NULL,
    ResponsibleBody NVARCHAR(150) NOT NULL,
    EventsStatus   NVARCHAR(20)   NOT NULL,
    LocationsId    INT            NOT NULL,

    CONSTRAINT FK_Events_Locations
        FOREIGN KEY (LocationsId) REFERENCES [Locations](LocationsId),

    CONSTRAINT CK_Events_Status
        CHECK (EventsStatus IN (N'קרה', N'קורה', N'יקרה'))
);
GO

-- Ratings
CREATE TABLE Rating (
    RatingId      INT IDENTITY(1,1) PRIMARY KEY,
    UserId        INT           NOT NULL,
    EventsId      INT           NOT NULL,
    OverallScore  TINYINT       NOT NULL,
    NoiseScore    TINYINT       NOT NULL,
    TrafficScore  TINYINT       NOT NULL,
    SafetyScore   TINYINT       NOT NULL,
    Comment       NVARCHAR(1000) NULL,
    CreatedAt     DATETIME2     DEFAULT GETDATE(),

    CONSTRAINT FK_Rating_User
        FOREIGN KEY (UserId) REFERENCES [Users](UserId),

    CONSTRAINT FK_Rating_Events
        FOREIGN KEY (EventsId) REFERENCES [Events](EventsId),

    CONSTRAINT CK_Rating_OverallScore CHECK (OverallScore BETWEEN 1 AND 5)
);
GO

------------------------------------------------
-- 3. SEED DATA
------------------------------------------------

-- Users
INSERT INTO [Users] (FirstName, LastName, Email, [Password], DateOfBirth, PhoneNumber, [Address]) VALUES
(N'דנה',    N'כהן',     'dana.cohen@groundshare.com',    '1234', '1995-03-14', '050-1234567', N'עיר תל אביב'),
(N'יוסי',   N'לוי',     'yossi.levi@groundshare.com',    '1234', '1980-07-22', '052-7654321', N'מושב בני דרור'),
(N'נועה',   N'ישראלי',  'noa.israeli@groundshare.com',   '1234', '1998-11-05', '054-9988776', N'עיר נתניה'),
(N'אמיר',   N'חדד',     'amir.haddad@groundshare.com',   '1234', '1975-01-30', '052-3344556', N'קיבוץ יגור'),
(N'טלי',    N'מזרחי',   'tali.mizrahi@groundshare.com',  '1234', '1988-09-18', '050-2468135', N'עיר הרצליה'),
(N'רון',    N'ברק',     'ron.barak@groundshare.com',     '1234', '1965-05-09', '053-5557788', N'עיר חיפה'),
(N'שרית',   N'אברהמי',  'sarit.avrahami@groundshare.com','1234', '1972-12-02', '050-7891234', N'עיר כפר סבא'),
(N'גדי',    N'מלכה',    'gadi.malka@groundshare.com',    '1234', '1990-04-25', '052-1472583', N'עיר רמת גן'),
(N'לירון',  N'רז',      'liron.raz@groundshare.com',     '1234', '1993-06-11', '054-3216549', N'עיר גבעתיים'),
(N'מיכאל',  N'פרץ',     'michael.peretz@groundshare.com','1234', '1978-10-03', '052-3698521', N'קיבוץ מעגן מיכאל');
GO

-- Locations
INSERT INTO [Locations] (City, Street, HouseNumber, HouseType, Floor) VALUES
(N'תל אביב-יפו',     N'שדרות רוטשילד',  N'15', N'בניין',     3),
(N'נתניה',           N'פנחס לבון',      N'8',  N'בניין',     7),
(N'קיבוץ יגור',      N'הערבה',          N'4',  N'בית פרטי',  NULL),
(N'מושב בני דרור',   N'ההדרים',         N'12', N'בית פרטי',  NULL),
(N'הרצליה',          N'העצמאות',        N'20', N'בניין',     5),
(N'כפר סבא',         N'ויצמן',          N'45', N'בניין',     2),
(N'חיפה',            N'שדרות הנשיא',    N'99', N'בניין',     10),
(N'רמת גן',          N'בורוכוב',        N'3',  N'בית פרטי',  NULL),
(N'גבעתיים',         N'כצנלסון',        N'30', N'בניין',     4),
(N'קיבוץ מעגן מיכאל',N'הדקלים',        N'7',  N'בית פרטי',  NULL);
GO

-- Events
INSERT INTO [Events] (StartDateTime, EndDateTime, EventsType, PhotoUrl, [Description], Municipality, ResponsibleBody, EventsStatus, LocationsId) VALUES
('2025-01-10T08:00:00', '2025-01-15T18:00:00', N'עבודת תשתית', N'images/event1.jpg', N'שדרוג תשתיות מים וביוב ברחוב, כולל חפירות מקומיות וחסימות חלקיות של נתיבים.', N'עיריית תל אביב-יפו', N'עיריית תל אביב-יפו', N'קרה', 1),
('2025-11-20T22:00:00', NULL,                  N'חסימת כביש',  N'images/event2.jpg', N'חסימת כביש לילית לצורך עבודות סלילה. אין צפי סיום מדויק, צפויים שינויים.', N'עיריית נתניה', N'עיריית נתניה', N'קורה', 2),
('2026-03-01T07:00:00', '2026-09-01T17:00:00', N'פינוי בינוי', N'images/event3.jpg', N'תחילת פרויקט פינוי בינוי בבניינים ישנים ושיקום תשתיות סביבתיות.', N'מועצה אזורית זבולון', N'הרשות הממשלתית להתחדשות עירונית', N'יקרה', 3),
('2024-12-05T09:00:00', '2024-12-07T17:00:00', N'תמ"א 38',      N'images/event4.jpg', N'עבודות חיזוק בניין קיים במסגרת תמ"א 38, כולל שימוש במנופים.', N'מועצה אזורית לב השרון', N'קבלן פרטי - בניין הדרים', N'קרה', 4),
('2025-07-01T06:00:00', NULL,                  N'הפסקת חשמל',  N'images/event5.jpg', N'הפסקת חשמל מתוכננת לצורך שדרוג תחנת טרנספורמציה.', N'עיריית הרצליה', N'חברת החשמל', N'קורה', 5),
('2025-02-15T10:00:00', '2025-02-16T20:00:00', N'עבודת תשתית', N'images/event6.jpg', N'שיקום מדרכות והרחבת נתיב תחבורה ציבורית לאורך הרחוב.', N'עיריית כפר סבא', N'עיריית כפר סבא - אגף הנדסה', N'קרה', 6),
('2025-10-01T08:00:00', NULL,                  N'חסימת כביש',  N'images/event7.jpg', N'חסימת נתיב נסיעה לצורך התקנת רמזור חדש ומעברי חציה מוארים.', N'עיריית חיפה', N'עיריית חיפה - אגף תחבורה', N'קורה', 7),
('2024-08-01T00:00:00', '2024-08-01T06:00:00', N'הפסקת חשמל',  N'images/event8.jpg', N'הפסקת חשמל נקודתית בלילה לצורך טיפול בתקלה דחופה.', N'עיריית רמת גן', N'חברת החשמל', N'קרה', 8),
('2025-05-20T09:30:00', '2025-05-22T18:00:00', N'עבודת תשתית', N'images/event9.jpg', N'חפירות לצורך הטמנת כבלי תקשורת תת-קרקעיים ושיפור תשתית אינטרנט.', N'עיריית גבעתיים', N'חברת תקשורת מקומית', N'קרה', 9),
('2026-01-10T08:00:00', NULL,                  N'פינוי בינוי', N'images/event10.jpg', N'תחילת פרויקט פינוי בינוי כולל הריסת מבנים ישנים ובניית מתחם מגורים חדש.', N'מועצה אזורית חוף הכרמל', N'הרשות הממשלתית להתחדשות עירונית', N'יקרה', 10);
GO

-- Update Images to Placeholder URLs
UPDATE [Events] SET PhotoUrl = 'https://placehold.co/600x400/orange/white?text=Infrastructure' WHERE EventsId IN (1, 6, 9);
UPDATE [Events] SET PhotoUrl = 'https://placehold.co/600x400/orange/white?text=RoadBlock' WHERE EventsId IN (2, 7);
UPDATE [Events] SET PhotoUrl = 'https://placehold.co/600x400/orange/white?text=Construction' WHERE EventsId IN (3, 10);
UPDATE [Events] SET PhotoUrl = 'https://placehold.co/600x400/orange/white?text=TAMA38' WHERE EventsId = 4;
UPDATE [Events] SET PhotoUrl = 'https://placehold.co/600x400/orange/white?text=PowerOutage' WHERE EventsId IN (5, 8);
GO

-- Ratings
INSERT INTO Rating (UserId, EventsId, OverallScore, NoiseScore, TrafficScore, SafetyScore, Comment, CreatedAt) VALUES
(1, 1, 4, 3, 4, 5, N'העבודות היו קצת רועשות, אבל הודיעו מראש והכול התנהל בצורה מסודרת.', '2025-01-16T10:00:00'),
(2, 2, 2, 2, 1, 4, N'החסימה בלילה יצרה פקקים גם בבוקר, חבל שלא נפתח נתיב חלופי.', '2025-11-22T08:30:00'),
(3, 2, 3, 3, 2, 4, N'מובן שיש צורך בעבודות, אבל התחושות בפקקים לא נעימות.', '2025-11-23T19:15:00'),
(4, 3, 5, 3, 4, 5, N'פרויקט חשוב לשכונה, מקווה שיושלם בזמן בלי עיכובים מיותרים.', '2026-03-10T12:00:00'),
(5, 4, 4, 3, 4, 4, N'קצת רעש במהלך היום, אבל שיפור הבניין מורגש ומוסיף ביטחון.', '2024-12-08T09:45:00'),
(6, 5, 2, 1, 3, 3, N'הפסקת החשמל המתוכננת האריכה מעבר לצפי, והייתה פגיעה במקררים ובעסקים.', '2025-07-10T14:20:00'),
(7, 6, 5, 4, 5, 5, N'עבודות מתוכננות היטב, שיפור משמעותי בגישה לתחבורה הציבורית.', '2025-02-17T11:10:00'),
(8, 7, 3, 3, 2, 4, N'הרעיון טוב, אבל החסימה מושכת יותר מדי זמן. כדאי לעדכן את התושבים בתדירות גבוהה יותר.', '2025-10-10T18:05:00'),
(9, 8, 4, 4, 4, 4, N'הפסקת חשמל קצרה בלילה, כמעט ולא הרגשנו. טיפול מהיר יחסית.', '2024-08-02T07:30:00'),
(10, 9, 5, 4, 5, 5, N'סוף סוף אינטרנט יציב ומהיר ברחוב, לגמרי שווה את אי הנוחות הזמנית.', '2025-05-25T16:40:00');
GO

------------------------------------------------
-- 4. STORED PROCEDURES
------------------------------------------------

-- USERS: Login
CREATE PROCEDURE spLoginUser
    @Email NVARCHAR(255),
    @Password NVARCHAR(50)
AS
BEGIN
    SELECT UserId, FirstName, LastName, Email, IsAdmin
    FROM [Users]
    WHERE Email = @Email AND [Password] = @Password;
END
GO

-- USERS: Register
CREATE PROCEDURE spRegisterUser
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(255),
    @Password NVARCHAR(50),
    @DateOfBirth DATE,
    @PhoneNumber NVARCHAR(20),
    @Address NVARCHAR(255)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM [Users] WHERE Email = @Email)
    BEGIN
        SELECT -1; 
        RETURN;
    END

    INSERT INTO [Users] (FirstName, LastName, Email, [Password], DateOfBirth, PhoneNumber, [Address])
    VALUES (@FirstName, @LastName, @Email, @Password, @DateOfBirth, @PhoneNumber, @Address);
    
    SELECT SCOPE_IDENTITY();
END
GO

-- EVENTS: Get Single Event
CREATE PROCEDURE spGetEventById
    @EventsId INT
AS
BEGIN
    SELECT * FROM [Events] E
    JOIN [Locations] L ON E.LocationsId = L.LocationsId
    WHERE E.EventsId = @EventsId;
END
GO

-- EVENTS: Create
CREATE PROCEDURE spCreateEvent
    @StartDateTime DATETIME2,
    @EndDateTime DATETIME2 = NULL,
    @EventsType NVARCHAR(100),
    @PhotoUrl NVARCHAR(500) = NULL,
    @Description NVARCHAR(1000),
    @Municipality NVARCHAR(100),
    @ResponsibleBody NVARCHAR(150),
    @EventsStatus NVARCHAR(20),
    @LocationsId INT
AS
BEGIN
    INSERT INTO [Events] (StartDateTime, EndDateTime, EventsType, PhotoUrl, [Description], Municipality, ResponsibleBody, EventsStatus, LocationsId)
    VALUES (@StartDateTime, @EndDateTime, @EventsType, @PhotoUrl, @Description, @Municipality, @ResponsibleBody, @EventsStatus, @LocationsId);
    
    SELECT SCOPE_IDENTITY();
END
GO

-- EVENTS: Delete
CREATE PROCEDURE spDeleteEvent
    @EventsId INT
AS
BEGIN
    DELETE FROM Rating WHERE EventsId = @EventsId;
    DELETE FROM [Events] WHERE EventsId = @EventsId;
END
GO

-- LOCATIONS: Add
CREATE PROCEDURE spAddLocation
    @City NVARCHAR(100),
    @Street NVARCHAR(100),
    @HouseNumber NVARCHAR(10),
    @HouseType NVARCHAR(50),
    @Floor INT = NULL
AS
BEGIN
    INSERT INTO [Locations] (City, Street, HouseNumber, HouseType, Floor)
    VALUES (@City, @Street, @HouseNumber, @HouseType, @Floor);
    SELECT SCOPE_IDENTITY();
END
GO

-- LOCATIONS: Get All
CREATE PROCEDURE spGetAllLocations
AS
BEGIN
    SELECT * FROM [Locations];
END
GO

-- RATINGS: Add
CREATE PROCEDURE spAddRating
    @UserId INT,
    @EventsId INT,
    @OverallScore TINYINT,
    @NoiseScore TINYINT,
    @TrafficScore TINYINT,
    @SafetyScore TINYINT,
    @Comment NVARCHAR(1000) = NULL
AS
BEGIN
    INSERT INTO Rating (UserId, EventsId, OverallScore, NoiseScore, TrafficScore, SafetyScore, Comment, CreatedAt)
    VALUES (@UserId, @EventsId, @OverallScore, @NoiseScore, @TrafficScore, @SafetyScore, @Comment, GETDATE());
    
    SELECT SCOPE_IDENTITY();
END
GO

-- RATINGS: Get By Event (Reviews Modal)
CREATE PROCEDURE spGetRatingsByEvent
    @EventsId INT
AS
BEGIN
    SELECT * FROM Rating
    WHERE EventsId = @EventsId
    ORDER BY CreatedAt DESC;
END
GO

-- EVENTS: Get All (Main Feed - with Ratings & Address)
CREATE PROCEDURE spGetAllEvents
AS
BEGIN
    SELECT 
        E.EventsId, 
        E.StartDateTime, 
        E.EndDateTime, 
        E.EventsType, 
        E.PhotoUrl, 
        E.[Description], 
        E.EventsStatus,
        E.Municipality,
        E.ResponsibleBody,
        L.City, 
        L.Street,
        L.HouseNumber,
        -- Calculate Average Rating (default to 0 if no ratings)
        ISNULL((SELECT AVG(CAST(OverallScore AS FLOAT)) FROM Rating R WHERE R.EventsId = E.EventsId), 0) AS AvgRating,
        -- Count how many ratings exist
        (SELECT COUNT(*) FROM Rating R WHERE R.EventsId = E.EventsId) AS RatingCount
    FROM [Events] E
    JOIN [Locations] L ON E.LocationsId = L.LocationsId
    ORDER BY E.StartDateTime DESC;
END
GO

-- Ad-Hoc: Get count of events per type
CREATE PROCEDURE spGetEventTypeStats
AS
BEGIN
    SELECT EventsType, COUNT(*) as Total
    FROM Events
    GROUP BY EventsType
END
GO


-- Event Status Update
CREATE PROCEDURE spUpdateEventStatus
    @EventsId INT,
    @NewStatus NVARCHAR(20)
AS
BEGIN
    UPDATE Events
    SET EventsStatus = @NewStatus
    WHERE EventsId = @EventsId;
END
GO