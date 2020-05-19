using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.XLIFF.Manager.Wizard.ViewModel;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class TransactionModel: BaseModel
	{			
		public List<ProjectFileActionModel> ProjectFileActions { get; set; }
	}
}
