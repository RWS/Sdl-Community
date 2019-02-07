using System;
using System.ComponentModel;
using Sdl.Community.CleanUpTasks.Models;

namespace Sdl.Community.CleanUpTasks.Utilities
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