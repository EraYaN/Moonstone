﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WatTvdb.V1
{
    [XmlRoot(ElementName = "Actors")]
    public class TvdbActorRoot
    {
        public TvdbActorRoot()
        {
            Actors = new List<TvdbActor>();
        }

        [XmlElement(ElementName = "Actor")]
        public List<TvdbActor> Actors { get; set; }
    }

    public class TvdbActor
    {
        [XmlElement]
        public int id { get; set; }

        [XmlElement]
        public string Image { get; set; }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Role { get; set; }

        [XmlElement]
        public int SortOrder { get; set; }
    }
}
