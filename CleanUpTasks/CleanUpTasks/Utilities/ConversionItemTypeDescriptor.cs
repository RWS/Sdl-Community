using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sdl.Community.CleanUpTasks.Models;

namespace Sdl.Community.CleanUpTasks.Utilities
{
	public class ConversionItemTypeDescriptor : CustomTypeDescriptor
    {
        public ConversionItemTypeDescriptor(ICustomTypeDescriptor parent)
            : base(parent)
        {
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var props = new List<PropertyDescriptor>();

            foreach (PropertyDescriptor outer in base.GetProperties(attributes))
            {
                props.Add(outer);

                if (outer.PropertyType == typeof(SearchText) || outer.PropertyType == typeof(ReplacementText))
                {
                    foreach (PropertyDescriptor inner in outer.GetChildProperties())
                    {
                        if (inner.PropertyType == typeof(string))
                        {
                            props.Add(new ConversionItemPropertyDescriptor(outer, inner, outer.Name + inner.Name));
                        }
                        else
                        {
                            props.Add(inner);
                        }
                    }
                }
            }

            return new PropertyDescriptorCollection(props.ToArray());
        }
    }
}