using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatTvdb.Utilities
{
    internal partial class RequestGenerator
    {
        public const string REQUEST_GETMIRRORS = "{apikey}/mirrors.xml";
        public const string REQUEST_GETSERVERTIME = "Updates.php";
        public const string REQUEST_GETLANGUAGES = "{apikey}/languages.xml";
        public const string REQUEST_GETSEARCHSERIES = "GetSeries.php";
        public const string REQUEST_GETSERIESBASE = "api/{apikey}/series/{id}/{lang}.xml";
        public const string REQUEST_GETSERIESFULL = "api/{apikey}/series/{id}/all/{lang}.xml";
        public const string REQUEST_GETSERIESBANNERS = "api/{apikey}/series/{id}/banners.xml";
        public const string REQUEST_GETSERIESACTORS = "api/{apikey}/series/{id}/actors.xml";
        public const string REQUEST_GETEPISODES = "api/{apikey}/episodes/{id}/{lang}.xml";
        public const string REQUEST_GETSERIESEPISODES = "api/{apikey}/series/{id}/default/{season}/{episode}/{lang}.xml";
        public const string REQUEST_GETUPDATES = "api/{apikey}/updates/updates_{period}.xml";
        public const string REQUEST_GETUPDATESSINCE = "api/Updates.php?type=all&time={time}";
    }

    internal partial class RequestBuilder
    {
        public const string URLSEGMENT_APIKEY = "apikey";
        public const string URLSEGMENT_ID = "id";
        public const string URLSEGMENT_LANGUAGE = "lang";
        public const string URLSEGMENT_SEASON = "season";
        public const string URLSEGMENT_EPISODE = "episode";
        public const string URLSEGMENT_PERIOD = "period";
        public const string URLSEGMENT_TIME = "time";

        public const string PARAMETER_TYPE = "type";
        public const string PARAMETER_SERIESNAME = "seriesname";
    }
}
