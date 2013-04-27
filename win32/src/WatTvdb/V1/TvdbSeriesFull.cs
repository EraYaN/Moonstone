using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WatTvdb.V1
{
    [XmlRoot(ElementName="Data")]
    public class TvdbSeriesFull
    {
        public TvdbSeriesFull()
        {
            Episodes = new List<TvdbEpisode>();
        }

        [XmlElement]
        public TvdbSeriesBase Series { get; set; }

        [XmlElement(ElementName = "Episode")]
        public List<TvdbEpisode> Episodes { get; set; }
    }
}
