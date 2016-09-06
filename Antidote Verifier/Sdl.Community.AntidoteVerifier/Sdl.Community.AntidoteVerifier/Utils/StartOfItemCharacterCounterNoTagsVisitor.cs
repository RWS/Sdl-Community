using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using static Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi.CharacterCountingIterator;

namespace Sdl.Community.AntidoteVerifier.Utils
{
    public class StartOfItemCharacterCounterNoTagsVisitor : ICharacterCountingVisitor
    {
        int _count;
        bool _inLockedContent;
        public int Count
        {
            get
            {
                return _count;
            }

            set
            {
                _count = value;
            }
        }

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            // start of comments have no character count
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
            // location markers have no charcter count
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            // all locked counted must be counted, as it is treated
            // as a single unit.
            if (_inLockedContent)
            {
                // we are already iterating recursively through all locked content,
                // must not do it again here, as that would cause nested locked
                // content to be counted multiple times.
                return;
            }

            // track the fact that we are in locked content, so that we avoid
            // counting nested locked content multiple times while vising all the sub-items.
            _inLockedContent = true;

            foreach (IAbstractMarkupData item in lockedContent.Content.AllSubItems)
            {
                item.AcceptVisitor(this);
            }
            // done

            _inLockedContent = false;
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            // start of other marker has no character count
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            //we don't count the tags
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            // start of revision marker has no character count.
        }

        public void VisitSegment(ISegment segment)
        {
            // the "start" of a segment has no character count
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            //we don't count the tags
        }

        public void VisitText(IText text)
        {
            _count += text.Properties.Text.Length;
        }
    }
}
