CREATE VIEW [dbo].[MemberDiagnosisAndCategory]
AS
WITH MostSevereDiagnosisPerCategory(MemberId, MostSevereDiagnosisID, DiagnosisCategoryID) AS
	(SELECT 
		md.MemberID, 
		MIN(md.DiagnosisID) AS MostSevereDiagnosisID, 
		dc.DiagnosisCategoryID
	FROM 
		dbo.MemberDiagnosis AS md
		INNER JOIN dbo.DiagnosisCategoryMap AS dcm ON dcm.DiagnosisID = md.DiagnosisID
		INNER JOIN dbo.DiagnosisCategory AS dc ON dcm.DiagnosisCategoryID = dc.DiagnosisCategoryID
		INNER JOIN dbo.Diagnosis AS d ON dcm.DiagnosisID = d.DiagnosisID
	GROUP BY 
		md.MemberID, 
		dc.DiagnosisCategoryID), 

MostSevereCategoryPerMember(MemberId, MostSevereCategoryId) AS
	(SELECT 
		md.MemberID, 
		MIN(dcm.DiagnosisCategoryID) AS MostSevereCategoryId
	FROM 
		dbo.MemberDiagnosis AS md 
		INNER JOIN dbo.DiagnosisCategoryMap AS dcm ON dcm.DiagnosisID = md.DiagnosisID 
		INNER JOIN dbo.DiagnosisCategory AS dc ON dcm.DiagnosisCategoryID = dc.DiagnosisCategoryID
	GROUP BY 
		md.MemberID)

	SELECT
		m.MemberID AS [Member ID], 
		m.FirstName AS [First Name], 
		m.LastName AS [Last Name],
		msd.MostSevereDiagnosisID AS [Most Severe Diagnosis ID],
		d.DiagnosisDescription AS [Most Severe Diagnosis Description],
		dc.DiagnosisCategoryID AS [Category ID], 
		dc.CategoryDescription AS [Category Description],
		dc.CategoryScore AS [Category Score],
	CASE
		WHEN(msc.MostSevereCategoryId IS NULL OR msc.MostSevereCategoryId = dc.DiagnosisCategoryID) THEN '1'
		ELSE '0'
	END AS [Is Most Severe Category]
	FROM
		dbo.Member AS m
		LEFT OUTER JOIN MostSevereDiagnosisPerCategory AS msd ON m.MemberID = msd.MemberId
		LEFT OUTER JOIN dbo.Diagnosis AS d ON msd.MostSevereDiagnosisID = d.DiagnosisID
		LEFT OUTER JOIN dbo.DiagnosisCategoryMap AS dcm ON d.DiagnosisID = dcm.DiagnosisID
		LEFT OUTER JOIN dbo.DiagnosisCategory AS dc ON dcm.DiagnosisCategoryID = dc.DiagnosisCategoryID
		LEFT OUTER JOIN MostSevereCategoryPerMember AS msc ON m.MemberID = msc.MemberId
GO
