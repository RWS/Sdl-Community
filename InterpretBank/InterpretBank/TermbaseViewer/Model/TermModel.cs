using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace InterpretBank.TermbaseViewer.Model
{
	public class TermModel : INotifyPropertyChanged
	{
		private string _commentAll;
		private string _sourceTerm;
		private string _sourceTermComment1;
		private string _sourceTermComment2;
		private string _targetTerm;
		private string _targetTermComment1;
		private string _targetTermComment2;

		public TermModel()
		{ }

		public TermModel(long id, string targetTerm, string targetTermComment1, string targetTermComment2, string sourceTerm, string sourceTermComment1, string sourceTermComment2, string commentAll)
		{
			Id = id;
			_targetTerm = targetTerm;
			_targetTermComment1 = targetTermComment1;
			_targetTermComment2 = targetTermComment2;
			_sourceTerm = sourceTerm;
			_sourceTermComment1 = sourceTermComment1;
			_sourceTermComment2 = sourceTermComment2;
			_commentAll = commentAll;

			OriginalTerm = new TermModel()
			{
				Id = id,
				TargetTerm = targetTerm,
				TargetTermComment1 = targetTermComment1,
				TargetTermComment2 = targetTermComment2,
				SourceTerm = sourceTerm,
				SourceTermComment1 = sourceTermComment1,
				SourceTermComment2 = sourceTermComment2,
				CommentAll = commentAll
			};
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public string CommentAll
		{
			get => _commentAll;
			set => SetField(ref _commentAll, value);
		}

		public bool Edited
		{
			get
			{
				var properties = GetType().GetProperties(BindingFlags.Public);
				return properties.Any(propertyInfo => !propertyInfo.GetValue(this).Equals(propertyInfo.GetValue(OriginalTerm)));
			}
		}

		public long Id { get; set; }

		public string SourceTerm
		{
			get => _sourceTerm;
			set => SetField(ref _sourceTerm, value);
		}

		public string SourceTermComment1
		{
			get => _sourceTermComment1;
			set => SetField(ref _sourceTermComment1, value);
		}

		public string SourceTermComment2
		{
			get => _sourceTermComment2;
			set => SetField(ref _sourceTermComment2, value);
		}

		public string TargetTerm
		{
			get => _targetTerm;
			set => SetField(ref _targetTerm, value);
		}

		public string TargetTermComment1
		{
			get => _targetTermComment1;
			set => SetField(ref _targetTermComment1, value);
		}

		public string TargetTermComment2
		{
			get => _targetTermComment2;
			set => SetField(ref _targetTermComment2, value);
		}

		private TermModel OriginalTerm { get; set; }

		public override bool Equals(object obj)
		{
			return obj is TermModel other && Id == other.Id && CommentAll == other.CommentAll && TargetTermComment1 == other.TargetTermComment1 && TargetTermComment2 == other.TargetTermComment2 && TargetTerm == other.TargetTerm && SourceTerm == other.SourceTerm && SourceTermComment1 == other.SourceTermComment1 && SourceTermComment2 == other.SourceTermComment2;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Id.GetHashCode();
				hashCode = (hashCode * 397) ^ (CommentAll != null ? CommentAll.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TargetTermComment1 != null ? TargetTermComment1.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TargetTermComment2 != null ? TargetTermComment2.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TargetTerm != null ? TargetTerm.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SourceTerm != null ? SourceTerm.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SourceTermComment1 != null ? SourceTermComment1.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SourceTermComment2 != null ? SourceTermComment2.GetHashCode() : 0);
				return hashCode;
			}
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return false;
			field = value;

			OnPropertyChanged(propertyName);
			OnPropertyChanged(nameof(Edited));
			return true;
		}
	}
}