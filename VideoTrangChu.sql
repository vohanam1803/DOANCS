USE AMNHAC
GO
CREATE TABLE VideoTrangChu(
	id NVARCHAR(255) NOT NULL,
    title NVARCHAR(255) NOT NULL,
    author NVARCHAR(150),
	Mota NVARCHAR(255),
);
SELECT * FROM VideoTrangChu