using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.Community.InvoiceAndQuotes
{
    [Serializable]
    public class RateValue : ISerializable
    {
        public String ResourceToken { get; set; }
        public String Type { get; set; }
        public Decimal Rate { get; set; }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ResourceToken", ResourceToken);
            info.AddValue("Type", Type);
            info.AddValue("Rate", Rate.ToString(CultureInfo.InvariantCulture));
        }
    }
}
