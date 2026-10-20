using System.Collections.Generic;

namespace Sdl.Community.DsiViewer.Model
{
	public class SdlMtCloudFilterSettings : ModelBase
	{
		private string _model;
		private bool _qeAdequate;
		private bool _qeGood;
		private bool _qeNone;
		private bool _qePoor;

		public bool ByModel => !string.IsNullOrEmpty(Model);

		public bool ByQualityEstimation => QeAdequate || QeGood || QePoor || QeNone;

		public bool IsActive => ByModel || ByQualityEstimation;

		/// <summary>Short description of the active filter, e.g. "Good, Adequate · engfra_pro".</summary>
		public string Summary
		{
			get
			{
				var estimations = new List<string>();
				if (QeGood) estimations.Add(Constants.GoodQuality);
				if (QeAdequate) estimations.Add(Constants.AdequateQuality);
				if (QePoor) estimations.Add(Constants.PoorQuality);
				if (QeNone) estimations.Add("No QE");

				var parts = new List<string>();
				if (estimations.Count > 0) parts.Add(string.Join(", ", estimations));
				if (ByModel) parts.Add(Model);
				return string.Join(" · ", parts);
			}
		}

		public string Model
		{
			get => _model;
			set
			{
				_model = value;
				OnPropertyChanged(nameof(Model));
			}
		}

		public bool QeAdequate
		{
			get => _qeAdequate;
			set
			{
				_qeAdequate = value;
				OnPropertyChanged(nameof(QeAdequate));
			}
		}

		public bool QeGood
		{
			get => _qeGood;
			set
			{
				_qeGood = value;
				OnPropertyChanged(nameof(QeGood));
			}
		}

		public bool QeNone
		{
			get => _qeNone;
			set
			{
				_qeNone = value;
				OnPropertyChanged(nameof(QeNone));
			}
		}

		public bool QePoor
		{
			get => _qePoor;
			set
			{
				_qePoor = value;
				OnPropertyChanged(nameof(QePoor));
			}
		}

		public void ClearFilter()
		{
			Model = null;
			QeNone = false;
			QePoor = false;
			QeGood = false;
			QeAdequate = false;
		}
	}
}
