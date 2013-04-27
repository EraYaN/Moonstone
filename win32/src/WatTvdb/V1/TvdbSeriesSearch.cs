using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WatTvdb.V1
{
    [XmlRoot(ElementName = "Data")]
    public class TvdbSeriesSearchRoot
    {
        public TvdbSeriesSearchRoot()
        {
            Series = new List<TvdbSeriesSearchItem>();
        }
        
        [XmlElement(ElementName = "Series")]
        public List<TvdbSeriesSearchItem> Series { get; set; }
    }

    public class TvdbSeriesSearchItem
    {
        [XmlElement]
        public int id { get; set; }

        [XmlElement]
        public int seriesid { get; set; }

        [XmlElement]
        public string language { get; set; }

        [XmlElement]
        public string SeriesName { get; set; }

        [XmlElement]
        public string banner { get; set; }

        [XmlElement]
        public string Overview { get; set; }

        [XmlElement(ElementName = "FirstAired")]
        public string FirstAiredString
        {
            get { return FirstAired.HasValue ? FirstAired.Value.ToString("yyyy-MM-dd") : null; }
            set
            {
                DateTime d;
                if (DateTime.TryParse(value, out d))
                    FirstAired = d;
                else
                    FirstAired = null;
            }
        }

        [XmlIgnore]
        public DateTime? FirstAired { get; set; }

        [XmlElement]
        public string IMDB_ID { get; set; }

        [XmlElement]
        public string zap2it_id { get; set; }
    }
}
