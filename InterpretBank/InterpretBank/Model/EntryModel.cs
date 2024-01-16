using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace InterpretBank.Model
{
    public class EntryModel : ViewModelBase.ViewModel, IEquatable<EntryModel>
    {
        private ObservableCollection<TermModel> _terms;
        public string EntryComment { get; set; }
        public string GlossaryName { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
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

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 0;
                hashCode = (hashCode * 397) ^ (EntryComment != null ? EntryComment.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (GlossaryName != null ? GlossaryName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SubGlossaryName != null ? SubGlossaryName.GetHashCode() : 0);
                return hashCode;
            }
        }

        private bool AreCollectionsEqual(EntryModel other)
        {
            var firstNotSecond = Terms.Except(other.Terms).ToList();
            var secondNotFirst = other.Terms.Except(Terms).ToList();

            return !firstNotSecond.Any() && !secondNotFirst.Any();
        }
    }
}