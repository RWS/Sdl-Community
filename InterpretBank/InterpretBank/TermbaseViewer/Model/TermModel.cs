using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Runtime.CompilerServices;

namespace InterpretBank.TermbaseViewer.Model
{
	public class TermModel : INotifyPropertyChanged
	{
		private string _commentAll;
		private bool _isEditing;
		private int _sourceLanguageIndex;
		private string _sourceTerm;
		private string _sourceTermComment1;
		private string _sourceTermComment2;
		private int _targetLanguageIndex;
		private string _targetTerm;
		private string _targetTermComment1;
		private string _targetTermComment2;

		public TermModel()
		{
			Id = -1;
		}

		public TermModel(long id, string targetTerm, string targetTermComment1, string targetTermComment2, string sourceTerm, string sourceTermComment1, string sourceTermComment2, string commentAll, int sourceLanguageIndex, int targetLanguageIndex)
		{
			Id = id;
			_targetTerm = targetTerm;
			_targetTermComment1 = targetTermComment1;
			_targetTermComment2 = targetTermComment2;
			_sourceTerm = sourceTerm;
			_sourceTermComment1 = sourceTermComment1;
			_sourceTermComment2 = sourceTermComment2;
			_commentAll = commentAll;
			_sourceLanguageIndex = sourceLanguageIndex;
			_targetLanguageIndex = targetLanguageIndex;

			SetOriginalTerm();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public string CommentAll
		{
			get => _commentAll;
			set => SetField(ref _commentAll, value);
		}

		public bool Edited => !ContentEquals(OriginalTerm);

		public long Id { get; set; }

		public bool IsEditing
		{
			get => _isEditing;
			set
			{
				if (value == _isEditing)
					return;
				_isEditing = value;
				OnPropertyChanged();
			}
		}

		public int SourceLanguageIndex
		{
			get => _sourceLanguageIndex;
			set => _sourceLanguageIndex = value;
		}

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

		public int TargetLanguageIndex
		{
			get => _targetLanguageIndex;
			set => _targetLanguageIndex = value;
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

		public bool ContentEquals(TermModel other)
		{
			return _commentAll == other._commentAll && _sourceTerm == other._sourceTerm && _sourceTermComment1 == other._sourceTermComment1 && _sourceTermComment2 == other._sourceTermComment2 && _targetTerm == other._targetTerm && _targetTermComment1 == other._targetTermComment1 && _targetTermComment2 == other._targetTermComment2;
		}

		//public override bool Equals(object other)
		//{
		//	var termModel = other as TermModel;
		//	if (termModel is null) return false;

		//	return _commentAll == termModel._commentAll && _sourceTerm == termModel._sourceTerm && _sourceTermComment1 == termModel._sourceTermComment1 && _sourceTermComment2 == termModel._sourceTermComment2 && _targetTerm == termModel._targetTerm && _targetTermComment1 == termModel._targetTermComment1 && _targetTermComment2 == termModel._targetTermComment2;
		//}

		//public override int GetHashCode()
		//{
		//	unchecked
		//	{
		//		var hashCode = (_commentAll != null ? _commentAll.GetHashCode() : 0);
		//		hashCode = (hashCode * 397) ^ (_sourceTerm != null ? _sourceTerm.GetHashCode() : 0);
		//		hashCode = (hashCode * 397) ^ (_sourceTermComment1 != null ? _sourceTermComment1.GetHashCode() : 0);
		//		hashCode = (hashCode * 397) ^ (_sourceTermComment2 != null ? _sourceTermComment2.GetHashCode() : 0);
		//		hashCode = (hashCode * 397) ^ (_targetTerm != null ? _targetTerm.GetHashCode() : 0);
		//		hashCode = (hashCode * 397) ^ (_targetTermComment1 != null ? _targetTermComment1.GetHashCode() : 0);
		//		hashCode = (hashCode * 397) ^ (_targetTermComment2 != null ? _targetTermComment2.GetHashCode() : 0);
		//		return hashCode;
		//	}
		//}

		public void Revert()
		{
			if (OriginalTerm == null)
				return;
			SourceTerm = OriginalTerm.SourceTerm;
			SourceTermComment1 = OriginalTerm.SourceTermComment1;
			SourceTermComment2 = OriginalTerm.SourceTermComment2;
			TargetTerm = OriginalTerm.TargetTerm;
			TargetTermComment1 = OriginalTerm.TargetTermComment1;
			TargetTermComment2 = OriginalTerm.TargetTermComment2;
			CommentAll = OriginalTerm.CommentAll;
			OnPropertyChanged(nameof(Edited));
		}

		public void SetOriginalTerm(bool triggerOnPropertyChanged = false)
		{
			OriginalTerm = new TermModel
			{
				TargetTerm = TargetTerm,
				TargetTermComment1 = TargetTermComment1,
				TargetTermComment2 = TargetTermComment2,
				SourceTerm = SourceTerm,
				SourceTermComment1 = SourceTermComment1,
				SourceTermComment2 = SourceTermComment2,
				CommentAll = CommentAll
			};

			if (triggerOnPropertyChanged)
				OnPropertyChanged(nameof(Edited));
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