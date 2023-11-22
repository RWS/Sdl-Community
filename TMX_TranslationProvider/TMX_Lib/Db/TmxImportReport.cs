using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TMX_Lib.Db
{
	public class TmxImportReport : INotifyPropertyChanged 
	{
		private int _tusRead = 0;
		private int _tusImportedSuccessfully = 0;
		private int _tusWithSyntaxErrors = 0;
		private int _tusWithInvalidChars = 0;
		private int _languageCount = 0;
		private DateTime _startTime = DateTime.MinValue;
		private DateTime _endTime = DateTime.MinValue;
		private string _fatalError = "";

		public int TUsRead { 
			get => _tusRead; 
			set {
				if (_tusRead == value) return;
				_tusRead = value;
				OnPropertyChanged();
			}
		} 
		public int TUsImportedSuccessfully
		{
			get => _tusImportedSuccessfully;
			set { 
				if (_tusImportedSuccessfully == value) return;
				_tusImportedSuccessfully = value;
				OnPropertyChanged();
			}
		}
		public int TUsWithSyntaxErrors
		{
			get => _tusWithSyntaxErrors;
			set { 
				if (_tusWithSyntaxErrors == value) return;
				_tusWithSyntaxErrors = value;
				OnPropertyChanged();
			}
		}
		public int TUsWithInvalidChars
		{
			get => _tusWithInvalidChars;
			set {
				if (_tusWithInvalidChars == value) return;
				_tusWithInvalidChars = value;
				OnPropertyChanged();
			}
		}
		public int LanguageCount
		{
			get => _languageCount;
			set {
				if (_languageCount == value) return;
				_languageCount = value;
				OnPropertyChanged();
			}
		}
		public DateTime StartTime
		{
			get => _startTime;
			private set {
				if (_startTime == value) return;
				_startTime = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsStarted));
				OnPropertyChanged(nameof(ReportTimeSecs));
				OnPropertyChanged(nameof(HasFinished));
			}
		}
		public DateTime EndTime
		{
			get => _endTime;
			set {
				if (_endTime == value) return;
				_endTime = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ReportTimeSecs));
				OnPropertyChanged(nameof(HasFinished));
			}
		}
		public string FatalError
		{
			get => _fatalError;
			set {
				if (_fatalError == value) return;
				_fatalError = value;
				OnPropertyChanged();
			}
		}


		public static TmxImportReport StartNow()
		{
			var r = new TmxImportReport();
			r.StartTime = DateTime.Now;
			return r;
		}

		public bool IsFatalError => FatalError != "";
		public bool IsStarted => StartTime != DateTime.MinValue;
		public bool IsEnded => EndTime != DateTime.MinValue;
		public int ReportTimeSecs => IsStarted ? ((int)((IsEnded ? EndTime : DateTime.Now) - StartTime).TotalSeconds) : 0;
		public bool HasFinished => IsStarted && EndTime != DateTime.MinValue;

		public TmxImportReport Copy()
		{
			var copy = new TmxImportReport();
			copy.CopyFrom(this);
			return copy;
		}

		public void CopyFrom(TmxImportReport other)
		{
			TUsRead = other.TUsRead;
			TUsImportedSuccessfully = other.TUsImportedSuccessfully;
			TUsWithSyntaxErrors = other.TUsWithSyntaxErrors;
			TUsWithInvalidChars = other.TUsWithInvalidChars;
			LanguageCount = other.LanguageCount;
			StartTime = other.StartTime;
			EndTime = other.EndTime;
			FatalError = other.FatalError;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
