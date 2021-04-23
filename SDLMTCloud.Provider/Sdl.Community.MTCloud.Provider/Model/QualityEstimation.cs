﻿using System.Collections.Generic;
using System.Linq;
using Sdl.Community.MTCloud.Provider.ViewModel;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class QualityEstimation : BaseViewModel
	{
		private int? _index;
		private string _originalEstimation;

		public int? Index
		{
			get => _index;
			set
			{
				_index = value;
				OnPropertyChanged(nameof(Index));
			}
		}

		public string OriginalEstimation
		{
			get => _originalEstimation;
			set
			{
				_originalEstimation = value;
				Index = _originalEstimation != null ? (int?) Qualifiers[_originalEstimation] : null;
				OnPropertyChanged(nameof(OriginalEstimation));
			}
		}

		public bool UserChoseDifferently => Qualifiers[OriginalEstimation] != Index;

		public string UserEstimation => Qualifiers.FirstOrDefault(q => q.Value == Index).Key;

		private Dictionary<string, int> Qualifiers { get; } = new Dictionary<string, int>
		{
			["GOOD"] = 0,
			["ADEQUATE"] = 1,
			["POOR"] = 2
		};
	}
}