/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license
can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Sdl.Community.Studio.Time.Tracker.ExcelPowerTools
{
    public static class PtExtensions
    {
        public static string StringConcatenate(this IEnumerable<string> source)
        {
            var sb = new StringBuilder();
            foreach (var s in source)
                sb.Append(s);
            return sb.ToString();
        }

        public static string StringConcatenate<T>(
            this IEnumerable<T> source,
            Func<T, string> projectionFunc)
        {
            return source.Aggregate(
                new StringBuilder(),
                (s, i) => s.Append(projectionFunc(i)),
                s => s.ToString());
        }

        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> func)
        {
            var ie1 = first.GetEnumerator();
            var ie2 = second.GetEnumerator();
            while (ie1.MoveNext() && ie2.MoveNext())
                yield return func(ie1.Current, ie2.Current);
        }

        public static IEnumerable<IGrouping<TKey, TSource>> GroupAdjacent<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            var last = default(TKey);
            var haveLast = false;
            var list = new List<TSource>();

            foreach (var s in source)
            {
                var k = keySelector(s);
                if (haveLast)
                {
                    if (!k.Equals(last))
                    {
                        yield return new GroupOfAdjacent<TSource, TKey>(list, last);
                        list = new List<TSource>();
                        list.Add(s);
                        last = k;
                    }
                    else
                    {
                        list.Add(s);
                        last = k;
                    }
                }
                else
                {
                    list.Add(s);
                    last = k;
                    haveLast = true;
                }
            }
            if (haveLast)
                yield return new GroupOfAdjacent<TSource, TKey>(list, last);
        }

        private static void InitializeReverseDocumentOrder(XElement element)
        {
            XElement prev = null;
            foreach (var e in element.Elements())
            {
                e.AddAnnotation(new ReverseDocumentOrderInfo { PreviousSibling = prev });
                prev = e;
            }
        }

        public static IEnumerable<XElement> ElementsBeforeSelfReverseDocumentOrder(
            this XElement element)
        {
            if (element.Annotation<ReverseDocumentOrderInfo>() == null)
                InitializeReverseDocumentOrder(element.Parent);
            var current = element;
            while (true)
            {
                var previousElement = current
                    .Annotation<ReverseDocumentOrderInfo>()
                    .PreviousSibling;
                if (previousElement == null)
                    yield break;
                yield return previousElement;
                current = previousElement;
            }
        }

        public static string ToStringNewLineOnAttributes(this XElement element)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.NewLineOnAttributes = true;
            var stringBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(stringBuilder))
            using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                element.WriteTo(xmlWriter);
            return stringBuilder.ToString();
        }

        public static IEnumerable<XElement> DescendantsTrimmed(this XElement element,
            XName trimName)
        {
            return DescendantsTrimmed(element, e => e.Name == trimName);
        }

        public static IEnumerable<XElement> DescendantsTrimmed(this XElement element,
            Func<XElement, bool> predicate)
        {
            var iteratorStack = new Stack<IEnumerator<XElement>>();
            iteratorStack.Push(element.Elements().GetEnumerator());
            while (iteratorStack.Count > 0)
            {
                while (iteratorStack.Peek().MoveNext())
                {
                    var currentXElement = iteratorStack.Peek().Current;
                    if (predicate(currentXElement))
                    {
                        yield return currentXElement;
                        continue;
                    }
                    yield return currentXElement;
                    iteratorStack.Push(currentXElement.Elements().GetEnumerator());
                }
                iteratorStack.Pop();
            }
        }

        public static IEnumerable<TResult> Rollup<TSource, TResult>(
            this IEnumerable<TSource> source,
            TResult seed,
            Func<TSource, TResult, TResult> projection)
        {
            var nextSeed = seed;
            foreach (var src in source)
            {
                var projectedValue = projection(src, nextSeed);
                nextSeed = projectedValue;
                yield return projectedValue;
            }
        }

        public static IEnumerable<TSource> SequenceAt<TSource>(this TSource[] source, int index)
        {
            var i = index;
            while (i < source.Length)
                yield return source[i++];
        }
    }

    public class ReverseDocumentOrderInfo
    {
        public XElement PreviousSibling;
    }

    public class GroupOfAdjacent<TSource, TKey> : IEnumerable<TSource>, IGrouping<TKey, TSource>
    {
        public TKey Key { get; set; }
        private List<TSource> GroupList { get; set; }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TSource>)this).GetEnumerator();
        }

        IEnumerator<TSource>
            IEnumerable<TSource>.GetEnumerator()
        {
            foreach (var s in GroupList)
                yield return s;
        }

        public GroupOfAdjacent(List<TSource> source, TKey key)
        {
            GroupList = source;
            Key = key;
        }
    }


    public class XEntity : XText
    {
        public override void WriteTo(XmlWriter writer)
        {
            writer.WriteEntityRef(Value);
        }
        public XEntity(string value) : base(value) { }
    }
}
