using ConsoleApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{

	//There's a wide variety of expectations when doing code for a potential interview. We do this often at work and are typically 
	//looking for basic functionality. Not production level code.

	//I basically took the approach of keeping it pretty simple.

	//I also wanted to use Entity Framework, which I think greatly simplifies the code.I started off by choosing to model everything 
	//off of the tables in the database. But since there are no relationships among the tables (at least not by any table constraints), 
	//and I did not want to manually add associations to the edmx file, I based the entire model on a view.I could have easily just 
	//used my same SQL statement with a where clause for member id, but that doesn't really show much.


	//Notes:
	//1.	Use EntityFramework for retrieving data.
	//2.	Ensure that view is created.
	//3.	Encapsulate a few helper classes to clean up the code
	//4.	I purposely did not use any error handling anywhere except when looking for a connectionstring value. This is what is most likely 
	//		error to occur if another dev runs the app.
	//5.	I used inline SQL statements.I almost never do this in a production application no matter how simple the task.
	//6.	Very few comments.I try to name variables and methods that explain what is happening.I did comment the Setup class and explained 
	//		its purpose.
	//7.	Admittedly, I read glassdoor reviews on interviews - and response.Seems I have the same assignment as others.And I did create a 
	//		video of my working application.

	class Program
	{
		private static readonly string _indentation = "        ";

		static void Main(string[] args)
		{
			Setup.EnsureSetup();
			ProcessUserInput();
		}

		private static void ProcessUserInput()
		{
			do
			{
				ScreenOutput.Write("Memeber ID: ");
				var id = Console.ReadLine();

				if (int.TryParse(id, out int memberId))
					LookupMemberInformation(memberId);
				else
					HandleMemberNotFoundOutput();
			}
			while (true); //Just let user manually close app
		}

		private static void LookupMemberInformation(int memberId)
		{
			var memberInfoList = MemberData.GetMemberInfoByMemberId(memberId);

			if (memberInfoList.Any())
				HandleMemberFoundOutput(memberInfoList);
			else
				HandleMemberNotFoundOutput();
		}

		private static void HandleMemberFoundOutput(List<MemberDiagnosisAndCategory> memberList)
		{
			var MemberInformationOutput =
				_indentation + "Member ID.............................. {0}" + Environment.NewLine +
				_indentation + "First Name............................. {1}" + Environment.NewLine +
				_indentation + "Last Name.............................. {2}" + Environment.NewLine +
				_indentation + "Most Severe Diagnosis ID............... {3}" + Environment.NewLine +
				_indentation + "Most Severe Diagnosis Description...... {4}" + Environment.NewLine +
				_indentation + "Category ID............................ {5}" + Environment.NewLine +
				_indentation + "Category Description................... {6}" + Environment.NewLine +
				_indentation + "Category Score......................... {7}" + Environment.NewLine +
				_indentation + "Is Most Severe Category................ {8}" + Environment.NewLine;

			foreach (var memberInfo in memberList)
			{
				ScreenOutput.WriteLine(string.Empty);

				var output = string.Format(MemberInformationOutput,
						memberInfo.Member_ID.ToString(),
						memberInfo.First_Name,
						memberInfo.Last_Name,
						memberInfo.Most_Severe_Diagnosis_ID.ToString(),
						memberInfo.Most_Severe_Diagnosis_Description,
						memberInfo.Category_ID.ToString(),
						memberInfo.Category_Description,
						memberInfo.Category_Score.ToString(),
						memberInfo.Is_Most_Severe_Category);

				ScreenOutput.WriteLine(output, ConsoleColor.DarkGreen);
			}
		}

		private static void HandleMemberNotFoundOutput()
		{
			ScreenOutput.WriteLine(string.Empty);
			ScreenOutput.WriteLine(_indentation + "Member not Found", ConsoleColor.Red);
			ScreenOutput.WriteLine(string.Empty);
		}
	}
}
