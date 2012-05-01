-- update database FROM version 200104 to 200105
BEGIN TRANSACTION  --( will be rolled back if wrong version#

-------------------------------------------------------------------------------
-- Deleted Stuff
-------------------------------------------------------------------------------

--( fnSplitCommaDelimIdStr is being replaced by fnGetIdsFromString

DROP FUNCTION fnSplitCommaDelimIdStr
GO

-------------------------------------------------------------------------------
-- New Stuff
-------------------------------------------------------------------------------

IF OBJECT_ID('fnGetIdsFromString') IS NOT NULL BEGIN
	PRINT 'removing function fnGetIdsFromString'
	DROP FUNCTION fnGetIdsFromString
END
GO
PRINT 'creating function fnGetIdsFromString'
GO

CREATE FUNCTION fnGetIdsFromString (@ntIds NTEXT, @hXmlIds INT)
RETURNS @tabIds TABLE (ID INT, ClassName NVARCHAR(100))
AS
BEGIN
	DECLARE
		@nId INT,
		@nEnd INT,
		@nStart INT

	--( Load from a comma delimited list.

	IF SUBSTRING(@ntIds, 1, 1) != '<' BEGIN
		SET @nStart = 1
		SET @nEnd = CHARINDEX(',', @ntIds, @nStart)

		WHILE @nEnd > 1 BEGIN
			SET @nId = SUBSTRING(@ntIds, @nStart, @nEnd - @nStart)

			INSERT INTO @tabIds
			SELECT @nId, c.Name
			FROM CmObject o
			JOIN Class$ c ON c.ID = o.Class$
			WHERE o.ID = @nId

			SET @nStart = @nEnd + 1
			SET @nEnd = CHARINDEX(',', @ntIds, @nStart)
		END

		--( last one.
		SET @nId = SUBSTRING(@ntIds, @nStart, DATALENGTH(@ntIds) - @nStart)

		INSERT INTO @tabIds
		SELECT @nId, c.Name
		FROM CmObject o
		JOIN Class$ c ON c.ID = o.Class$
		WHERE o.ID = @nId
	END

	--( Load from an XML string
	ELSE BEGIN
		--( In certain conditions, a function cannot call
		--( sp_xml_preparedocument. You must set it up first
		--( in the calling program:
		--(
		--( EXECUTE sp_xml_preparedocument @hXmlIds OUTPUT, @ntIds

		INSERT INTO @tabIds
		SELECT i.ID, c.Name
		FROM OPENXML(@hXmlIds, '/root/Obj') WITH (ID INT) i
		JOIN CmObject o ON o.ID = i.ID
		JOIN Class$ c ON c.ID = o.CLASS$

		--( In certain conditions, a function cannot call
		--( sp_xml_removedocument. You must set it up first in
		--( the calling program:
		--(
		--( EXECUTE sp_xml_removedocument @hXmlIds
	END
	RETURN

Fail:
	DELETE FROM @tabIds
	INSERT INTO @tabIds VALUES (-1, NULL)
	RETURN
END
GO

-------------------------------------------------------------------------------

IF OBJECT_ID('CreateDeleteObj') IS NOT NULL BEGIN
	PRINT 'removing procedure CreateDeleteObj'
	DROP PROC CreateDeleteObj
END
GO
PRINT 'creating procedure CreateDeleteObj'
GO

CREATE PROCEDURE CreateDeleteObj
	@nClassId INT
AS
	DECLARE
		@nvcObjClassName NVARCHAR(100),
		@nvcClassName NVARCHAR(100),
		@nFieldId INT,
		@nvcFieldName NVARCHAR(100),
		@nvcProcName NVARCHAR(120),  --( max possible size + a couple spare
		@nvcQuery1 VARCHAR(4000), --( 4000's not big enough; need more than 1 string
		@nvcQuery2 VARCHAR(4000),
		@nvcQuery3 VARCHAR(4000),
		@nvcQuery4 VARCHAR(4000),
		@fBuildProc BIT, --( Currently all tables are getting one
		@nvcDropQuery NVARCHAR(140),
		@nvcObjName NVARCHAR(100),
		@nOwnedClassId INT,
		@nDebug TINYINT

	SET @nDebug = 0

	SELECT @nvcObjClassName = c.Name FROM Class$ c WHERE c.Id = @nClassId

	SET @fBuildProc = 0
	SET @nvcProcName = N'TR_' + @nvcObjClassName + N'_ObjDel_Del'
	SET @nvcQuery1 = ''
	SET @nvcQuery2 = ''
	SET @nvcQuery3 = ''
	SET @nvcQuery4 = ''

	--( The initial part of the CREATE TRIGGER command
	IF OBJECT_ID(@nvcProcName) IS NULL
		SET @nvcQuery1 = N'CREATE'
	ELSE
		SET @nvcQuery1 = N'ALTER'

	--( This assumes only one ID (row) in deleted

	SET @nvcQuery1 = @nvcQuery1 +
		N' TRIGGER ' + @nvcProcName + N' ON ' + @nvcObjClassName + CHAR(13) +
		N'INSTEAD OF DELETE ' + CHAR(13) +
		N'AS ' + CHAR(13)
	IF @nDebug = 1
		SET @nvcQuery1 = @nvcQuery1 +
			CHAR(9) + N'PRINT ''TRIGGER ' + @nvcProcName +
				N' ON ' + @nvcObjClassName + N' INSTEAD OF DELETE ''' + CHAR(13) +
			CHAR(9) + CHAR(13)
	SET @nvcQuery1 = @nvcQuery1 +
		CHAR(9) + N'/* == This trigger generated by CreateDeleteObj == */ ' + CHAR(13) +
		CHAR(9) + CHAR(13) +
		CHAR(9) + N'DECLARE @nObjId INT ' + CHAR(13) +
		CHAR(9) + N'SELECT @nObjId = d.Id FROM deleted d' + CHAR(13) +
		CHAR(9) + CHAR(13)

	--==( Delete references *to* this object )==--

	--( atomic references to this object

	SELECT TOP 1 @nFieldId = f.Id, @nvcClassName = c.Name, @nvcFieldName = f.Name
	FROM Field$ f
	JOIN Class$ c ON c.Id = f.Class
	WHERE f.DstCls = @nClassId AND f.Type = 24
	ORDER BY f.Id

	IF @@ROWCOUNT != 0 BEGIN
		SET @fBuildProc = 1
		SET @nvcQuery1 = @nvcQuery1 + CHAR(9) +
			'/* Delete atomic references *to* this object */ ' + CHAR(13) +
			CHAR(9) + CHAR(13)
	END
	WHILE @@ROWCOUNT != 0 BEGIN
		SET @nvcQuery1 = @nvcQuery1 +
			CHAR(9) + N'UPDATE ' + @nvcClassName + N' WITH (SERIALIZABLE) ' + CHAR(13) +
			CHAR(9) + N'SET "' + @nvcFieldName + N'" = NULL ' + CHAR(13) +
			CHAR(9) + N'WHERE "' + @nvcFieldName + N'" = @nObjId ' + CHAR(13) +
			CHAR(9) + CHAR(13)

		SELECT TOP 1 @nFieldId = f.Id, @nvcClassName = c.Name, @nvcFieldName = f.Name
		FROM Field$ f
		JOIN Class$ c ON c.Id = f.Class
		WHERE f.Id > @nFieldId AND f.DstCls = @nClassId AND f.Type = 24
		ORDER BY f.Id
	END

	--( collection and sequence refences

	SELECT TOP 1 @nFieldId = f.Id, @nvcClassName = c.Name, @nvcFieldName = f.Name
	FROM Field$ f
	JOIN Class$ c ON c.Id = f.Class
	WHERE f.DstCls = @nClassId AND f.Type IN (26, 28)
	ORDER BY f.Id

	IF @@ROWCOUNT != 0 BEGIN
		SET @fBuildProc = 1
		SET @nvcQuery2 = @nvcQuery2 + CHAR(9) +
			'/* Delete collection and sequence references *to* this object */ ' + CHAR(13) +
			CHAR(9) + CHAR(13)
	END
	WHILE @@ROWCOUNT != 0 BEGIN
		SET @nvcQuery2 = @nvcQuery2 +
			CHAR(9) + N'DELETE ' + @nvcClassName + N'_' + @nvcFieldName + N' WITH (SERIALIZABLE) ' + CHAR(13) +
			CHAR(9) + N'WHERE "Dst" = @nObjId ' + CHAR(13) +
			CHAR(9) + CHAR(13)

		SELECT TOP 1 @nFieldId = f.Id, @nvcClassName = c.Name, @nvcFieldName = f.Name
		FROM Field$ f
		JOIN Class$ c ON c.Id = f.Class
		WHERE f.Id > @nFieldId AND f.DstCls = @nClassId AND f.Type IN (26, 28)
		ORDER BY f.Id
	END

	--==( Delete references *of* this object )==--

	--( Atomic references will get wiped out autmatically when this record
	--( goes away.

	--( Collection and Sequence refences

	SELECT TOP 1 @nFieldId = f.Id, @nvcClassName = c.Name, @nvcFieldName = f.Name
	FROM Field$ f
	JOIN Class$ c ON c.Id = f.Class
	WHERE f.Class = @nClassId AND f.Type IN (26, 28)
	ORDER BY f.Id

	IF @@ROWCOUNT != 0 BEGIN
		SET @fBuildProc = 1
		SET @nvcQuery3 = @nvcQuery3 + CHAR(9) +
			'/* Delete references *of* this object */ ' + CHAR(13) +
			CHAR(9) + CHAR(13)
	END
	WHILE @@ROWCOUNT != 0 BEGIN
		SET @nvcQuery3 = @nvcQuery3 +
			CHAR(9) + N'DELETE ' + @nvcClassName + N'_' + @nvcFieldName + N' WITH (SERIALIZABLE) ' + CHAR(13) +
			CHAR(9) + N'WHERE "Src" = @nObjId ' + CHAR(13) +
			CHAR(9) + CHAR(13)

		SELECT TOP 1 @nFieldId = f.Id, @nvcClassName = c.Name, @nvcFieldName = f.Name
		FROM Field$ f
		JOIN Class$ c ON c.Id = f.Class
		WHERE f.Id > @nFieldId AND f.Class = @nClassId AND f.Type IN (26, 28)
		ORDER BY f.Id
	END

	--==( Delete strings of this object )==--

	SET @nvcQuery4 = @nvcQuery4 +
		CHAR(9) + N'/* Delete any strings of this object */ ' + CHAR(13) +
		CHAR(9) + CHAR(13)

	--( If any MultiStr$ properties, create delete code.
	SELECT TOP 1 @nFieldId = Id FROM Field$ WHERE Class = @nClassId AND Type = 14
	IF @@ROWCOUNT != 0 BEGIN
		SET @fBuildProc = 1
		SET @nvcQuery4 = @nvcQuery4 +
			CHAR(9) + N'DELETE MultiStr$ WITH (SERIALIZABLE) ' + CHAR(13) +
			CHAR(9) + N'WHERE "Obj" = @nObjId ' + CHAR(13) +
			CHAR(9) + CHAR(13)
	END

	--( If any MultiTxt$ properties, create delete code.
	SELECT TOP 1 @nFieldId = f.ID, @nvcClassName = c.Name, @nvcFieldName = f.Name
	FROM Field$ f
	JOIN Class$ c ON c.ID = f.Class
	WHERE f.Class = @nClassId AND f.Type = 16
	ORDER BY f.Id

	WHILE @@ROWCOUNT != 0 BEGIN
		SET @fBuildProc = 1
		SET @nvcQuery4 = @nvcQuery4 +
			CHAR(9) + N'DELETE ' + @nvcClassName + N'_' + @nvcFieldName + N' WITH (SERIALIZABLE) ' + CHAR(13) +
			CHAR(9) + N'WHERE "Obj" = @nObjId ' + CHAR(13) +
			CHAR(9) + CHAR(13)

		SELECT TOP 1 @nFieldId = f.Id, @nvcClassName = c.Name, @nvcFieldName = f.Name
		FROM Field$ f
		JOIN Class$ c ON c.Id = f.Class
		WHERE f.ID > @nFieldId AND f.Class = @nClassId AND f.Type = 16
		ORDER BY f.Id
	END

	--( If any MultiBigStr$ properties, create delete code.

	SELECT TOP 1 @nFieldId = Id FROM Field$ WHERE Class = @nClassId AND Type = 18
	IF @@ROWCOUNT != 0 BEGIN
		SET @fBuildProc = 1
		SET @nvcQuery4 = @nvcQuery4 +
			CHAR(9) + N'DELETE MultiBigStr$ WITH (SERIALIZABLE) ' + CHAR(13) +
			CHAR(9) + N'WHERE "Obj" = @nObjId ' + CHAR(13) +
			CHAR(9) + CHAR(13)
	END

	--( If any MultiBigTxt$ properties, create delete code.

	SELECT TOP 1 @nFieldId = Id FROM Field$ WHERE Class = @nClassId AND Type = 20
	IF @@ROWCOUNT != 0 BEGIN
		SET @fBuildProc = 1
		SET @nvcQuery4 = @nvcQuery4 +
			CHAR(9) + N'DELETE MultiBigTxt$ WITH (SERIALIZABLE) ' + CHAR(13) +
			CHAR(9) + N'WHERE "Obj" = @nObjId ' + CHAR(13) +
			CHAR(9) + CHAR(13)
	END

	--==( Delete this row, since this is an DELETE INSTEAD OF trigger )==--

	SET @fBuildProc = 1
	SET @nvcQuery4 = @nvcQuery4 +
		CHAR(9) + N'/* Delete this row (for INSTEAD OF DELETE trigger) */ ' + CHAR(13) +
		CHAR(9) + CHAR(13)
	SET @nvcQuery4 = @nvcQuery4 +
		CHAR(9) + N'DELETE ' + @nvcObjClassName + N' WITH (SERIALIZABLE) ' + CHAR(13) +
		CHAR(9) + N'WHERE "Id" = @nObjId ' + CHAR(13) +
		CHAR(9) + CHAR(13)

	--==( Delete properties in parent class )==--

	--( This will delete properties *only* in the parent class,
	--( because the parent class will have the same call to
	--( delete properties in *its* parent class. The parent
	--( class has a depth of 1.

	SELECT @nvcClassName = c.Name
	FROM ClassPar$ cp
	JOIN Class$ c ON c.Id = cp.Dst
	WHERE cp.Src = @nClassId AND cp.Depth = 1

	IF @@ROWCOUNT = 1 BEGIN	--( should only be CmObject that misses
		SET @fBuildProc = 1
		SET @nvcQuery4 = @nvcQuery4 +
			CHAR(9) + N'/* Delete properties in parent class */' + CHAR(13) +
			CHAR(9) + CHAR(13)
		SET @nvcQuery4 = @nvcQuery4 +
			CHAR(9) + N'DELETE ' + @nvcClassName + N' WITH (SERIALIZABLE) ' + CHAR(13) +
			CHAR(9) + N'WHERE "Id" = @nObjId ' + CHAR(13) +
			CHAR(9) + CHAR(13)
	END

	--==( Create the new trigger )==--

	IF @fBuildProc = 1 BEGIN
		IF @nDebug = 1 BEGIN
			PRINT '---- query1 ----'
			PRINT @nvcQuery1
			PRINT CHAR(9) + '---- query2 ----'
			PRINT @nvcQuery2
			PRINT CHAR(9) + '---- query3 ----'
			PRINT @nvcQuery3
			PRINT CHAR(9) + '---- query4 ----'
			PRINT @nvcQuery4
		END

		EXECUTE (@nvcQuery1 + @nvcQuery2 + @nvcQuery3 + @nvcQuery4)
	END
GO

-------------------------------------------------------------------------------

IF OBJECT_ID('DeleteObjects') IS NOT NULL BEGIN
	PRINT 'removing procedure DeleteObjects'
	DROP PROC DeleteObjects
END
GO
PRINT 'creating procedure DeleteObjects'
GO

CREATE PROCEDURE DeleteObjects
	@ntIds NTEXT
AS
	DECLARE @tIds TABLE (ID INT, ClassName NVARCHAR(100), Level TINYINT)

	DECLARE
		@hXmlIds INT,
		@nRowCount INT,
		@nObjId INT,
		@nLevel INT,
		@nvcClassName NVARCHAR(100),
		@nvcSql NVARCHAR(1000),
		@nError INT

	SET @nError = 0

	--==( Load Ids )==--

	--( If we're working with an XML doc:
	IF SUBSTRING(@ntIds, 1, 1) = '<' BEGIN
		EXECUTE sp_xml_preparedocument @hXmlIds OUTPUT, @ntIds

		INSERT INTO @tIds
		SELECT f.ID, f.ClassName, 0
		FROM dbo.fnGetIdsFromString(@ntIds, @hXmlIds) AS f

		EXECUTE sp_xml_removedocument @hXmlIds
	END
	ELSE
		INSERT INTO @tIds
		SELECT f.ID, f.ClassName, 0
		FROM dbo.fnGetIdsFromString(@ntIds, NULL) AS f

	--( Now find owned objects

	SET @nLevel = 1

	INSERT INTO @tIds
	SELECT o.ID, c.Name, @nLevel
	FROM @tIds t
	JOIN CmObject o ON o.Owner$ = t.Id
	JOIN Class$ c ON c.ID = o.Class$

	SET @nRowCount = @@ROWCOUNT
	WHILE @nRowCount != 0 BEGIN
		SET @nLevel = @nLevel + 1

		INSERT INTO @tIds
		SELECT o.ID, c.Name, @nLevel
		FROM @tIds t
		JOIN CmObject o ON o.Owner$ = t.Id
		JOIN Class$ c ON c.ID = o.Class$
		WHERE t.Level = @nLevel - 1

		SET @nRowCount = @@ROWCOUNT
	END
	SET @nLevel = @nLevel - 1

	--==( Delete objects )==--

	--( We're going to start out at the leaves and work
	--( toward the trunk.

	WHILE @nLevel >= 0	BEGIN

		SELECT TOP 1 @nObjId = t.ID, @nvcClassName = t.ClassName
		FROM @tIds t
		WHERE t.Level = @nLevel
		ORDER BY t.Id

		SET @nRowCount = @@ROWCOUNT
		WHILE @nRowCount = 1 BEGIN
			SET @nvcSql = N'DELETE ' + @nvcClassName + N' WHERE Id = @nObjectID'
			EXEC sp_executesql @nvcSql, N'@nObjectID INT', @nObjectId = @nObjId
			SET @nError = @@ERROR
			IF @nError != 0
				GOTO Fail

			SELECT TOP 1 @nObjId = t.ID, @nvcClassName = t.ClassName
			FROM @tIds t
			WHERE t.Id > @nobjId AND t.Level = @nLevel
			ORDER BY t.ID

			SET @nRowCount = @@ROWCOUNT
		END

		SET @nLevel = @nLevel - 1
	END

	RETURN 0

Fail:
	RETURN @nError
GO

-------------------------------------------------------------------------------
-- Finish or roll back transaction as applicable
-------------------------------------------------------------------------------

declare @dbVersion int
SELECT @dbVersion = DbVer FROM Version$
if @dbVersion = 200104
begin
	UPDATE Version$ SET DbVer = 200105
	COMMIT TRANSACTION
	print 'database updated to version 200105'
end
else
begin
	ROLLBACK TRANSACTION
	print 'Update aborted: this works only if DbVer = 200104 (DbVer = ' +
			convert(varchar, @dbVersion) + ')'
end
GO