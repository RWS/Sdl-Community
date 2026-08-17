using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public enum ModelType
    {
        Latency_Optimized,
        Quality_Optimized,
        Prefer_Quality_Optimized,
        Not_Supported
    }

    public static class ModelTypeEnumHelper
    {
        public static IEnumerable<ModelType> Values
        {
            get
            {
                var values = Enum.GetValues(typeof(ModelType));
                return values
                    .Cast<ModelType>().Take(values.Length - 1);
            }
        }
    }
}