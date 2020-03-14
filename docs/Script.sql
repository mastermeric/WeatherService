USE BigDataTeknolojiTEST -- Baska bir DB var ise o kullanilabilir.
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE name='weatherRecords')
BEGIN
	DROP TABLE weatherRecords	
END
GO

CREATE TABLE weatherRecords(
	 wrId int IDENTITY(1,1) NOT NULL PRIMARY KEY	
	,wrLocation nvarchar(64) NOT NULL DEFAULT('')		   --> nvarchar(64) cunku sehir isimleri Turkce karakter icerebilir..
	,wrDailyMinValue  decimal(5,2) NOT NULL DEFAULT(9999)  --> decimal(5,2) cunku max sicaklik 999.99 minimum -999.99 dusunuldu
	,wrDailyMaxValue decimal(5,2) NOT NULL DEFAULT(9999)  	
	,wrMinWeaklyValue decimal(5,2) NOT NULL DEFAULT(9999)  	
	,wrMaxWeaklyValue decimal(5,2) NOT NULL DEFAULT(9999) 	
	,wrInsertDate date NOT NULL DEFAULT('19000101')
	)
GO

IF (EXISTS(SELECT 1 FROM sys.indexes  WHERE name='weatherRecords_NCUI1'))
BEGIN	
	PRINT 'Index silindi OK'
	DROP INDEX weatherRecords_NCUI1 ON weatherRecords
END
GO

-- Performasn amacli Index: simdilik basit bir tablo. Client sorgu tiplerine gore indexleme detaylandirilabilir.
	CREATE UNIQUE INDEX weatherRecords_NCUI1 on weatherRecords(wrLocation,wrInsertDate) -- gunluk lokasyon bazli Kayitlar unique dusunuldu
	PRINT 'Index yaratýldý..OK'
GO

-- Ihtiyac halinde user/role yetkileri verilebilir :
--GRANT SELECT,UPDATE,DELETE,INSERT ON pskMakale to dba

--INSERT Dummy data
INSERT INTO weatherRecords(wrLocation,wrDailyMinValue,wrDailyMaxValue,wrMinWeaklyValue,wrMaxWeaklyValue,wrInsertDate) VALUES('adana' ,15.0,15.8,3.5,999.99,GETDATE())
INSERT INTO weatherRecords(wrLocation,wrDailyMinValue,wrDailyMaxValue,wrMinWeaklyValue,wrMaxWeaklyValue,wrInsertDate) VALUES('adana' ,15.0,15.8,3.5,999.99,'20200310')
INSERT INTO weatherRecords(wrLocation,wrDailyMinValue,wrDailyMaxValue,wrMinWeaklyValue,wrMaxWeaklyValue,wrInsertDate) VALUES('ankara',15.0,18.7,3.5,21.2,'20200310')
INSERT INTO weatherRecords(wrLocation,wrDailyMinValue,wrDailyMaxValue,wrMinWeaklyValue,wrMaxWeaklyValue,wrInsertDate) VALUES('ankara',15.0,18.7,3.5,21.2,'20200309')
INSERT INTO weatherRecords(wrLocation,wrDailyMinValue,wrDailyMaxValue,wrMinWeaklyValue,wrMaxWeaklyValue,wrInsertDate) VALUES('ankara',15.0,18.7,3.5,21.2,GETDATE())
INSERT INTO weatherRecords(wrLocation,wrDailyMinValue,wrDailyMaxValue,wrMinWeaklyValue,wrMaxWeaklyValue,wrInsertDate) VALUES('paris',-999.99,15.0,-5.3,-1.5,GETDATE())
INSERT INTO weatherRecords(wrLocation,wrDailyMinValue,wrDailyMaxValue,wrMinWeaklyValue,wrMaxWeaklyValue,wrInsertDate) VALUES('paris',-999.99,15.0,-5.3,-1.5,'20200310')


--Test sorgusu
SELECT wrLocation,wrDailyMinValue,wrDailyMaxValue,wrMinWeaklyValue,wrMaxWeaklyValue,wrInsertDate FROM weatherRecords
WHERE wrInsertDate = cast(GETDATE() AS DATE)