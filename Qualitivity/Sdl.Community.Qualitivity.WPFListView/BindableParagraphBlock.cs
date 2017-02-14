using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Sdl.Community.WPFListView
{
    public class BindableParagraphBlock : Paragraph
    {
        public Inline BoundInline
        {
            get
            {
                return (Inline)GetValue(BoundInlineProperty);
            }
            set
            {
                SetValue(BoundInlineProperty, value);
            }
        }

        public BindableParagraphBlock()
        {
            Helpers.FixupDataContext(this);
        }

     
        public static readonly DependencyProperty BoundInlineProperty = DependencyProperty.Register("BoundInline"
       , typeof(Inline)
       , typeof(BindableParagraphBlock)
       , new PropertyMetadata(OnBoundSpanChanged));

        private static void OnBoundSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BindableParagraphBlock)d).Inlines.Clear();
            if (e.NewValue != null)
                ((BindableParagraphBlock)d).Inlines.Add(e.NewValue as Inline);        
        }







    }

    internal static class Helpers
    {
        /// <summary>
        /// If you use a bindable flow document element more than once, you may encounter a "Collection was modified" exception.
        /// The error occurs when the binding is updated because of a change to an inherited dependency property. The most common scenario 
        /// is when the inherited DataContext changes. It appears that an inherited properly like DataContext is propagated to its descendants. 
        /// When the enumeration of descendants gets to a BindableXXX, the dependency properties of that element change according to the new 
        /// DataContext, which change the (non-dependency) properties. However, for some reason, changing the flow content invalidates the 
        /// enumeration and raises an exception. 
        /// To work around this, one can either DataContext="{Binding DataContext, RelativeSource={RelativeSource AncestorType=FrameworkElement}}" 
        /// in code. This is clumsy, so every derived type calls this function instead (which performs the same thing).
        /// See http://code.logos.com/blog/2008/01/data_binding_in_a_flowdocument.html
        /// </summary>
        /// <param name="element"></param>
        public static void FixupDataContext(FrameworkContentElement element)
        {
            var b = new Binding(FrameworkContentElement.DataContextProperty.Name)
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(FrameworkElement), 1)
            };
            // another approach (if this one has problems) is to bind to an ancestor by ElementName
            element.SetBinding(FrameworkContentElement.DataContextProperty, b);
        }


        private static bool InternalUnFixupDataContext(DependencyObject dp)
        {
            // only consider those elements for which we've called FixupDataContext(): they all belong to this namespace
            if (!(dp is FrameworkContentElement) || dp.GetType().Namespace != typeof(Helpers).Namespace)
                return LogicalTreeHelper.GetChildren(dp).OfType<DependencyObject>().Any(InternalUnFixupDataContext);
            var binding = BindingOperations.GetBinding(dp, FrameworkContentElement.DataContextProperty);
            if (binding == null || binding.Path == null ||
                binding.Path.Path != FrameworkContentElement.DataContextProperty.Name || binding.RelativeSource == null ||
                binding.RelativeSource.Mode != RelativeSourceMode.FindAncestor ||
                binding.RelativeSource.AncestorType != typeof(FrameworkElement) ||
                binding.RelativeSource.AncestorLevel != 1)
                return LogicalTreeHelper.GetChildren(dp).OfType<DependencyObject>().Any(InternalUnFixupDataContext);
            BindingOperations.ClearBinding(dp, FrameworkContentElement.DataContextProperty);
            return true;
            // as soon as we have disconnected a binding, return. Don't continue the enumeration, since the collection may have changed
        }


        public static void UnFixupDataContext(DependencyObject dp)
        {
            while (InternalUnFixupDataContext(dp))
            {
            }
        }



        public static FrameworkContentElement LoadDataTemplate(DataTemplate dataTemplate)
        {
            object content = dataTemplate.LoadContent();
            var fragment = content as Fragment;
            if (fragment != null)
                return fragment.Content;
            var block = content as TextBlock;
            if (block == null) 
                throw new Exception("Data template needs to contain a <Fragment> or <TextBlock>");
            var inlines = block.Inlines;
            if (inlines.Count == 1)
                return inlines.FirstInline;
            var paragraph = new Paragraph();
            // we can't use an enumerator, since adding an inline removes it from its collection
            while (inlines.FirstInline != null)
                paragraph.Inlines.Add(inlines.FirstInline);
            return paragraph;
        }
    }

    [ContentProperty("Content")]
    public class Fragment : FrameworkElement
    {
        private static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(FrameworkContentElement), typeof(Fragment));

        public FrameworkContentElement Content
        {
            get
            {
                return (FrameworkContentElement)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }
    }
}
