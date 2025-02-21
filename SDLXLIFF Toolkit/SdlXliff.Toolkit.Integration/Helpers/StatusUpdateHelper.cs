using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace SdlXliff.Toolkit.Integration.Helpers
{
    public class StatusUpdateHelper
    {
        public static void UnlockContent(ISegment targetSegment, IDocumentItemFactory itemFactory, IPropertiesFactory propFactory)
        {
            Location textLocation = new Location(targetSegment, true);
            bool isLContentFound = false;

            do
            {
                ILockedContent content = textLocation.ItemAtLocation as ILockedContent;
                if (content != null)
                {
                    isLContentFound = true;

                    int indexInParent = content.IndexInParent;
                    ILockedContent origLContent = (ILockedContent)content.Clone();
                    Location lockedLocation = new Location(origLContent.Content, true);
                    Location origLocation = new Location(CloneContainer(content.Parent), true);

                    // create new parent subitems
                    IAbstractMarkupDataContainer newParent = content.Parent;
                    newParent.Clear();

                    int index = 0;
                    do // loop by parent location
                    {
                        // take from locked selection
                        if (index == indexInParent)
                        {
                            do // loop by locked content
                                if (lockedLocation.ItemAtLocation != null)
                                    newParent.Add((IAbstractMarkupData)lockedLocation.ItemAtLocation.Clone());
                            while (lockedLocation.MoveNextSibling());
                            index++;
                        }
                        // take original items
                        else
                        {
                            if (origLocation.ItemAtLocation != null)
                                newParent.Add((IAbstractMarkupData)origLocation.ItemAtLocation.Clone());
                            index++;
                        }
                    }
                    while (origLocation.MoveNextSibling());
                }
            }
            while (textLocation.MoveNext());

            if (isLContentFound)
                // merge IText objects
                MergeMarkupData(targetSegment, itemFactory, propFactory);
        }

        public static void UpdateSegmentProperties(ISegmentPair item, SearchSettings settings)
        {
            // update confirmation level (segment status)
            if (settings.UpdateStatus)
                item.Properties.ConfirmationLevel = settings.NewStatus;

            // update lock status
            if (settings.LockSegment)
            {
                if (!item.Properties.IsLocked)
                    item.Properties.IsLocked = true;
            }
            else if (settings.UnlockContent)
                if (item.Properties.IsLocked)
                    item.Properties.IsLocked = false;
        }


        private static IAbstractMarkupDataContainer CloneContainer(IAbstractMarkupDataContainer container)
        {
            var segContainer = container as ISegment;
            if (segContainer != null)
                return (IAbstractMarkupDataContainer)segContainer.Clone();

            var txtContainer = container as IText;
            if (txtContainer != null)
                return (IAbstractMarkupDataContainer)txtContainer.Clone();

            var tagContainer = container as ITagPair;
            if (tagContainer != null)
                return (IAbstractMarkupDataContainer)tagContainer.Clone();

            var commContainer = container as ICommentMarker;
            if (commContainer != null)
                return (IAbstractMarkupDataContainer)commContainer.Clone();

            var revContainer = container as IRevisionMarker;
            if (revContainer != null)
                return (IAbstractMarkupDataContainer)revContainer.Clone();

           var locContainer = container as ILocationMarker;
            if (locContainer != null)
                return (IAbstractMarkupDataContainer)locContainer.Clone();

            return null;
        }
        private static void MergeMarkupData(ISegment segment, IDocumentItemFactory itemFactory, IPropertiesFactory propFactory)
        {
            Location textLocation = new Location(segment, true);

            IText prevText = null;

            do
                if (textLocation.ItemAtLocation != null)
                {
                    IAbstractMarkupData currData = (IAbstractMarkupData)textLocation.ItemAtLocation.Clone();
                    IText currText = currData as IText;
                    if (currText == null)
                        prevText = null;
                    else
                        if (prevText == null)
                        prevText = currText;
                    else
                    {
                        prevText = itemFactory.CreateText(propFactory.CreateTextProperties(prevText.Properties.Text +
                    currText.Properties.Text));
                        textLocation.ItemAtLocation.Parent.RemoveAt(textLocation.ItemAtLocation.IndexInParent);

                        textLocation.MovePrevious();
                        textLocation.ItemAtLocation.Parent[textLocation.ItemAtLocation.IndexInParent] = prevText;
                    }
                }
            while (textLocation.MoveNextSibling());
        }
    }
}
