-- update database FROM version 200137 to 200138
BEGIN TRANSACTION  --( will be rolled back if wrong version#

-------------------------------------------------------------------------------
-- New function fnGetRefsToObj, generated by CreateGetRefsToObj. Implement in
-- all appropriate triggers.
-------------------------------------------------------------------------------

IF OBJECT_ID('CreateGetRefsToObj') IS NOT NULL BEGIN
	PRINT 'removing procedure CreateGetRefsToObj'
	DROP PROCEDURE CreateGetRefsToObj
END
GO
PRINT 'creating procedure CreateGetRefsToObj'
GO

CREATE PROCEDURE CreateGetRefsToObj
AS
	DECLARE
		@fDebug BIT,
		@nFieldId INT,
		@nDstCls INT,
		@nDstCls2 INT,
		@nvcClassName NVARCHAR(100),
		@nvcFieldName NVARCHAR(100),
		@nFieldClass INT,
		@nFieldType INT,
		@nvcProcName NVARCHAR(120),  --( max possible size + a couple spare
		@nvcQ VARCHAR(4000),
		@nvcQuery1 VARCHAR(4000), --( 4000's not big enough; need more than 1 string
		@nvcQuery2 VARCHAR(4000),
		@nvcQuery3 VARCHAR(4000),
		@nvcQuery4 VARCHAR(4000),
		@nvcQuery5 VARCHAR(4000),
		@nvcQuery6 VARCHAR(4000),
		@nvcQuery7 VARCHAR(4000),
		@nvcQuery8 VARCHAR(4000),
		@nvcQuery9 VARCHAR(4000),
		@nvcQuery10 VARCHAR(4000),
		@nvcQuery11 VARCHAR(4000),
		@nvcQuery12 VARCHAR(4000),
		@nvcQuery13 VARCHAR(4000),
		@nFetchStatus BIT,
		@fFirstIf BIT,
		@nError INT

	SET @fDebug = 0 --( 0 produces stored procedure, 1 produces print
	SET @nvcQ = ''
	SET @nvcQuery1 = ''
	SET @nvcQuery2 = ''
	SET @nvcQuery3 = ''
	SET @nvcQuery4 = ''
	SET @nvcQuery5 = ''
	SET @nvcQuery6 = ''
	SET @nvcQuery7 = ''
	SET @nvcQuery8 = ''
	SET @nvcQuery9 = ''
	SET @nvcQuery10 = ''
	SET @nvcQuery11 = ''
	SET @nvcQuery12 = ''
	SET @nvcQuery13 = ''
	set @nError = 0

	--( Loop for subclasses

	IF OBJECT_ID('fnGetRefsToObj') IS NULL
		SET @nvcQuery1 = N'CREATE FUNCTION fnGetRefsToObj (' + CHAR(13)
	ELSE
		SET @nvcQuery1 = N'ALTER FUNCTION fnGetRefsToObj (' + CHAR(13)

	SET @nvcQuery1 = @nvcQuery1 +
		CHAR(9) + N'@nObjId INT, ' + CHAR(13) +
		CHAR(9) + N'@nClassId INT = NULL) ' + CHAR(13) +
		N'RETURNS @tblR TABLE ( ' + CHAR(13) +
		CHAR(9) + N'ObjId INT, ' + CHAR(13) +
		CHAR(9) + N'ObjClass INT, ' + CHAR(13) +
		CHAR(9) + N'ClassLevel INT, ' + CHAR(13) +
		CHAR(9) + N'ObjLevel INT, ' + CHAR(13) +
		CHAR(9) + N'RefObjId INT, ' + CHAR(13) +
		CHAR(9) + N'RefObjClass INT, ' + CHAR(13) +
		CHAR(9) + N'RefObjField INT, ' + CHAR(13) +
		CHAR(9) + N'RefObjFieldOrder INT, ' + CHAR(13) +
		CHAR(9) + N'RefObjFieldType INT) ' + CHAR(13) +
		N'AS BEGIN ' + CHAR(13) +
		CHAR(13) +
		N'/* == This function generated by CreateGetRefsToObj == */ ' + CHAR(13) +
		CHAR(13) +
		N'DECLARE @nDst INT, @nFetchStatus INT, @nObjLevel INT, @nClassLevel INT;' + CHAR(13) +
		N'DECLARE @tblO TABLE (Id INT, ObjLevel INT, Class INT, ClassLevel INT)' + CHAR(13) +
		N'IF @nClassId IS NULL ' + CHAR(13) +
		CHAR(9) + 'SELECT @nClassId = Class$ FROM CmObject WHERE [ID] = @nObjId' + CHAR(13) +
		CHAR(13) +
		N'/* Get Owned objects */ ' + CHAR(13) +
		N'SET @nObjLevel = 1;'  + CHAR(13) +
		N'INSERT INTO @tblO (ID, ObjLevel, Class, ClassLevel)' + CHAR(13) +
		CHAR(9) + N'VALUES (@nObjId, @nObjLevel, @nClassId, 0)' + CHAR(13) +
		N'WHILE @@ROWCOUNT != 0 BEGIN' + CHAR(13) +
		CHAR(9) + N'SET @nObjLevel = @nObjLevel + 1;' + CHAR(13) +
		CHAR(9) + N'INSERT INTO @tblO' + CHAR(13) +
		CHAR(9) + CHAR(9) + N'SELECT co.Id, @nObjLevel, co.Class$, 0' +CHAR(13) +
		CHAR(9) + CHAR(9) + N'FROM @tblO t ' + CHAR(13) +
		CHAR(9) + CHAR(9) + N'JOIN CmObject co ON co.Owner$ = t.Id' + CHAR(13) +
		CHAR(9) + CHAR(9) + N'WHERE t.ObjLevel = @nObjLevel - 1;' + CHAR(13) +
		N'END;' + CHAR(13) +
		CHAR(13) +
		N'/* Get super classes of the objects */ ' + CHAR(13) +
		N'INSERT INTO @tblO' + CHAR(13) +
		N'SELECT o.Id, o.ObjLevel, cp.dst, cp.Depth' + CHAR(13) +
		N'FROM @tblO o' + CHAR(13) +
		N'JOIN ClassPar$ cp ON cp.Src = o.Class' + CHAR(13) +
		N'WHERE cp.Depth != 0' + CHAR(13) +
		CHAR(13) +
		N'/* Now get references to them */ ' + CHAR(13) +
		N'DECLARE curClassDepth CURSOR ' + CHAR(13) +
		CHAR(9) + N'FOR SELECT Id, Class, ClassLevel, ObjLevel - 1 FROM @tblO '  + CHAR(13) +
		N'OPEN curClassDepth;' + CHAR(13) +
		N'FETCH NEXT FROM curClassDepth INTO @nObjId, @nDst, @nClassLevel, @nObjLevel;' + CHAR(13) +
		N'SET @nFetchStatus = @@FETCH_STATUS;' + CHAR(13) +
		N'WHILE @nFetchStatus = 0 BEGIN ' + CHAR(13)

	DECLARE curRefs CURSOR LOCAL STATIC FORWARD_ONLY READ_ONLY FOR
		SELECT f.Id, f.Class, f.DstCls, c.Name AS ClassName, f.Name AS FieldName, f.Type
		FROM Field$ f
		JOIN Class$ c ON c.Id = f.Class
		WHERE f.Type IN (24, 26, 28)
		ORDER BY f.DstCls, f.Id

	SET @nDstCls2 = 987654321	--( bogus ID
	SET @fFirstIf = 1

	OPEN curRefs
	FETCH curRefs INTO @nFieldId, @nFieldClass, @nDstCls, @nvcClassName, @nvcFieldName, @nFieldType
	SET @nFetchStatus = @@FETCH_STATUS
	WHILE @nFetchStatus = 0 BEGIN
		--( Create an IF block for a particular class

		IF @nDstCls != @nDstCls2 BEGIN
			SET @nvcQ = CHAR(9)
			IF @fFirstIf = 0
				SET @nvcQ = @nvcQ + N'ELSE '
			SET @nvcQ = @nvcQ + N'IF @nDst = ' + CONVERT(NVARCHAR(10), @nDstCls) + N' BEGIN ' + CHAR(13)
			SET @nDstCls2 = @nDstCls
		END

		IF LEN(@nvcQuery1) < 3750
			SET @nvcQuery1 = @nvcQuery1 + @nvcQ
		ELSE IF LEN(@nvcQuery2) < 3750
			SET @nvcQuery2 = @nvcQuery2 + @nvcQ
		ELSE IF LEN(@nvcQuery3) < 3750
			SET @nvcQuery3 = @nvcQuery3 + @nvcQ
		ELSE IF LEN(@nvcQuery4) < 3750
			SET @nvcQuery4 = @nvcQuery4 + @nvcQ
		ELSE IF LEN(@nvcQuery5) < 3750
			SET @nvcQuery5 = @nvcQuery5 + @nvcQ
		ELSE IF LEN(@nvcQuery6) < 3750
			SET @nvcQuery6 = @nvcQuery6 + @nvcQ
		ELSE IF LEN(@nvcQuery7) < 3750
			SET @nvcQuery7 = @nvcQuery7 + @nvcQ
		ELSE IF LEN(@nvcQuery8) < 3750
			SET @nvcQuery8 = @nvcQuery8 + @nvcQ
		ELSE IF LEN(@nvcQuery9) < 3750
			SET @nvcQuery9 = @nvcQuery9 + @nvcQ
		ELSE IF LEN(@nvcQuery10) < 3750
			SET @nvcQuery10 = @nvcQuery10 + @nvcQ
		ELSE IF LEN(@nvcQuery11) < 3750
			SET @nvcQuery11 = @nvcQuery11 + @nvcQ
		ELSE IF LEN(@nvcQuery12) < 3750
			SET @nvcQuery12 = @nvcQuery12 + @nvcQ
		ELSE IF LEN(@nvcQuery13) < 3750
			SET @nvcQuery13 = @nvcQuery13 + @nvcQ

		--( Cycle through the classes that refer to this one.

		WHILE @nDstCls2 = @nDstCls AND @nFetchStatus = 0 BEGIN
			IF @nFieldType = 24
				SET @nvcQ =
					CHAR(9) + CHAR(9) + N'INSERT INTO @tblR ' + CHAR(13) +
					CHAR(9) + CHAR(9) + CHAR(9) + N'SELECT @nObjId, @nDst, @nClassLevel, @nObjLevel, r.[Id], '
						+ CONVERT(NVARCHAR(10), @nFieldClass) + N', ' +
						+ CONVERT(NVARCHAR(10), @nFieldId) +
						+ N', NULL, '
						+ CONVERT(NVARCHAR(10), @nFieldType) + CHAR(13) +
					N' FROM [' + @nvcClassName + N'] r ' + CHAR(13) +
					CHAR(9) + CHAR(9) + CHAR(9) + N'LEFT OUTER JOIN @tblO o ON o.[Id] = r.[Id] ' + CHAR(13) +
					CHAR(9) + CHAR(9) + CHAR(9) + N'WHERE r.[' + @nvcFieldName + N'] = @nObjId ' +
					N'AND o.[Id] IS NULL;' + CHAR(13)
			ELSE BEGIN
				SET @nvcQ =
					CHAR(9) + CHAR(9) + N'INSERT INTO @tblR ' + CHAR(13) +
					CHAR(9) + CHAR(9) + CHAR(9) + N'SELECT @nObjId, @nDst, @nClassLevel, @nObjLevel, r.Src, ' +
					+ CONVERT(NVARCHAR(10), @nFieldClass) + N', ' +
					+ CONVERT(NVARCHAR(10), @nFieldId)

				IF @nFieldType = 26
					SET @nvcQ = @nvcQ + N', NULL, '
				ELSE
					SET @nvcQ = @nvcQ + N', r.Ord, '

				SET @nvcQ = @nvcQ +
					CONVERT(NVARCHAR(10), @nFieldType) + CHAR(13) +
					' FROM ' + @nvcClassName + N'_' + @nvcFieldName + N' r ' + CHAR(13) +
					CHAR(9) + CHAR(9) + CHAR(9) + N'LEFT OUTER JOIN @tblO o ON o.Id = r.Src ' + CHAR(13) +
					CHAR(9) + CHAR(9) + CHAR(9) + N'WHERE r.Dst = @nObjId AND o.Id IS NULL;' + CHAR(13)
			END
			IF LEN(@nvcQuery1) < 3750
				SET @nvcQuery1 = @nvcQuery1 + @nvcQ
			ELSE IF LEN(@nvcQuery2) < 3750
				SET @nvcQuery2 = @nvcQuery2 + @nvcQ
			ELSE IF LEN(@nvcQuery3) < 3750
				SET @nvcQuery3 = @nvcQuery3 + @nvcQ
			ELSE IF LEN(@nvcQuery4) < 3750
				SET @nvcQuery4 = @nvcQuery4 + @nvcQ
			ELSE IF LEN(@nvcQuery5) < 3750
				SET @nvcQuery5 = @nvcQuery5 + @nvcQ
			ELSE IF LEN(@nvcQuery6) < 3750
				SET @nvcQuery6 = @nvcQuery6 + @nvcQ
			ELSE IF LEN(@nvcQuery7) < 3750
				SET @nvcQuery7 = @nvcQuery7 + @nvcQ
			ELSE IF LEN(@nvcQuery8) < 3750
				SET @nvcQuery8 = @nvcQuery8 + @nvcQ
			ELSE IF LEN(@nvcQuery9) < 3750
				SET @nvcQuery9 = @nvcQuery9 + @nvcQ
			ELSE IF LEN(@nvcQuery10) < 3750
				SET @nvcQuery10 = @nvcQuery10 + @nvcQ
			ELSE IF LEN(@nvcQuery11) < 3750
				SET @nvcQuery11 =@nvcQuery11 + @nvcQ
			ELSE IF LEN(@nvcQuery12) < 3750
				SET @nvcQuery12 = @nvcQuery12 + @nvcQ
			ELSE IF LEN(@nvcQuery13) < 3750
				SET @nvcQuery13 = @nvcQuery13 + @nvcQ

			FETCH curRefs INTO @nFieldId, @nFieldClass, @nDstCls, @nvcClassName, @nvcFieldName, @nFieldType
			SET @nFetchStatus = @@FETCH_STATUS
		END

		--( Close out the if block
		IF LEN(@nvcQuery1) < 3750
			SET @nvcQuery1 = @nvcQuery1 + CHAR(9) + N'END;' + CHAR(13)
		ELSE IF LEN(@nvcQuery2) < 3750
			SET @nvcQuery2 = @nvcQuery2 + CHAR(9) + N'END;' + CHAR(13)
		ELSE IF LEN(@nvcQuery3) < 3750
			SET @nvcQuery3 = @nvcQuery3 + CHAR(9) + N'END;' + CHAR(13)
		ELSE IF LEN(@nvcQuery4) < 3750
			SET @nvcQuery4 = @nvcQuery4 + CHAR(9) + N'END;' + CHAR(13)
		ELSE IF LEN(@nvcQuery5) < 3750
			SET @nvcQuery5 = @nvcQuery5 + CHAR(9) + N'END;' + CHAR(13)
		ELSE IF LEN(@nvcQuery6) < 3750
			SET @nvcQuery6 = @nvcQuery6 + CHAR(9) + N'END;' + CHAR(13)
		ELSE IF LEN(@nvcQuery7) < 3750
			SET @nvcQuery7 = @nvcQuery7 + CHAR(9) + N'END;' + CHAR(13)
		ELSE IF LEN(@nvcQuery8) < 3750
			SET @nvcQuery8 = @nvcQuery8 + CHAR(9) + N'END;' + CHAR(13)
		ELSE IF LEN(@nvcQuery9) < 3750
			SET @nvcQuery9 = @nvcQuery9 + CHAR(9) + N'END;' + CHAR(13)
		ELSE IF LEN(@nvcQuery10) < 3750
			SET @nvcQuery10 = @nvcQuery10 + CHAR(9) + N'END;' + CHAR(13)
		ELSE IF LEN(@nvcQuery11) < 3750
			SET @nvcQuery11 = @nvcQuery11 + CHAR(9) + N'END;' + CHAR(13)
		ELSE IF LEN(@nvcQuery12) < 3750
			SET @nvcQuery12 = @nvcQuery12 + CHAR(9) + N'END;' + CHAR(13)
		ELSE IF LEN(@nvcQuery13) < 3750
			SET @nvcQuery13 = @nvcQuery13 + CHAR(9) + N'END;' + CHAR(13)

	END --( @@FETCH_STATUS = 0
	CLOSE curRefs
	DEALLOCATE curRefs

	SET @nvcQ =
		CHAR(9) + N'FETCH NEXT FROM curClassDepth INTO @nObjId, @nDst, @nClassLevel, @nObjLevel; ' + CHAR(13) +
		CHAR(9) + N'SET @nFetchStatus = @@FETCH_STATUS; ' + CHAR(13) +
		N'END; ' + CHAR(13) +
		N'RETURN; ' + CHAR(13) +
		N'END ' + CHAR(13)

	IF LEN(@nvcQuery1) < 3750
		SET @nvcQuery1 = @nvcQuery1 + @nvcQ
	ELSE IF LEN(@nvcQuery2) < 3750
		SET @nvcQuery2 = @nvcQuery2 + @nvcQ
	ELSE IF LEN(@nvcQuery3) < 3750
		SET @nvcQuery3 = @nvcQuery3 + @nvcQ
	ELSE IF LEN(@nvcQuery4) < 3750
		SET @nvcQuery4 = @nvcQuery4 + @nvcQ
	ELSE IF LEN(@nvcQuery5) < 3750
		SET @nvcQuery5 = @nvcQuery5 + @nvcQ
	ELSE IF LEN(@nvcQuery6) < 3750
		SET @nvcQuery6 = @nvcQuery6 + @nvcQ
	ELSE IF LEN(@nvcQuery7) < 3750
		SET @nvcQuery7 = @nvcQuery7 + @nvcQ
	ELSE IF LEN(@nvcQuery8) < 3750
		SET @nvcQuery8 = @nvcQuery8 + @nvcQ
	ELSE IF LEN(@nvcQuery9) < 3750
		SET @nvcQuery9 = @nvcQuery9 + @nvcQ
	ELSE IF LEN(@nvcQuery10) < 3750
		SET @nvcQuery10 = @nvcQuery10 + @nvcQ
	ELSE IF LEN(@nvcQuery11) < 3750
		SET @nvcQuery11 = @nvcQuery11 + @nvcQ
	ELSE IF LEN(@nvcQuery12) < 3750
		SET @nvcQuery12 = @nvcQuery12 + @nvcQ
	ELSE IF LEN(@nvcQuery13) < 3750
		SET @nvcQuery13 = @nvcQuery13 + @nvcQ

	IF @fDebug = 0 BEGIN
		EXEC (
			@nvcQuery1 + N' ' + @nvcQuery2 + N' ' + @nvcQuery3 + N' ' +
			@nvcQuery4 + N' ' + @nvcQuery5 + N' ' + @nvcQuery6 + N' ' +
			@nvcQuery7 + N' ' + @nvcQuery8 + N' ' + @nvcQuery9 + N' ' +
			@nvcQuery10 + N' ' + @nvcQuery11 + N' ' + @nvcQuery12 + N' ' +
			@nvcQuery13)
		SET @nError = @@ERROR
	END
	ELSE BEGIN
		PRINT @nvcQuery1
		PRINT '--(Starting @nvcQuery2'
		PRINT @nvcQuery2
		PRINT '--(Starting @nvcQuery3'
		PRINT @nvcQuery3
		PRINT '--(Starting @nvcQuery4'
		PRINT @nvcQuery4
		PRINT '--(Starting @nvcQuery5'
		PRINT @nvcQuery5
		PRINT '--(Starting @nvcQuery6'
		PRINT @nvcQuery6
		PRINT '--(Starting @nvcQuery7'
		PRINT @nvcQuery7
		PRINT '--(Starting @nvcQuery8'
		PRINT @nvcQuery8
		PRINT '--(Starting @nvcQuery9'
		PRINT @nvcQuery9
		PRINT '--(Starting @nvcQuery10'
		PRINT @nvcQuery10
		PRINT '--(Starting @nvcQuery11'
		PRINT @nvcQuery11
		PRINT '--(Starting @nvcQuery12'
		PRINT @nvcQuery12
		PRINT '--(Starting @nvcQuery13'
		PRINT @nvcQuery13
	END

	RETURN @nError
GO


EXEC CreateGetRefsToObj
GO

-------------------------------------------------------------------------------

IF OBJECT_ID('TR_Class$_InsLast') IS NOT NULL BEGIN
	PRINT 'removing trigger TR_Class$_InsLast'
	DROP TRIGGER TR_Class$_InsLast
END
GO
PRINT 'creating trigger TR_Class$_InsLast'
GO

CREATE TRIGGER TR_Class$_InsLast ON Class$ FOR INSERT
AS
	DECLARE
		@nErr INT,
		@nClassid INT,
		@nAbstract BIT

	SELECT @nClassId = Id, @nAbstract = Abstract FROM inserted

	--( Build the CreateObject_ stored procedure
	IF @nAbstract = 0 BEGIN
		EXEC @nErr = DefineCreateProc$ @nClassId
		IF @nErr <> 0 GOTO LFail
	END

	--( Build the delete trigger
	EXEC @nErr = CreateDeleteObj @nClassId
	IF @nErr <> 0 GOTO LFail

	--( Rebuild the stored function fnGetRefsToObj
	EXEC @nErr = CreateGetRefsToObj
	IF @nErr <> 0 GOTO LFail

	RETURN

LFail:
	ROLLBACK TRANSACTION
	RETURN
GO


EXEC sp_settriggerorder 'TR_Class$_InsLast', 'last', 'INSERT'
GO

-------------------------------------------------------------------------------

IF OBJECT_ID('TR_Field$_UpdateModel_InsLast') IS NOT NULL BEGIN
	PRINT 'removing trigger TR_Field$_UpdateModel_InsLast'
	DROP TRIGGER TR_Field$_UpdateModel_InsLast
END
GO
PRINT 'creating trigger TR_Field$_UpdateModel_InsLast'
GO

CREATE TRIGGER TR_Field$_UpdateModel_InsLast ON Field$ FOR INSERT
AS
	DECLARE
		@nErr INT,
		@nClassid INT,
		@nAbstract BIT,
		@nLoopLevel TINYINT,
		@fExit BIT

	DECLARE @tblSubclasses TABLE (ClassId INT, Abstract BIT, ClassLevel TINYINT)

	SELECT @nClassId = Class FROM inserted
	SET @nLoopLevel = 1

	--==( Outer loop: all the classes for the level )==--

	--( This insert is necessary for any subclasses. It also
	--( gets Class$.Abstract for updating the CreateObject_*
	--( stored procedure.

	INSERT INTO @tblSubclasses
	SELECT @nClassId, c.Abstract, @nLoopLevel
	FROM Class$ c
	WHERE c.Id = @nClassId

	--( Rebuild the delete trigger

	EXEC @nErr = CreateDeleteObj @nClassId
	IF @nErr <> 0 GOTO LFail

	--( Rebuild CreateObject_*

	SELECT @nAbstract = Abstract FROM @tblSubClasses
	IF @nAbstract != 1 BEGIN
		EXEC @nErr = DefineCreateProc$ @nClassId
		IF @nErr <> 0 GOTO LFail
	END

	SET @fExit = 0
	WHILE @fExit = 0 BEGIN

		--( Inner loop: update all classes subclassed from the previous
		--( set of classes.

		SELECT TOP 1 @nClassId = ClassId, @nAbstract = Abstract
		FROM @tblSubclasses
		WHERE ClassLevel = @nLoopLevel
		ORDER BY ClassId

		WHILE @@ROWCOUNT > 0 BEGIN

			--( Update the view

			EXEC @nErr = UpdateClassView$ @nClassId, 1
			IF @nErr <> 0 GOTO LFail

			--( Get next class

			SELECT TOP 1 @nClassId = ClassId, @nAbstract = Abstract
			FROM @tblSubclasses
			WHERE ClassLevel = @nLoopLevel AND ClassId > @nClassId
			ORDER BY ClassId
		END

		--( Load outer loop with next level
		SET @nLoopLevel = @nLoopLevel + 1

		INSERT INTO @tblSubclasses
		SELECT c.Id, c.Abstract, @nLoopLevel
		FROM @tblSubClasses sc
		JOIN Class$ c ON c.Base = sc.ClassId
		WHERE sc.ClassLevel = @nLoopLevel - 1

		IF @@ROWCOUNT = 0
			SET @fExit = 1
	END

	--( Rebuild the stored function fnGetRefsToObj
	EXEC @nErr = CreateGetRefsToObj
	IF @nErr <> 0 GOTO LFail

	RETURN

LFail:
	ROLLBACK TRANSACTION
	RETURN

GO

EXEC sp_settriggerorder 'TR_Field$_UpdateModel_InsLast', 'last', 'INSERT'
GO

-------------------------------------------------------------------------------

if object_id('TR_Field$_UpdateModel_Del') is not null begin
	print 'removing trigger TR_Field$_UpdateModel_Del'
	drop trigger TR_Field$_UpdateModel_Del
end
go
print 'creating trigger TR_Field$_UpdateModel_Del'
go
create trigger TR_Field$_UpdateModel_Del on Field$ for delete
as
	declare @Clid INT
	declare @DstCls INT
	declare @sName VARCHAR(100)
	declare @sClass VARCHAR(100)
	declare @sFlid VARCHAR(20)
	declare @Type INT
	DECLARE @nAbstract INT

	declare @Err INT
	declare @fIsNocountOn INT
	declare @sql VARCHAR(1000)

	set @fIsNocountOn = @@options & 512
	if @fIsNocountOn = 0 set nocount on

	-- get the first custom field to process
	Select @sFlid= min([id]) from deleted

	-- loop through all of the custom fields to be deleted
	while @sFlid is not null begin

		-- get deleted fields
		select 	@Type = [Type], @Clid = [Class], @sName = [Name], @DstCls = [DstCls]
		from	deleted
		where	[Id] = @sFlid

		-- get class name
		select 	@sClass = [Name], @nAbstract = Abstract  from class$  where [Id] = @Clid

		if @type IN (14,16,18,20) begin
			-- Remove any data stored for this multilingual custom field.
			declare @sTable VARCHAR(20)
			set @sTable = case @type
				when 14 then 'MultiStr$'
				when 16 then 'MultiTxt$ (No Longer Exists)'
				when 18 then 'MultiBigStr$'
				when 20 then 'MultiBigTxt$'
				end
			IF @type != 16  -- MultiTxt$ data will be deleted when the table is dropped
			BEGIN
				set @sql = 'DELETE FROM [' + @sTable + '] WHERE [Flid] = ' + @sFlid
				exec (@sql)
				set @Err = @@error
				if @Err <> 0 goto LFail
			END

			-- Remove the view created for this multilingual custom field.
			IF @type != 16
				set @sql = 'DROP VIEW [' + @sClass + '_' + @sName + ']'
			ELSE
				SET @sql = 'DROP TABLE [' + @sClass + '_' + @sName + ']'
			exec (@sql)
			set @Err = @@error
			if @Err <> 0 goto LFail
		end
		else if @type IN (23,25,27) begin
			-- Remove the view created for this custom OwningAtom/Collection/Sequence field.
			set @sql = 'DROP VIEW [' + @sClass + '_' + @sName + ']'
			exec (@sql)
			set @Err = @@error
			if @Err <> 0 goto LFail
			-- Check for any objects stored for this custom OwningAtom/Collection/Sequence field.
			declare @DelId INT
			select @DelId = [Id] FROM CmObject (readuncommitted) WHERE [OwnFlid$] = @sFlid
			set @Err = @@error
			if @Err <> 0 goto LFail
			if @DelId is not null begin
				raiserror('TR_Field$_UpdateModel_Del: Unable to remove %s field until corresponding objects are deleted',
						16, 1, @sName)
				goto LFail
			end
		end
		else if @type IN (26,28) begin
			-- Remove the table created for this custom ReferenceCollection/Sequence field.
			set @sql = 'DROP TABLE [' + @sClass + '_' + @sName + ']'
			exec (@sql)
			set @Err = @@error
			if @Err <> 0 goto LFail

			-- Remove the procedure that handles reference collections or sequences for
			-- the dropped table
			set @sql = N'
				IF OBJECT_ID(''ReplaceRefColl_' + @sClass +  '_' + @sName + ''') IS NOT NULL
					DROP PROCEDURE [ReplaceRefColl_' + @sClass + '_' + @sName + ']
				IF OBJECT_ID(''ReplaceRefSeq_' + @sClass +  '_' + @sName + ''') IS NOT NULL
					DROP PROCEDURE [ReplaceRefSeq_' + @sClass + '_' + @sName + ']'
			exec (@sql)
			set @Err = @@error
			if @Err <> 0 goto LFail
		end
		else begin
			-- Remove the format column created if this was a custom String field.
			if @type in (13,17) begin
				set @sql = 'ALTER TABLE [' + @sClass + '] DROP COLUMN [' + @sName + '_Fmt]'
				exec (@sql)
				set @Err = @@error
				if @Err <> 0 goto LFail
			end
			-- Remove the constraint created if this was a custom ReferenceAtom field.
			-- Not necessary for CmObject : Foreign Key constraints are not created agains CmObject
			if @type = 24 begin
				declare @sTarget VARCHAR(100)
				select @sTarget = [Name] FROM [Class$] WHERE [Id] = @DstCls
				set @Err = @@error
				if @Err <> 0 goto LFail
				if @sTarget != 'CmObject' begin
					set @sql = 'ALTER TABLE [' + @sClass + '] DROP CONSTRAINT [' +
						'_FK_' + @sClass + '_' + @sName + ']'
					exec (@sql)
					set @Err = @@error
					if @Err <> 0 goto LFail
				end
			end
			-- Remove Default Constraint from Numeric or Date fields before dropping the column
			If @type in (1,2,3,4,5,8) begin
				select @sql = 'ALTER TABLE [' + @sClass + '] DROP CONSTRAINT [' + so.name + ']'
				from sysconstraints sc
					join sysobjects so on so.id = sc.constid and so.name like 'DF[_]%'
					join sysobjects so2 on so2.id = sc.id
					join syscolumns sco on sco.id = sc.id and sco.colid = sc.colid
				where so2.name = @sClass   -- Tablename
				and   sco.name = @sName    -- Fieldname
				and   so2.type = 'U'	   -- Userdefined table
				exec (@sql)
				set @Err = @@error
				if @Err <> 0 goto LFail
			end

			-- Remove the column created for this custom field.
			set @sql = 'ALTER TABLE [' + @sClass + '] DROP COLUMN [' + @sName + ']'
			exec (@sql)
			set @Err = @@error
			if @Err <> 0 goto LFail

			-- fix the view associated with this class.
			exec @Err = UpdateClassView$ @Clid, 1
			if @Err <> 0 goto LFail
		end

		--( Rebuild the delete trigger

		EXEC @Err = CreateDeleteObj @Clid
		IF @Err <> 0 GOTO LFail

		--( Rebuild CreateObject_*

		IF @nAbstract != 1 BEGIN
			EXEC @Err = DefineCreateProc$ @Clid
			IF @Err <> 0 GOTO LFail
		END

		-- get the next custom field to process
		Select @sFlid= min([id]) from deleted  where [Id] > @sFlid

	end -- While loop

	--( Rebuild the stored function fnGetRefsToObj
	EXEC @Err = CreateGetRefsToObj
	IF @Err <> 0 GOTO LFail

	-- if nocount was turned on turn it off
	if @fIsNocountOn = 0 set nocount off
	return

LFail:
	rollback tran
	return
go

-------------------------------------------------------------------------------
-- Finish or roll back transaction as applicable
-------------------------------------------------------------------------------
declare @dbVersion int
SELECT @dbVersion = DbVer FROM Version$
if @dbVersion = 200137
begin
	UPDATE Version$ SET DbVer = 200138
	COMMIT TRANSACTION
	print 'database updated to version 200138'
end
else
begin
	ROLLBACK TRANSACTION
	print 'Update aborted: this works only if DbVer = 200137 (DbVer = ' +
			convert(varchar, @dbVersion) + ')'
end
GO