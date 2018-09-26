using System.Collections.Generic;
using Sdl.Community.SdlTmAnonymizer.Helpers;
using Sdl.Community.SdlTmAnonymizer.Model;

namespace Sdl.Community.SdlTmAnonymizer.Services
{
	public class UsersService
	{
		public List<User> GetImportedUsers(List<string> files)
		{
			var listOfUsers = new List<User>();
			foreach (var file in files)
			{
				var package = ExcelFile.GetExcelPackage(file);
				var workSheet = package.Workbook.Worksheets[1];
				for (var i = workSheet.Dimension.Start.Row;
					i <= workSheet.Dimension.End.Row;
					i++)
				{
					var user = new User()
					{
						IsSelected = true,
						UserName = string.Empty,
						Alias = string.Empty
					};
					for (var j = workSheet.Dimension.Start.Column;
						j <= workSheet.Dimension.End.Column;
						j++)
					{
						var address = workSheet.Cells[i, j].Address;

						var cellValue = workSheet.Cells[i, j].Value;
						if (address.Contains("A") && cellValue != null)
						{
							user.UserName = cellValue.ToString();
						}
						else if (address.Contains("B") && cellValue != null)
						{
							user.Alias = cellValue.ToString();
						}
					}
					listOfUsers.Add(user);
				}
			}
			return listOfUsers;
		}

		public void ExportUsers(string filePath, List<User> users)
		{
			var package = ExcelFile.GetExcelPackage(filePath);
			var worksheet = package.Workbook.Worksheets.Add("Exported system fields");
			var lineNumber = 1;
			foreach (var user in users)
			{
				if (user != null)
				{
					worksheet.Cells["A" + lineNumber].Value = user.UserName;
					worksheet.Cells["B" + lineNumber].Value = user.Alias;
					lineNumber++;
				}
			}
			package.Save();
		}
	}
}
