using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sdl.Community.Structures.QualityMetrics;

namespace Sdl.Community.Structures.Configuration
{
    [Serializable]
    public class QualityMetricGroupSettings : ICloneable
    {
        public List<QualityMetricGroup> QualityMetricGroups { get; set; }


        public QualityMetricGroupSettings()
        {
            QualityMetricGroups = new List<QualityMetricGroup>();
        }

        public object Clone()
        {
            var metricSettings = new QualityMetricGroupSettings {QualityMetricGroups = new List<QualityMetricGroup>()};

            foreach (var qualityMetricGroup in QualityMetricGroups)
                metricSettings.QualityMetricGroups.Add((QualityMetricGroup)qualityMetricGroup.Clone());

            return metricSettings;
        }
    }




    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable
    {

        #region IXmlSerializable Members

        public XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(XmlReader reader)
        {

            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));


            var wasEmpty = reader.IsEmptyElement;

            reader.Read();

            if (wasEmpty)

                return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {

                reader.ReadStartElement("item");

                reader.ReadStartElement("key");

                var key = (TKey)keySerializer.Deserialize(reader);

                reader.ReadEndElement();

                reader.ReadStartElement("value");

                var value = (TValue)valueSerializer.Deserialize(reader);

                reader.ReadEndElement();
                Add(key, value);

                reader.ReadEndElement();

                reader.MoveToContent();

            }

            reader.ReadEndElement();

        }



        public void WriteXml(XmlWriter writer)
        {
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));


            foreach (var key in Keys)
            {

                writer.WriteStartElement("item");

                writer.WriteStartElement("key");

                keySerializer.Serialize(writer, key);

                writer.WriteEndElement();
                writer.WriteStartElement("value");

                var value = this[key];

                valueSerializer.Serialize(writer, value);

                writer.WriteEndElement();

                writer.WriteEndElement();

            }

        }

        #endregion

    }
}
