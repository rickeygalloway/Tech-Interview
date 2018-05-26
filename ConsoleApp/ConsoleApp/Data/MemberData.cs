using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp.Data
{
	internal static class MemberData
	{
		internal static List<MemberDiagnosisAndCategory> GetMemberInfoByMemberId(int memberId)
		{
			var memberList = new Pulse8TestDBEntities().MemberDiagnosisAndCategories.Where(m => m.Member_ID == memberId);

			return memberList.ToList();
		}
	}
}
