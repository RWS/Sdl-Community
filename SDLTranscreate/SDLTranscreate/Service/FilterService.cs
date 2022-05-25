using System;
using Rws.MultiSelectComboBox.API;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.Service
{
	public class FilterService : IFilterService
	{
		private string _criteria;

		public void SetFilter(string criteria)
		{
			_criteria = criteria;
			ConfigureFilter();
		}

		public Predicate<object> Filter { get; set; }

		private bool FilteringByName(object item)
		{
			return string.IsNullOrEmpty(_criteria) || ((FilterItem)item).ToString().ToLower().Contains(_criteria.ToLower());
		}

		private void ConfigureFilter()
		{
			Filter = FilteringByName;
		}
	}
}
