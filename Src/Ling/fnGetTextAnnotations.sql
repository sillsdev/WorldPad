/***********************************************************************************************
* Function: fnGetTextAnnotations
*
* Description: 
*	Gets the annotations for a text, whether they be glosses, analyses, or word forms
*
* Parameters: 
*	@nvcTextName = Name of the text
*	@nVernacularWS = Object ID of the vernaculare writing system
*	@nAnalysisWS = Object ID of the analysis writing system
*
* Returns: 
*	Table containing annotations
*
* Calling sample:
*	SELECT * FROM fnGetTextAnnotations('My Green Mat', NULL, NULL)
*
* Notes:
*	This function was requested by Ken as a support utility. It is not used in any
*	application code, but should still be kept.
**********************************************************************************************/

IF OBJECT_ID('fnGetTextAnnotations') IS NOT NULL BEGIN
	PRINT 'removing function fnGetTextAnnotations'
	DROP FUNCTION fnGetTextAnnotations
END
GO
PRINT 'creating function fnGetTextAnnotations'
GO

CREATE FUNCTION dbo.fnGetTextAnnotations(
	@nvcTextName NVARCHAR(4000),
	@nVernacularWs INT = NULL,
	@nAnalysisWS INT = NULL)
RETURNS @tblTextAnnotations TABLE (
	TextId INT,
	TextName NVARCHAR(4000),
	Paragraph INT, 
	StTxtParaId INT,
	BeginOffset INT,
	EndOffset INT,
	AnnotationId INT,
	WordFormId INT,
	Wordform NVARCHAR(4000),
	AnalysisId INT,
	GlossId INT,
	Gloss NVARCHAR(4000))
AS
BEGIN
	DECLARE @nAnnotationDefnPIC INT

	SELECT @nAnnotationDefnPIC = Obj
	FROM CmPossibility_Name
	WHERE Txt = 'Punctuation In Context'

	IF @nAnalysisWS IS NULL
		SELECT TOP 1 @nAnalysisWS = Dst 
		FROM LangProject_CurAnalysisWss ORDER BY Ord
	IF @nVernacularWS IS NULL
		SELECT TOP 1 @nVernacularWS = dst
		FROM LangProject_CurVernWss ORDER BY Ord

	-- REVIEW (SteveMiller): Most of these queries (joined together by the 
	-- UNIONs) can be optimized by dropping out some tables. Since this is
	-- utility function, and it moves pretty fast already, I didn't take
	-- the time to tweak it anymore.
	
	-- REVIEW (SteveMiller): Text segment queries are still not being
	-- picked up. If those are desired, another query will be needed,
	-- UNIONed with the rest of them.
	
	--== Annotation is not an InstanceOf anything ==--
	INSERT INTO @tblTextAnnotations
	SELECT
		cmon.Obj AS TextId,
		cmon.Txt AS TextName,
		tp.Ord AS Paragraph, 
		stp.Id AS StTxtParaId,
		cba.BeginOffset,
		cba.EndOffset,
		cba.Id AS AnnotationId,
		NULL AS WordFormId,
		SUBSTRING(stp.Contents, cba.BeginOffset + 1, cba.EndOffset - cba.BeginOffset) 
			COLLATE SQL_Latin1_General_CP1_CI_AS AS WordForm, --( avoids collate mismatch
		NULL AS AnalysisId,
		NULL AS GlossId,
		NULL AS Gloss
	FROM CmMajorObject_Name cmon
	JOIN Text_Contents tc ON tc.Src = cmon.Obj
	JOIN StText st ON st.Id = tc.Dst
	JOIN StText_Paragraphs tp ON tp.Src = st.Id
	JOIN StTxtPara stp ON stp.Id = tp.Dst
	JOIN CmBaseAnnotation cba ON cba.BeginObject = stp.Id
	JOIN CmAnnotation ca ON ca.Id = cba.Id
	WHERE ca.InstanceOf IS NULL
		AND cmon.Txt = @nvcTextName
		AND ca.AnnotationType = @nAnnotationDefnPIC
	--== Annotation is an InstanceOf Wordform ==--
	UNION
	SELECT
		cmon.Obj AS TextId,
		cmon.Txt AS TextName,
		tp.Ord AS Paragraph, 
		stp.Id AS StTxtParaId,
		cba.BeginOffset,
		cba.EndOffset,
		cba.Id AS AnnotationId,
		wwff.Obj AS WordFormId,
		wwff.Txt AS WordForm,
		NULL AS AnalysisId,
		NULL AS GlossId,
		NULL AS Gloss
	FROM CmMajorObject_Name cmon
	JOIN Text_Contents tc ON tc.Src = cmon.Obj
	JOIN StText st ON st.Id = tc.Dst
	JOIN StText_Paragraphs tp ON tp.Src = st.Id
	JOIN StTxtPara stp ON stp.Id = tp.Dst
	JOIN CmBaseAnnotation cba ON cba.BeginObject = stp.Id
	JOIN CmAnnotation ca ON ca.Id = cba.Id
	JOIN WfiWordForm_Form wwff ON wwff.Obj = ca.InstanceOf AND wwff.WS = @nVernacularWS
	WHERE cmon.Txt = @nvcTextName
	--== Annotation is an InstanceOf Annotation ==--
	UNION
	SELECT
		cmon.Obj AS TextId,
		cmon.Txt AS TextName,
		tp.Ord AS Paragraph, 
		stp.Id AS StTxtParaId,
		cba.BeginOffset,
		cba.EndOffset,
		cba.Id AS AnnotationId,
		wwff.Obj AS WordFormId,
		wwff.Txt AS WordForm,
		wa.Id AS AnalysisId,
		NULL AS GlossId,
		NULL AS Gloss
	FROM CmMajorObject_Name cmon
	JOIN Text_Contents tc ON tc.Src = cmon.Obj
	JOIN StText st ON st.Id = tc.Dst
	JOIN StText_Paragraphs tp ON tp.Src = st.Id
	JOIN StTxtPara stp ON stp.Id = tp.Dst
	JOIN CmBaseAnnotation cba ON cba.BeginObject = stp.Id
	JOIN CmAnnotation ca ON ca.Id = cba.Id
	JOIN WfiAnalysis wa ON wa.Id = ca.InstanceOf
	LEFT OUTER JOIN WfiWordForm_Analyses wwfa ON wwfa.Dst = wa.Id
	LEFT OUTER JOIN WfiWordForm_Form wwff ON wwff.Obj = wwfa.Src AND wwff.WS = @nVernacularWS
	WHERE cmon.Txt = @nvcTextName
	--== Annotation is an InstanceOf Gloss ==--
	UNION
	SELECT
		cmon.Obj AS TextId,
		cmon.Txt AS TextName,
		tp.Ord AS Paragraph, 
		stp.Id AS StTxtParaId,
		cba.BeginOffset,
		cba.EndOffset,
		cba.Id AS AnnotationId,
		wwff.Obj AS WordFormId,
		wwff.Txt AS WordForm,
		wa.Id AS AnalysisId,
		wgf.Obj AS GlossId,
		wgf.Txt AS Gloss
	FROM CmMajorObject_Name cmon
	JOIN Text_Contents tc ON tc.Src = cmon.Obj
	JOIN StText st ON st.Id = tc.Dst
	JOIN StText_Paragraphs tp ON tp.Src = st.Id
	JOIN StTxtPara stp ON stp.Id = tp.Dst
	JOIN CmBaseAnnotation cba ON cba.BeginObject = stp.Id
	JOIN CmAnnotation ca ON ca.Id = cba.Id
	JOIN WfiGloss_Form wgf ON wgf.Obj = ca.InstanceOf AND wgf.WS = @nAnalysisWS
	LEFT OUTER JOIN WfiAnalysis_Meanings wam ON wam.Dst = wgf.Obj
	LEFT OUTER JOIN WfiAnalysis wa ON wa.Id = wam.Src
	LEFT OUTER JOIN WfiWordForm_Analyses wwfa ON wwfa.Dst = wa.Id
	LEFT OUTER JOIN WfiWordForm_Form wwff ON wwff.Obj = wwfa.Src AND wwff.WS = @nVernacularWS
	WHERE cmon.Txt = @nvcTextName
	ORDER BY tp.Ord, cba.BeginOffset

	RETURN
END
GO