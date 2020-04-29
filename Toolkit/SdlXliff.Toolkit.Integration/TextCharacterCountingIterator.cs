using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;

namespace SdlXliff.Toolkit.Integration
{
    public class TextCharacterCountingIterator : CharacterCountingIterator
    {
        public TextCharacterCountingIterator(Location startLocation)
            : base(startLocation,
            () => new StartOfItemTextCharacterCounterVisitor(),
            () => new EndOfItemTextCharacterCounterVisitor())
        {
        }

        /// <summary>
        /// Visitor that counts "start of item" characters as text characters,
        /// does not count start tags and only counts text equivalents for placeholder tags.
        /// </summary>
        protected class StartOfItemTextCharacterCounterVisitor : StartOfItemCharacterCounterVisitor
        {
            private bool isCheckTags = false;

            public override void VisitTagPair(ITagPair tagPair)
            {
                // don't count characters for tag pairs
                if (isCheckTags)
                    VisitChildren(tagPair);
            }

            public override void VisitLockedContent(ILockedContent lockedContent)
            {
                isCheckTags = true;
                VisitChildren((IAbstractMarkupDataContainer)lockedContent.Content);
                isCheckTags = false;
            }

            public override void VisitPlaceholderTag(IPlaceholderTag tag)
            {
                //if (tag.Properties.HasTextEquivalent)
                //{
                //    // count text equivalent characters for placeholder tags
                //    Count += tag.Properties.TextEquivalent.Length;
                //}
            }

            private void VisitChildren(IAbstractMarkupDataContainer container)
            {
                foreach (var item in container)
                {
                    item.AcceptVisitor(this);
                }
            }
        }


        protected class EndOfItemTextCharacterCounterVisitor : EndOfItemCharacterCounterVisitor
        {
            public override void VisitTagPair(ITagPair tagPair)
            {
                // don't count end tags
            }
        }
    }
}
