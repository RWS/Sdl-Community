using System;
using System.ComponentModel;

namespace Sdl.Community.CleanUpTasks.Utilities
{
	public class ConversionItemPropertyDescriptor : PropertyDescriptor
    {
        private readonly PropertyDescriptor parentDescriptor = null;
        private readonly PropertyDescriptor childDescriptor = null;

        public ConversionItemPropertyDescriptor(PropertyDescriptor parent,
                                                PropertyDescriptor child,
                                                string pdName)
            : base(pdName, null)
        {
            parentDescriptor = parent;
            childDescriptor = child;
        }

        public override Type ComponentType { get { return childDescriptor.ComponentType; } }

        public override bool IsReadOnly { get { return childDescriptor.IsReadOnly; } }

        public override Type PropertyType { get { return childDescriptor.PropertyType; } }

		public override bool CanResetValue(object component)
		{
			return childDescriptor.CanResetValue(component);
		}

		public override object GetValue(object component)
		{
			return childDescriptor.GetValue(parentDescriptor.GetValue(component));
		}

		public override void ResetValue(object component)
		{
			childDescriptor.ResetValue(parentDescriptor.GetValue(component));
		}

		public override void SetValue(object component, object value)
        {
            childDescriptor.SetValue(parentDescriptor.GetValue(component), value);
            OnValueChanged(component, EventArgs.Empty);
        }

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}
	}
}