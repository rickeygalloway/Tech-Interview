using ConsoleApp.Data;
using System;
using System.Data.SqlClient;

namespace ConsoleApp
{
	internal static class Setup
	{
		/// <summary>
		/// This class ensures that the MemberDiagnosisAndCategory view needed for EntityFramework has been created.
		/// 
		/// If the View is not present, then it will be created.
		/// 
		/// Of course, all of this is assuming that other aspects of the system is in place - Database, tables, data, permissions, etc.
		/// </summary>
		/// <returns></returns>
		public static bool EnsureSetup()
		{
			bool isSetupCorrect = false;
			var connectionString = GetConnectionString();

			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();

				isSetupCorrect = CheckForView(connection);

				if (!isSetupCorrect)
				{
					CreateView(connection);
					isSetupCorrect = CheckForView(connection);
				}

				connection.Close();
			}

			return isSetupCorrect;
		}

		private static string GetConnectionString()
		{
			string connectionString = string.Empty;
			try
			{
				connectionString = new Pulse8TestDBEntities().Database.Connection.ConnectionString;
			}
			catch (Exception ex)
			{
				ScreenOutput.WriteLine("Could not retrieve database ConnectionString, Please check your App.Config file.", ConsoleColor.Red);
				ScreenOutput.WriteLine(string.Empty);
				ScreenOutput.WriteLine("Error: " + ex.Message, ConsoleColor.Red);
			}

			return connectionString;
		}

		private static bool CheckForView(SqlConnection connection)
		{
			var sqlCheck = "SELECT 1 FROM sys.views WHERE NAME='MemberDiagnosisAndCategory'";
			using (var command = new SqlCommand(sqlCheck, connection))
			{
				var response = command.ExecuteReader();

				if (!response.HasRows)
				{
					return false;
				}
				return true;
			}
		}

		private static void CreateView(SqlConnection connection)
		{
			var viewSql = @"CREATE VIEW [dbo].[MemberDiagnosisAndCategory]
						AS
						WITH MostSevereDiagnosisPerCategory(MemberId, MostSevereDiagnosisID, DiagnosisCategoryID) AS
							(SELECT md.MemberID, MIN(md.DiagnosisID) AS MostSevereDiagnosisID, dc.DiagnosisCategoryID
							FROM dbo.MemberDiagnosis AS md
							INNER JOIN dbo.DiagnosisCategoryMap AS dcm ON dcm.DiagnosisID = md.DiagnosisID
							INNER JOIN dbo.DiagnosisCategory AS dc ON dcm.DiagnosisCategoryID = dc.DiagnosisCategoryID
							INNER JOIN dbo.Diagnosis AS d ON dcm.DiagnosisID = d.DiagnosisID
							GROUP BY md.MemberID, dc.DiagnosisCategoryID), 
						MostSevereCategoryPerMember(MemberId, MostSevereCategoryId) AS
							(SELECT md.MemberID, MIN(dcm.DiagnosisCategoryID) AS MostSevereCategoryId
							 FROM dbo.MemberDiagnosis AS md INNER JOIN
							 dbo.DiagnosisCategoryMap AS dcm ON dcm.DiagnosisID = md.DiagnosisID INNER JOIN
							 dbo.DiagnosisCategory AS dc ON dcm.DiagnosisCategoryID = dc.DiagnosisCategoryID
							 GROUP BY md.MemberID)

						 SELECT
							m.MemberID AS [Member ID], m.FirstName AS [First Name], m.LastName AS [Last Name],
							msd.MostSevereDiagnosisID AS [Most Severe Diagnosis ID],
							d.DiagnosisDescription AS [Most Severe Diagnosis Description],
							dc.DiagnosisCategoryID AS [Category ID], dc.CategoryDescription AS [Category Description],
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
							 LEFT OUTER JOIN MostSevereCategoryPerMember AS msc ON m.MemberID = msc.MemberId";

			var command = new SqlCommand(viewSql, connection);
			command.ExecuteNonQuery();
		}
	}
}
