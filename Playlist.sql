USE AMNHAC
GO
CREATE TABLE Playlist(
	idmusic INT IDENTITY PRIMARY KEY,
    namemusic NVARCHAR(255) NOT NULL,
    singer NVARCHAR(50),
	hinhmusic NVARCHAR(255),
    link NVARCHAR(255),
);
SELECT * FROM Playlist