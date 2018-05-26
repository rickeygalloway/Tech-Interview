--Used Common Table Expression's for readablity instead of subquery or #temp table.
--In a production environment with much more data, more analysis may be needed to ensure optimum performance.

--I did have some questions along to way, and I was told to call if I had questions.  This work was done late at
--night, so instead of stopping, I continued with "My understanding of this" for each section.

WITH

--Requirement
---------------------------------------------------------------------------------------------------------------------
	--3B - Most Severe Diagnosis ID and Description should be the diagnosis with the lowest Diagnosis ID present for 
	--each Member’s Category. 

	--My understanding of this:
	--Get the lowest DiagnosisId for EACH of the member's category.

	--Return:
	--John Smith has categories 2 and 3 ==> lowest DiagnosisId of 2 and 4 respectively for each category
	--Will Smyth has categories 3 and 3 ==> multiple DiagnosisId's of 3 and 4 - 3 is lowest
	--Jack Smith none => null

	--Output:
	--MemberId	MostSevereDiagnosisID	DiagnosisCategoryID
	--1			2						2
	--1			4						3
	--3			3						3
MostSevereDiagnosisPerCategory (MemberId, MostSevereDiagnosisID, DiagnosisCategoryID) AS
		(SELECT 
			md.MemberId, 
			min(md.DiagnosisId) as MostSevereDiagnosisID, 
			dc.DiagnosisCategoryID
		FROM 
			MemberDiagnosis md 
			INNER JOIN DiagnosisCategoryMap dcm ON dcm.DiagnosisID = md.DiagnosisID
			INNER JOIN DiagnosisCategory dc ON dcm.DiagnosisCategoryID = dc.DiagnosisCategoryID
			INNER JOIN Diagnosis d ON dcm.DiagnosisID = d.DiagnosisID
		GROUP BY
			md.MemberID,
			dc.DiagnosisCategoryID), 
		
--Requirement:
---------------------------------------------------------------------------------------------------------------------
	--3c Is Most Severe Category – 0 or 1 should be set to 1 for the lowest Category ID present for each Member. 
	--Severity is not based on category score. Please also set this to 1 for Members without corresponding Categories. 

	--My understanding of this:
	--This is to find the lowest CategoryId for the member. Use this MostSevereCategoryId value in a case statement to 
	--write out 0 or 1 to indicate 'Is Most Severe Category' for each member

	--Output:
	--MemberId	MostSevereCategoryId
	--1			2			
	--3			3
MostSevereCategoryPerMember (MemberId, MostSevereCategoryId) AS
		(SELECT 
			md.MemberId, 
			min(dcm.DiagnosisCategoryID) as MostSevereCategoryId
		FROM
			MemberDiagnosis md 
			INNER JOIN DiagnosisCategoryMap dcm ON dcm.DiagnosisID = md.DiagnosisID
			INNER JOIN DiagnosisCategory dc ON dcm.DiagnosisCategoryID = dc.DiagnosisCategoryID
		GROUP BY 
			md.MemberId)

--Requirement: 
---------------------------------------------------------------------------------------------------------------------
	--3a Please include the following fields: Member ID, First Name, Last Name, Most Severe Diagnosis ID, 
	--Most Severe Diagnosis Description, Category ID, Category Description, Category Score and Is Most Severe Category

	--My understanding of this:
	--These are litterally the field names desired - including spacing and casing


--Requirement:
---------------------------------------------------------------------------------------------------------------------
	--3c Please also set this to 1 for Members without corresponding Categories. 

	--My understanding of this:
	--Even though the member has no category, this still represents the Most Severe Category. This was accomplished 
	--by a null check in the Case statement in the SELECT clause below



--Requirement:
---------------------------------------------------------------------------------------------------------------------
	--3d
	--Members without Diagnosis or Categories should be included in the result set. 

	--My understanding of this:
	--Show members without this information and show NULL where applicable. This was accomplished by using the 
	--LEFT JOINS in the statement below

SELECT 
	m.MemberId AS [Member ID], 
	m.FirstName as [First Name], 
	m.LastName AS [Last Name], 
	msd.MostSevereDiagnosisID AS [Most Severe Diagnosis ID], 
	d.DiagnosisDescription AS [Most Severe Diagnosis Description], 
	dc.DiagnosisCategoryID [Category ID], 
	dc.CategoryDescription  AS [Category Description], 
	dc.CategoryScore AS [Category Score], 
	Case 
		When (msc.MostSevereCategoryId is null OR msc.MostSevereCategoryId = dc.DiagnosisCategoryID) then '1'
		Else '0'
	End as [Is Most Severe Category]
FROM
	Member m 
	LEFT JOIN MostSevereDiagnosisPerCategory msd ON m.MemberId = msd.MemberId
	LEFT JOIN Diagnosis d ON msd.MostSevereDiagnosisId =  d.DiagnosisID
	LEFT JOIN DiagnosisCategoryMap dcm on d.DiagnosisID = dcm.DiagnosisID
	LEFT JOIN DiagnosisCategory dc ON dcm.DiagnosisCategoryID = dc.DiagnosisCategoryID
	LEFT JOIN MostSevereCategoryPerMember msc on m.MemberID = msc.MemberId

--OUTPUT
--Member ID	First Name	Last Name	Most Severe Diagnosis ID	Most Severe Diagnosis Description	Category ID	Category Description	Category Score	Is Most Severe Category
--1			John		Smith		2							Test Diagnosis 2					2			Category B				20				1
--1			John		Smith		4							Test Diagnosis 4					3			Category C				30				0
--2			Jack		Smith		NULL						NULL								NULL		NULL					NULL			1
--3			Will		Smyth		3							Test Diagnosis 3					3			Category C				30				1