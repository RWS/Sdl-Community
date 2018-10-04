using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtComparisonTool.Models
{
	public class UserResponse
	{
		public string Sid { get; set; }
		public List<OosAccounts> OosAccounts { get; set; }
	}
}
