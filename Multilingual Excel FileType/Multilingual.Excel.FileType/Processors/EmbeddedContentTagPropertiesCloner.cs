using System.Collections.Generic;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Processors
{
    internal class EmbeddedContentTagPropertiesCloner
    {
        public void Process(IParagraphUnit paragraphUnit)
        {
            CloneEmbeddecContentTagPairs(paragraphUnit);
            CloneEmbeddedContentPlaceholders(paragraphUnit);
        }

        private void CloneEmbeddedContentPlaceholders(IParagraphUnit paragraphUnit)
        {
            var clonableAbstractTags = GetClonableTags<IPlaceholderTag>(paragraphUnit);

            if (clonableAbstractTags.Count < 1) return;

            foreach (var placeholder in GetAllSubItems<IPlaceholderTag>(paragraphUnit.Target))
            {
                if (!placeholder.Properties.MetaDataContainsKey(EmbeddedContentConstants.EmbeddedContentMetaKey))
                    continue;

                var metaDataValue = placeholder.Properties.GetMetaData(EmbeddedContentConstants.EmbeddedContentMetaKey);

                foreach (var clonablePlaceholder in clonableAbstractTags.Where(clonablePlaceholder => clonablePlaceholder.TagProperties.GetMetaData(EmbeddedContentConstants.EmbeddedContentMetaKey) == metaDataValue))
                {
                    placeholder.Properties = (clonablePlaceholder as IPlaceholderTag).Properties.Clone() as IPlaceholderTagProperties;
                }
            }
        }

        private void CloneEmbeddecContentTagPairs(IParagraphUnit paragraphUnit)
        {
            var clonableAbstractTags = GetClonableTags<ITagPair>(paragraphUnit);

            if (clonableAbstractTags.Count < 1)
                return;

            foreach (var tagPair in GetAllSubItems<ITagPair>(paragraphUnit.Target))
            {
                if (!tagPair.TagProperties.MetaDataContainsKey(EmbeddedContentConstants.EmbeddedContentMetaKey))
                    continue;

                var metaDataValue = tagPair.TagProperties.GetMetaData(EmbeddedContentConstants.EmbeddedContentMetaKey);

                foreach (var clonableTagPair in clonableAbstractTags.Where(clonableTagPair => clonableTagPair.TagProperties.GetMetaData(EmbeddedContentConstants.EmbeddedContentMetaKey) == metaDataValue))
                {
                    tagPair.StartTagProperties = (clonableTagPair as ITagPair).StartTagProperties.Clone() as IStartTagProperties;
                    tagPair.EndTagProperties = (clonableTagPair as ITagPair).EndTagProperties.Clone() as IEndTagProperties;
                }
            }
        }

        private List<IAbstractTag> GetClonableTags<T>(IParagraphUnit paragraphUnit) where T : IAbstractTag
        {
            var embeddedContentTags = GetAllSubItems<T>(paragraphUnit.Source)
                .Where(abstractTag => abstractTag.TagProperties.MetaDataContainsKey(EmbeddedContentConstants.EmbeddedContentMetaKey));

            var clonableAbstractTags = new List<IAbstractTag>();

            foreach (var abstractTag in embeddedContentTags.Where(abstractTag => !clonableAbstractTags.Exists(tagsFromList =>
                tagsFromList.TagProperties.GetMetaData(EmbeddedContentConstants.EmbeddedContentMetaKey) ==
                abstractTag.TagProperties.GetMetaData(EmbeddedContentConstants.EmbeddedContentMetaKey))))
            {
                clonableAbstractTags.Add(abstractTag);
            }
            return clonableAbstractTags;
        }

        private static IList<T> GetAllSubItems<T>(IAbstractMarkupDataContainer container)
        {
            var abstractMarkupDataList = new List<T>();
            abstractMarkupDataList.AddRange(container.AllSubItems.OfType<T>().ToList());

            foreach (var lockedContent in container.AllSubItems.OfType<ILockedContent>().ToList())
            {
                abstractMarkupDataList.AddRange(GetAllSubItems<T>(lockedContent.Content));
            }

            return abstractMarkupDataList;
        }
    }
}
