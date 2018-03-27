using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AhkPlugin.Model
{
	public class Script:ModelBase
	{
		private bool _isSelected;
		private bool _isActiveScript;
		private string _scriptStateAction;
		private string _rowColor;

		public string ScriptId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Text { get; set; }

		public string ScriptStateAction
		{
			get => _scriptStateAction;
			set
			{
				_scriptStateAction = value;
				OnPropertyChanged(nameof(ScriptStateAction));
			}
		}
		public string RowColor
		{
			get => _rowColor;
			set
			{
				_rowColor = value;
				OnPropertyChanged(nameof(RowColor));
			}
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}
		public bool Active
		{
			get => _isActiveScript;
			set
			{
				_isActiveScript = value;
				OnPropertyChanged(nameof(Active));
			}
		}

	}
}
