using System;
using System.ComponentModel;
using SDLCommunityCleanUpTasks.Models;

namespace SDLCommunityCleanUpTasks.Utilities
{
	public class ConversionItemDescriptionProvider : TypeDescriptionProvider
    {
        private static TypeDescriptionProvider defaultProvider = TypeDescriptor.GetProvider(typeof(ConversionItem));

        public ConversionItemDescriptionProvider()
            : base(defaultProvider)
        {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            ICustomTypeDescriptor parentDescriptor = base.GetTypeDescriptor(objectType, instance);

            return new ConversionItemTypeDescriptor(parentDescriptor);
        }
    }
}