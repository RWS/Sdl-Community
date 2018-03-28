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
		private string _description;
		private string _text;

		public string ScriptId { get; set; }
		public string Name { get; set; }
		public string Description {
			get => _description;
			set
			{
				_description = value;
				OnPropertyChanged(nameof(Description));
			}
		}
		public string Text {
			get => _text;
			set
			{
				_text = value;
				OnPropertyChanged(nameof(Text));
			}
		}

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
