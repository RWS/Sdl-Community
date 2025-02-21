using Sdl.Community.SDLBatchAnonymize.ViewModel;
using Xunit;

namespace Sdl.Community.SDLBatchAnonymizeUT
{
	public class BatchnonymizerSettingsVmUnitTest
	{
		private readonly BatchAnonymizerSettingsViewModel _anonymizerVm;

		public BatchnonymizerSettingsVmUnitTest()
		{
			_anonymizerVm = new BatchAnonymizerSettingsViewModel();
		}

		[Theory]
		[InlineData(true)]
		public void AnonymizeAllSelected_FirePropertyChanged(bool anonymizeAll)
		{
			var changedPropertyName = string.Empty;
			_anonymizerVm.PropertyChanged += (sender, args) => changedPropertyName = args.PropertyName;
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;
			Assert.Equal(nameof(_anonymizerVm.AnonymizeAllSettings),changedPropertyName);
		}

		[Theory]
		[InlineData(true)]
		public void AnonymizeAllSelected_CreatedByChecked(bool anonymizeAll)
		{
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.True(_anonymizerVm.CreatedByChecked);
		}

		[Theory]
		[InlineData(true,"jane doe")]
		public void AnonymizeAllSelected_ClearCreatedBy(bool anonymizeAll,string createdBy)
		{
			_anonymizerVm.CreatedByName = createdBy;

			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.Equal(string.Empty, _anonymizerVm.CreatedByName);
		}

		[Theory]
		[InlineData("jane doe")]
		public void SetCreatedBy(string createdBy)
		{
			_anonymizerVm.CreatedByName = createdBy;

			Assert.Equal(createdBy ,_anonymizerVm.CreatedByName);
		}

		[Theory]
		[InlineData("jane doe")]
		public void UnckeckCreatedBy_ClearCreatedBy(string createdBy)
		{
			_anonymizerVm.CreatedByChecked = true;
			_anonymizerVm.CreatedByName = createdBy;

			_anonymizerVm.CreatedByChecked = false;
			Assert.Equal(string.Empty,_anonymizerVm.CreatedByName);

		}
		[Theory]
		[InlineData("jane doe")]
		public void CheckCreatedBy(string createdBy)
		{
			_anonymizerVm.CreatedByName = createdBy;

			Assert.True(_anonymizerVm.CreatedByChecked);
		}

		[Theory]
		[InlineData(true, "jane doe")]
		public void AnonymizeAllSelected_ClearModifyBy(bool anonymizeAll, string userName)
		{
			_anonymizerVm.ModifyByName = userName;
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.Equal(string.Empty, _anonymizerVm.ModifyByName);
		}

		[Theory]
		[InlineData(true)]
		public void AnonymizeAllSelected_ModifyByChecked(bool anonymizeAll)
		{
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.True(_anonymizerVm.ModifyByChecked);
		}

		[Theory]
		[InlineData("jane doe")]
		public void SetModifiedBy(string modifiedBy)
		{
			_anonymizerVm.ModifyByName = modifiedBy;

			Assert.Equal(modifiedBy, _anonymizerVm.ModifyByName);
		}
		[Theory]
		[InlineData("jane doe")]
		public void UncheckModifyBy_ClearModifyBy(string modifiedBy)
		{
			_anonymizerVm.ModifyByChecked = true;
			_anonymizerVm.ModifyByName = modifiedBy;

			_anonymizerVm.ModifyByChecked = false;
			Assert.Equal(string.Empty, _anonymizerVm.ModifyByName);

		}
		[Theory]
		[InlineData("jane doe")]
		public void CheckModifyBy(string modifiedBy)
		{
			_anonymizerVm.ModifyByName = modifiedBy;

			Assert.True(_anonymizerVm.ModifyByChecked);
		}

		[Theory]
		[InlineData(true, "jane doe")]
		public void AnonymizeAllSelected_ClearCommentAuthor(bool anonymizeAll, string userName)
		{
			_anonymizerVm.CommentAuthorName = userName;

			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.Equal(string.Empty, _anonymizerVm.CommentAuthorName);
		}

		[Theory]
		[InlineData(true)]
		public void AnonymizeAllSelected_CommentChecked(bool anonymizeAll)
		{
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.True(_anonymizerVm.CommentChecked);
		}

		[Theory]
		[InlineData("jane doe")]
		public void SetCommentAuthor(string commentAuthor)
		{
			_anonymizerVm.CommentAuthorName = commentAuthor;

			Assert.Equal(commentAuthor, _anonymizerVm.CommentAuthorName);
		}
		[Theory]
		[InlineData("jane doe")]
		public void UncheckComment_ClearComment(string commentAuthor)
		{
			_anonymizerVm.CommentChecked = true;
			_anonymizerVm.CommentAuthorName = commentAuthor;

			_anonymizerVm.CommentChecked = false;
			Assert.Equal(string.Empty, _anonymizerVm.CommentAuthorName);

		}
		[Theory]
		[InlineData("jane doe")]
		public void CheckCommentAuthor(string commentAuthor)
		{
			_anonymizerVm.CommentAuthorName = commentAuthor;

			Assert.True(_anonymizerVm.CommentChecked);
		}

		[Theory]
		[InlineData(true, "jane doe")]
		public void AnonymizeAllSelected_ClearTrackedAuthor(bool anonymizeAll, string userName)
		{
			_anonymizerVm.TrackedName = userName;

			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.Equal(string.Empty, _anonymizerVm.TrackedName);
		}

		[Theory]
		[InlineData(true)]
		public void AnonymizeAllSelected_TrackedChecked(bool anonymizeAll)
		{
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.True(_anonymizerVm.TrackedChecked);
		}

		[Theory]
		[InlineData("jane doe")]
		public void SetTrackedAuthor(string trackedChangesAuthor)
		{
			_anonymizerVm.TrackedName = trackedChangesAuthor;

			Assert.Equal(trackedChangesAuthor, _anonymizerVm.TrackedName);
		}
		[Theory]
		[InlineData("jane doe")]
		public void UncheckTracked_ClearTracked(string trackedAuthor)
		{
			_anonymizerVm.TrackedChecked = true;
			_anonymizerVm.TrackedName = trackedAuthor;

			_anonymizerVm.TrackedChecked = false;
			Assert.Equal(string.Empty, _anonymizerVm.TrackedName);

		}
		[Theory]
		[InlineData("jane doe")]
		public void CheckTrackedAuthor(string trackedAuthor)
		{
			_anonymizerVm.TrackedName = trackedAuthor;

			Assert.True(_anonymizerVm.TrackedChecked);
		}

		[Theory]
		[InlineData(true)]
		public void AnonymizeAllSelected_RemoveMtChecked(bool anonymizeAll)
		{
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.True(_anonymizerVm.ChangeMtChecked);
		}

		[Theory]
		[InlineData(true)]
		public void AnonymizeAllSelected_RemoveTmUnchecked(bool anonymizeAll)
		{
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.False(_anonymizerVm.ChangeTmChecked);
		}

		[Theory]
		[InlineData(true)]
		public void AnonymizeAllSelected_UncheckRemoveTm(bool anonymizeAll)
		{
			_anonymizerVm.ChangeTmChecked = true;
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.False(_anonymizerVm.ChangeTmChecked);
		}

		[Theory]
		[InlineData(true)]
		public void AnonymizeAllSelected_SetSpecificResource(bool anonymizeAll)
		{
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.False(_anonymizerVm.SetSpecificResChecked);
		}

		[Theory]
		[InlineData(true)]
		public void AnonymizeAllSelected_UncheckSetSpecificResource(bool anonymizeAll)
		{
			_anonymizerVm.SetSpecificResChecked = true;
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.False(_anonymizerVm.ChangeTmChecked);
		}

		[Theory]
		[InlineData(true,80)]
		public void AnonymizeAllSelected_ClearFuzzyScore(bool anonymizeAll,decimal tmScore)
		{
			_anonymizerVm.SetSpecificResChecked = true;
			_anonymizerVm.FuzzyScore = tmScore;
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.Equal(0,_anonymizerVm.FuzzyScore);
		}

		[Theory]
		[InlineData(70)]
		public void FuzzyScore_SetValue(decimal tmScore)
		{
			_anonymizerVm.SetSpecificResChecked = true;
			_anonymizerVm.FuzzyScore = tmScore;

			Assert.Equal(70, _anonymizerVm.FuzzyScore);
		}

		[Theory]
		[InlineData(true, "testTm")]
		public void AnonymizeAllSelected_ClearTmName(bool anonymizeAll, string tmName)
		{
			_anonymizerVm.SetSpecificResChecked = true;
			_anonymizerVm.TmName = tmName;
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;

			Assert.Equal(string.Empty, _anonymizerVm.TmName);
		}

		[Theory]
		[InlineData("firstTm")]
		public void SetTmName(string tmName)
		{
			_anonymizerVm.TmName = tmName;

			Assert.Equal(tmName, _anonymizerVm.TmName);
		}

		[Theory]
		[InlineData(90)]
		public void UncheckSpecificRes_ClearTmScore(decimal score)
		{
			_anonymizerVm.SetSpecificResChecked = true;
			_anonymizerVm.FuzzyScore = score;

			_anonymizerVm.SetSpecificResChecked = false;
			
			Assert.Equal(0,_anonymizerVm.FuzzyScore);
		}

		[Theory]
		[InlineData("RM name")]
		public void UncheckSpecificRes_ClearTmName(string tmName)
		{
			_anonymizerVm.SetSpecificResChecked = true;
			_anonymizerVm.TmName = tmName;

			_anonymizerVm.SetSpecificResChecked = false;

			Assert.Equal(string.Empty, _anonymizerVm.TmName);
		}

		[Theory]
		[InlineData(90)]
		public void AddTmScore_CheckSpecificRes(decimal score)
		{
			_anonymizerVm.FuzzyScore = score;

			Assert.True(_anonymizerVm.SetSpecificResChecked);
		}

		[Theory]
		[InlineData("TM name")]
		public void AddTmName_CheckSpecificRes(string tmName)
		{
			_anonymizerVm.TmName = tmName;

			Assert.True(_anonymizerVm.SetSpecificResChecked);
		}

		[Theory]
		[InlineData(true)]
		public void DisableFuzzy_ForTMAnonymization(bool tmAnonymization)
		{
			_anonymizerVm.ChangeTmChecked = tmAnonymization;

			Assert.False(_anonymizerVm.IsFuzzyEnabled);
		}
		[Theory]
		[InlineData(true)]
		public void EnableFuzzy_TMUnchecked(bool tmAnonymization)
		{
			_anonymizerVm.ChangeTmChecked = tmAnonymization;
			_anonymizerVm.ChangeTmChecked = false;

			Assert.True(_anonymizerVm.IsFuzzyEnabled);
		}

		[Theory]
		[InlineData(true,true)]
		public void FuzzyEnabled_ForTM_AndMT(bool tmAnonymization, bool mtAnonymization)
		{
			_anonymizerVm.ChangeTmChecked = tmAnonymization;
			_anonymizerVm.ChangeMtChecked = mtAnonymization;

			Assert.True(_anonymizerVm.IsFuzzyEnabled);
		}

		[Theory]
		[InlineData(true)]
		public void FuzzyDisabled_AnonymizeAll(bool anonymizeAll)
		{
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;
			Assert.True(_anonymizerVm.IsFuzzyEnabled);
		}

		[Theory]
		[InlineData(true)]
		public void FuzzyEnabled_AnonymizeAllFalse(bool anonymizeAll)
		{
			_anonymizerVm.AnonymizeAllSettings = anonymizeAll;
			_anonymizerVm.AnonymizeAllSettings = false;

			Assert.True(_anonymizerVm.IsFuzzyEnabled);
		}
	}
}
