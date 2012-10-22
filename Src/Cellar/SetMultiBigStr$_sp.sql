/***********************************************************************************************
 * SetMultiBigStr$
 *
 * Description:
 *	Inserts, updates, or deletes a multi-string in the MultiBigStr$ string table
 *
 * Parameters:
 *	@flid=field id of the string
 *	@obj=the object that owns the string
 *	@ws=the string's language writing system
 *	@txt=the text contents of the string, if null then the multi-string will be deleted
 *	@fmt=the string's formating
 *
 * Returns:
 *	0 if successful, otherwise an error code
 **********************************************************************************************/
if object_id('SetMultiBigStr$') is not null begin
	print 'removing proc SetMultiBigStr$'
	drop proc [SetMultiBigStr$]
end
go
print 'creating proc SetMultiBigStr$'
go
create proc [SetMultiBigStr$]
	@flid int,
	@obj int,
	@ws int,
	@txt ntext,
	@fmt image
as
	declare @sTranName varchar(50), @nTrnCnt int
	declare	@fIsNocountOn int, @Err int
	declare @nRowCnt int

	set @fIsNocountOn = @@options & 512
	if @fIsNocountOn = 0 set nocount on

	-- determine if a transaction already exists; if one does then create a savepoint,
	--	otherwise create a transaction
	set @nTrnCnt = @@trancount
	set @sTranName = 'SetMultiBigStr$_tr' + convert(varchar(8), @@nestlevel)
	if @nTrnCnt = 0 begin tran @sTranName
	else save tran @sTranName

	-- determine if the string should be removed
	if @txt is null or @txt like '' begin
		delete from MultiBigStr$
		where	[Flid] = @flid
			and [Obj] = @obj
			and [Ws] = @ws
		set @Err = @@error
		if @Err <> 0 goto LCleanUp
	end
	else begin
		-- attempt to update the string, if no rows are updated then insert the string
		update	MultiBigStr$
		set	[Txt] = @txt, [Fmt] = @fmt
		where	[Flid] = @flid
			and [Obj] = @obj
			and [Ws] = @ws
		select @Err = @@error, @nRowCnt = @@rowcount
		if @Err <> 0 goto LCleanUp
		if @nRowCnt = 0 begin
			insert into [MultiBigStr$] ([Flid], [Obj], [Ws], [Txt], [Fmt])
				values(@flid, @obj, @ws, @txt, @fmt)
			set @Err = @@error
			if @Err <> 0 goto LCleanUp
		end
	end

	-- update the timestamp column in CmObject by updating the update date and time
	update	[CmObject]
	set	[UpdDttm] = getdate()
	where	[Id] = @obj
	set @Err = @@error
	if @Err <> 0 goto LCleanUp

LCleanUp:
	-- if nocount was turned on turn it off
	if @fIsNocountOn = 0 set nocount off

	if @Err = 0 begin
		-- if a transaction was created within this procedure commit it
		if @nTrnCnt = 0 commit tran @sTranName
	end
	else begin
		rollback tran @sTranName
	end

	return @err
go