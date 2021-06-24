EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'APP_AUTHENTICATION'
GO
use [APP_AUTHENTICATION];
GO
use [master];
GO
USE [master]
GO
ALTER DATABASE [APP_AUTHENTICATION] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
GO
USE [master]
GO
/****** Object:  Database [APP_AUTHENTICATION]    Script Date: 11/08/2020 11:01:05 p. m. ******/
DROP DATABASE [APP_AUTHENTICATION]
GO
CREATE DATABASE APP_AUTHENTICATION
GO
USE APP_AUTHENTICATION
GO
CREATE TABLE USER_TYPES
(
 ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
 DESCRIPTION VARCHAR(100) NOT NULL
)
GO
INSERT INTO USER_TYPES VALUES('Regular user');
INSERT INTO USER_TYPES VALUES('Tester');
INSERT INTO USER_TYPES VALUES('Admin');
GO
CREATE TABLE SCHOOLS
( 
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
NAME VARCHAR(100) NOT NULL,
HOME_PAGE VARCHAR(100) NOT NULL,
IMG_PATH VARCHAR(100) NOT NULL
)
GO
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 1','https://www.saes.cecyt1.ipn.mx/','https://www.saes.cecyt1.ipn.mx/Images/logos/01.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 2','https://www.saes.cecyt2.ipn.mx/','https://www.saes.cecyt2.ipn.mx/Images/logos/02.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 3','https://www.saes.cecyt3.ipn.mx/','https://www.saes.cecyt3.ipn.mx/Images/logos/03.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 4','https://www.saes.cecyt4.ipn.mx/','https://www.saes.cecyt4.ipn.mx/Images/logos/04.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 5','https://www.saes.cecyt5.ipn.mx/','https://www.saes.cecyt5.ipn.mx/Images/logos/05.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 6','https://www.saes.cecyt6.ipn.mx/','https://www.saes.cecyt6.ipn.mx/Images/logos/06.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 7','https://www.saes.cecyt7.ipn.mx/','https://www.saes.cecyt7.ipn.mx/Images/logos/07.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 8','https://www.saes.cecyt8.ipn.mx/','https://www.saes.cecyt8.ipn.mx/Images/logos/08.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 9','https://www.saes.cecyt9.ipn.mx/','https://www.saes.cecyt9.ipn.mx/Images/logos/09.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 10','https://www.saes.cecyt10.ipn.mx/','https://www.saes.cecyt10.ipn.mx/Images/logos/10.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 11','https://www.saes.cecyt11.ipn.mx/','https://www.saes.cecyt11.ipn.mx/Images/logos/11.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 12','https://www.saes.cecyt12.ipn.mx/','https://www.saes.cecyt12.ipn.mx/Images/logos/12.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 13','https://www.saes.cecyt13.ipn.mx/','https://www.saes.cecyt13.ipn.mx/Images/logos/13.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 14','https://www.saes.cecyt14.ipn.mx/','https://www.saes.cecyt14.ipn.mx/Images/logos/14.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CECyT 15','https://www.saes.cecyt15.ipn.mx/','https://www.saes.cecyt15.ipn.mx/Images/logos/15.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CET 1','https://www.saes.cet1.ipn.mx/','https://www.saes.cet1.ipn.mx/Images/logos/17.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESIME CULHUACÁN','https://www.saes.esimecu.ipn.mx/','https://www.saes.esimecu.ipn.mx/Images/logos/35.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESIME AZCAPOTZALCO','https://www.saes.esimeazc.ipn.mx/','https://www.saes.esimeazc.ipn.mx/Images/logos/36.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESIME TICOMÁN','https://www.saes.esimetic.ipn.mx/','https://www.saes.esimetic.ipn.mx/Images/logos/37.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESIME ZACATENCO','https://www.saes.esimez.ipn.mx/','https://www.saes.esimez.ipn.mx/Images/logos/30.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESIA TECAMACHALCO','https://www.saes.esiatec.ipn.mx/','https://www.saes.esiatec.ipn.mx/Images/logos/38.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESIA TICOMÁN','https://www.saes.esiatic.ipn.mx/','');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESIA ZACATENCO','https://www.saes.esiaz.ipn.mx/','https://www.saes.esiaz.ipn.mx/Images/logos/31.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CICS MILPA ALTA','https://www.saes.cicsma.ipn.mx/','https://www.saes.cicsma.ipn.mx/Images/logos/61.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('CICS SANTO TOMAS','https://www.saes.cicsst.ipn.mx/','https://www.saes.cicsst.ipn.mx/Images/logos/65.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESCA SANTO TOMAS','https://www.saes.escasto.ipn.mx/','https://www.saes.escasto.ipn.mx/Images/logos/40.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESCA TEPEPAN','https://www.saes.escatep.ipn.mx/','https://www.saes.escatep.ipn.mx/Images/logos/43.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ENCB','https://www.saes.encb.ipn.mx/','');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ENMH','https://www.saes.enmh.ipn.mx/','https://www.saes.enmh.ipn.mx/Images/logos/52.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESEO','https://www.saes.eseo.ipn.mx/','https://www.saes.eseo.ipn.mx/Images/logos/53.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESM','https://www.saes.esm.ipn.mx/','https://www.saes.esm.ipn.mx/Images/logos/51.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESE','https://www.saes.ese.ipn.mx/','https://www.saes.ese.ipn.mx/Images/logos/41.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('EST','https://www.saes.est.ipn.mx/','https://www.saes.est.ipn.mx/Images/logos/42.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('UPIBI','https://www.saes.upibi.ipn.mx/','https://www.saes.upibi.ipn.mx/Images/logos/62.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('UPIICSA','https://www.saes.upiicsa.ipn.mx/','https://www.saes.upiicsa.ipn.mx/Images/logos/60.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('UPIITA','https://www.saes.upiita.ipn.mx/','https://www.saes.upiita.ipn.mx/Images/logos/64.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESCOM','https://www.saes.escom.ipn.mx/','https://www.saes.escom.ipn.mx/Images/logos/63.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESFM','https://www.saes.esfm.ipn.mx/','https://www.saes.esfm.ipn.mx/Images/logos/33.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESIQUE','https://www.saes.esiqie.ipn.mx/','https://www.saes.esiqie.ipn.mx/Images/logos/32.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ESIT','https://www.saes.esit.ipn.mx/','https://www.saes.esit.ipn.mx/Images/logos/34.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('UPIIG','https://www.saes.upiig.ipn.mx/','https://www.saes.upiig.ipn.mx/Images/logos/66.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('UPIIH','https://www.saes.upiih.ipn.mx/','https://www.saes.upiih.ipn.mx/Images/logos/68.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('UPIIZ','https://www.saes.upiiz.ipn.mx/','');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('ENBA','https://www.saes.enba.ipn.mx/','https://www.saes.enba.ipn.mx/Images/logos/44.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('UPIIC','https://www.saes.upiic.ipn.mx/','https://www.saes.upiic.ipn.mx/Images/logos/69.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('UPIIP','https://www.saes.upiip.ipn.mx/','https://www.saes.upiip.ipn.mx/Images/logos/70.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('UPIEM','https://www.saes.upiem.ipn.mx/','https://www.saes.upiem.ipn.mx/Images/logos/45.png');
INSERT INTO SCHOOLS (NAME,HOME_PAGE,IMG_PATH) VALUES ('UPIIT','https://www.saes.upiit.ipn.mx/','https://www.saes.upiit.ipn.mx/Images/logos/71.png');

GO
CREATE TABLE USERS
(
 ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
 GUID UNIQUEIDENTIFIER DEFAULT newid() UNIQUE NOT NULL,
 TYPE INT FOREIGN KEY REFERENCES USER_TYPES,
 NAME VARCHAR(100) NOT NULL,
 BOLETA VARCHAR(20) UNIQUE NOT NULL,
 MAIL VARCHAR(100)  UNIQUE NOT NULL,
 PASSWORD_PIN VARCHAR(50) NOT NULL,
 PASSWORD_SAES VARCHAR(100) NOT NULL, 
 STRIKES INT DEFAULT 0,
 BANNED BIT DEFAULT 0,
 DELETED BIT DEFAULT 0,
 VERIFIED_EMAIL BIT DEFAULT 0
)
GO
CREATE TABLE USERS_SCHOOLS
(
 ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
 GUID UNIQUEIDENTIFIER DEFAULT newid() UNIQUE NOT NULL,
 SCHOOL_ID INT FOREIGN KEY REFERENCES SCHOOLS,
 USER_ID INT FOREIGN KEY REFERENCES USERS NOT NULL,
 ENROLL_DATE DATETIME NOT NULL DEFAULT GETDATE()
)
GO
CREATE TABLE DEVICES
(
 ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
 GUID UNIQUEIDENTIFIER DEFAULT newid() UNIQUE NOT NULL,
 USER_ID INT FOREIGN KEY REFERENCES USERS NOT NULL,
 DEVICE_KEY  VARCHAR(100) UNIQUE NOT NULL,
 BRAND VARCHAR(100) NOT NULL DEFAULT 'GENERIC',
 PLATFORM VARCHAR(100) NOT NULL DEFAULT 'GENERIC',
 MODEL VARCHAR(100) NOT NULL DEFAULT 'GENERIC',
 NAME VARCHAR(100) NOT NULL DEFAULT 'GENERIC',
 LAST_TIME_SEEN DATETIME NOT NULL DEFAULT GETDATE(),
 ENABLED BIT DEFAULT 1,

)
GO
CREATE PROCEDURE SP_CHECK_IF_BANNED (@USER_ID INT,@IS_BANNED BIT OUTPUT)
AS
BEGIN
DECLARE @STRIKES INT=0;
SELECT @STRIKES=STRIKES FROM USERS WHERE ID=@USER_ID
IF @STRIKES>=3
BEGIN
	UPDATE USERS SET BANNED=1 WHERE ID=@USER_ID
	SELECT  @IS_BANNED=1;
END ELSE 
BEGIN
	SELECT @IS_BANNED=0;
END
END;
GO
ALTER PROCEDURE SP_LOGIN (
@MAIL VARCHAR(100),
@BOLETA VARCHAR(20),
@PASSWORD_PIN VARCHAR(50),
@SCHOOL_NAME VARCHAR(100)
)
AS
BEGIN
DECLARE @USER_ID INT=0;
IF @BOLETA IS NOT NULL 
BEGIN
	select @USER_ID=ID From USERS where BOLETA=@BOLETA;
	IF @USER_ID<=0
		BEGIN
			SELECT 'SHOULD_ENROLL',''
			RETURN
		END 
END ELSE 
BEGIN
	select @USER_ID=ID From USERS where MAIL=@MAIL COLLATE Latin1_General_CS_AS;
	IF @USER_ID<=0
		BEGIN
			SELECT 'KO','Usuario o contraseña incorrectos'
			RETURN
		END 
END
DECLARE @REAL_PASSWORD VARCHAR(50)
select @REAL_PASSWORD=PASSWORD_PIN From USERS where ID=@USER_ID 
IF @REAL_PASSWORD!=@PASSWORD_PIN COLLATE Latin1_General_CS_AS
BEGIN
	SELECT 'KO','Usuario o contraseña incorrectos'
	RETURN
END
----------------
DECLARE @SCHOOL_ID INT =0;
IF @SCHOOL_NAME IS NOT NULL
BEGIN
SELECT @SCHOOL_ID=ID FROM SCHOOLS WHERE NAME=@SCHOOL_NAME
END ELSE
BEGIN
SELECT TOP 1 @SCHOOL_ID=SCHOOL_ID FROM USERS_SCHOOLS WHERE USER_ID=@USER_ID ORDER BY ID DESC
END

IF @SCHOOL_ID<=0
BEGIN
	SELECT 'INVALID_REQUEST','La escuela seleccionada es invalida'
	RETURN
END 
IF NOT EXISTS(SELECT ID FROM USERS_SCHOOLS WHERE USER_ID=@USER_ID AND SCHOOL_ID=@SCHOOL_ID)
	BEGIN
		SELECT 'KO','No estas registrado en este plantel, registrate para poder acceder'
		RETURN
	END ELSE
	BEGIN
		DECLARE @IS_BANNED BIT=0;
		EXEC SP_CHECK_IF_BANNED @USER_ID,@IS_BANNED OUTPUT
		IF @IS_BANNED=1
		BEGIN
			SELECT 'KO','Su usuario ha sido bloqueado debido a el mal uso de la aplicación.\n Si cree que se trata de un error contactenos en: jgarciag1404@alumno.ipn.mx'
		END
		ELSE 
		BEGIN
			DECLARE @SCHOOL VARCHAR(MAX)
			SELECT @SCHOOL='{"HomePage":"'+HOME_PAGE+'","Name":"'+NAME+'","ImgPath":"'+IMG_PATH+'"}'
			FROM SCHOOLS WHERE ID=@SCHOOL_ID
			SELECT 'OK','Bienvenido','{"Boleta":"'+BOLETA+'","Password":"'+PASSWORD_SAES+'","School":'+@SCHOOL+' }'
			FROM USERS WHERE ID=@USER_ID
		END
	END
END
GO
CREATE PROCEDURE SP_SIGNUP(
@BOLETA VARCHAR(20),
@NAME  VARCHAR(100),
@MAIL VARCHAR(100),
@PASSWORD_PIN VARCHAR(50),
@PASSWORD_SAES VARCHAR(100), 
@SCHOOL_NAME VARCHAR(100),
@DEVICE_KEY VARCHAR(100),
@BRAND VARCHAR(100),
@PLATFORM VARCHAR(100),
@MODEL  VARCHAR(100),
@D_NAME  VARCHAR(100),
@TYPE INT=1
)
AS
BEGIN
	DECLARE @SCHOOL_ID INT =0;
	SELECT @SCHOOL_ID=ID FROM SCHOOLS WHERE NAME=@SCHOOL_NAME
	IF @SCHOOL_ID<=0
	BEGIN
		SELECT 'INVALID_REQUEST','La escuela seleccionada es invalida'
	END ELSE 
	BEGIN
		DECLARE @USER_ID INT=0
		SELECT @USER_ID= ID FROM USERS WHERE BOLETA=@BOLETA
		IF @USER_ID>0
		BEGIN
			IF EXISTS(SELECT ID FROM USERS_SCHOOLS WHERE USER_ID=@USER_ID)
			BEGIN
				SELECT 'INVALID_REQUEST','Este usuario ya esta registrado'
			END 
		END ELSE
		BEGIN
			IF EXISTS(SELECT ID FROM USERS WHERE MAIL=@MAIL)
			BEGIN
				SELECT 'INVALID_REQUEST','Ya existe un usuario asocidado a este correo electrónico'
			END ELSE
			BEGIN
			
			INSERT INTO USERS(TYPE,NAME,BOLETA,MAIL,PASSWORD_PIN,PASSWORD_SAES)
			VALUES (@TYPE,@NAME,@BOLETA,@MAIL,@PASSWORD_PIN,@PASSWORD_SAES);
			SELECT @USER_ID=SCOPE_IDENTITY();
			INSERT INTO USERS_SCHOOLS(SCHOOL_ID,USER_ID) VALUES (@SCHOOL_ID,@USER_ID)
			INSERT INTO DEVICES(USER_ID,DEVICE_KEY,BRAND,PLATFORM,MODEL,NAME) VALUES(@USER_ID,@DEVICE_KEY,@BRAND,@PLATFORM,@MODEL,@D_NAME)
			SELECT 'OK','Bienvenido :)'
			END
		END

	END
END;
GO