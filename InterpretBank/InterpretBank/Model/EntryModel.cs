using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace InterpretBank.Model
{
    public class EntryModel : ViewModelBase.ViewModel, IEquatable<EntryModel>
    {
        private string _entryComment;
        private ObservableCollection<TermModel> _terms;
        private string _name;

        public string EntryComment
        {
            get => _entryComment;
            set => SetField(ref _entryComment, value);
        }

        public string GlossaryName { get; set; }
        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public string SubGlossaryName { get; set; }

        public ObservableCollection<TermModel> Terms
        {
            get => _terms;
            set => SetField(ref _terms, value);
        }

        public bool Equals(EntryModel other)
        {
            return AreCollectionsEqual(other) && EntryComment == other.EntryComment && GlossaryName == other.GlossaryName && Name == other.Name && SubGlossaryName == other.SubGlossaryName;
        }

        private bool AreCollectionsEqual(EntryModel other)
        {
            var firstNotSecond = Terms.Except(other.Terms).ToList();
            var secondNotFirst = other.Terms.Except(Terms).ToList();

            return !firstNotSecond.Any() && !secondNotFirst.Any();
        }
    }
}