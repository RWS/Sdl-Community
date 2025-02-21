using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

namespace TMProvider
{
    internal static class ServiceTypeConverters
    {
        #region Extension methods for the service types

 
        public static MemoQServerTypes.TMEntryModel ToMemoQServerTMEntryModel(this TranslationUnit tu)
        {
            MemoQServerTypes.TMEntryModel e = new MemoQServerTypes.TMEntryModel();
            e.Client = tu.Client;
            e.ContextID = tu.ContextID;
            e.Created = tu.Created;
            e.Creator = tu.Creator;
            e.Document = tu.Document;
            e.Domain = tu.Domain;
            e.FollowingSegment = tu.FollowingSegment;
            e.Key = tu.Key;
            e.Modified = tu.Modified;
            e.Modifier = tu.Modifier;
            e.PrecedingSegment = tu.PrecedingSegment;
            e.Project = tu.Project;
            e.SourceSegment = tu.SourceSegment;
            e.Subject = tu.Subject;
            e.TargetSegment = tu.TargetSegment;
            return e;
        }

        public static MemoQServerTypes.LookupSegmentOptions ToMemoQServerLookupOptions(this LookupSegmentRequest lookupSegmentRequest)
        {
            Mapper.CreateMap<LookupSegmentRequest, MemoQServerTypes.LookupSegmentOptions>().IgnoreAllNonExisting();
            MemoQServerTypes.LookupSegmentOptions newSr = Mapper.Map<MemoQServerTypes.LookupSegmentOptions>(lookupSegmentRequest);
            return newSr;
        }


        public static MemoQServerTypes.ConcordanceOptions ToMemoQServerConcOptions(this ConcordanceRequest concordanceRequest)
        {
            Mapper.CreateMap<ConcordanceRequest, MemoQServerTypes.ConcordanceOptions>().IgnoreAllNonExisting();
            MemoQServerTypes.ConcordanceOptions newCr = Mapper.Map<MemoQServerTypes.ConcordanceOptions>(concordanceRequest);
            return newCr;
        }

        #endregion

        /// <summary>
        /// Extension method to ignore all fields that don't exist in the source.
        /// Source: https://cangencer.wordpress.com/2011/06/08/auto-ignore-non-existing-properties-with-automapper/
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var existingMaps = Mapper.GetAllTypeMaps().First(x => x.SourceType.Equals(sourceType)
                && x.DestinationType.Equals(destinationType));
            foreach (var property in existingMaps.GetUnmappedPropertyNames())
            {
                expression.ForMember(property, opt => opt.Ignore());
            }
            return expression;
        }
    }
}
