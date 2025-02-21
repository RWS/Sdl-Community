using System;

namespace InterpretBank.GlossaryService.Model
{
	public class TimeRange
	{
		private DateTime? _creationDateEnd;
		private DateTime? _creationDateStart;
		private DateTime? _editDateEnd;
		private DateTime? _editDateStart;

		public TimeRange(DateTime? creationDateStart, DateTime? creationDateEnd, DateTime? editDateStart, DateTime? editDateEnd)
		{
			_creationDateStart = creationDateStart;
			_creationDateEnd = creationDateEnd;
			_editDateStart = editDateStart;
			_editDateEnd = editDateEnd;
		}
	}
}