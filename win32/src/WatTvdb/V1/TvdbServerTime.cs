using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WatTvdb.V1
{
    [XmlRoot(ElementName = "Items")]
    public class TvdbServerTime
    {
        [XmlElement]
        public Int64 Time { get; set; }
    }
}
