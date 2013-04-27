using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using WatTvdb.V1;

namespace WatTvdb.Utilities
{
    internal partial class RequestGenerator
    {
        #region Properties

        private Method Method { get; set; }
        private string ApiKey { get; set; }

        #endregion

        public RequestGenerator(string apiKey, Method method = RestSharp.Method.GET)
        {
            Method = method;
            ApiKey = apiKey;
        }

        private RequestBuilder GetBuilder(string request)
        {
            return new RequestBuilder(request, Method);
        }

        internal RestRequest GetMirrors(object userState = null)
        {
            return GetBuilder(REQUEST_GETMIRRORS)
                .SetUserState(userState)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_APIKEY, ApiKey)
                .GetRequest();
        }

        internal RestRequest GetServerTime(object userState = null)
        {
            return GetBuilder(REQUEST_GETSERVERTIME)
                .SetUserState(userState)
                .AddParameter(RequestBuilder.PARAMETER_TYPE, "none")
                .GetRequest();
        }

        internal RestRequest GetLanguages(object userState = null)
        {
            return GetBuilder(REQUEST_GETLANGUAGES)
                .SetUserState(userState)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_APIKEY, ApiKey)
                .GetRequest();
        }

        internal RestRequest GetSearchSeries(string search, object userState = null)
        {
            return GetBuilder(REQUEST_GETSEARCHSERIES)
                .SetUserState(userState)
                .AddParameter(RequestBuilder.PARAMETER_SERIESNAME, search)
                .GetRequest();
        }

        internal RestRequest GetSeriesBaseRecord(int SeriesId, string Language, object userState = null)
        {
            return GetBuilder(REQUEST_GETSERIESBASE)
                .SetUserState(userState)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_APIKEY, ApiKey)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_ID, SeriesId.ToString())
                .AddUrlSegment(RequestBuilder.URLSEGMENT_LANGUAGE, Language)
                .GetRequest();
        }

        internal RestRequest GetSeriesFullRecord(int SeriesId, string Language, object userState = null)
        {
            return GetBuilder(REQUEST_GETSERIESFULL)
                .SetUserState(userState)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_APIKEY, ApiKey)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_ID, SeriesId.ToString())
                .AddUrlSegment(RequestBuilder.URLSEGMENT_LANGUAGE, Language)
                .GetRequest();
        }

        internal RestRequest GetSeriesBanners(int SeriesId, object userState = null)
        {
            return GetBuilder(REQUEST_GETSERIESBANNERS)
                .SetUserState(userState)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_APIKEY, ApiKey)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_ID, SeriesId.ToString())
                .GetRequest();
        }

        internal RestRequest GetSeriesActors(int SeriesId, object userState = null)
        {
            return GetBuilder(REQUEST_GETSERIESACTORS)
                .SetUserState(userState)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_APIKEY, ApiKey)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_ID, SeriesId.ToString())
                .GetRequest();
        }

        internal RestRequest GetEpisodes(int EpisodeId, string Language, object userState = null)
        {
            return GetBuilder(REQUEST_GETEPISODES)
                .SetUserState(userState)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_APIKEY, ApiKey)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_ID, EpisodeId.ToString())
                .AddUrlSegment(RequestBuilder.URLSEGMENT_LANGUAGE, Language)
                .GetRequest();
        }

        internal RestRequest GetSeriesEpisodes(int SeriesId, int SeasonNum, int EpisodeNum, string Language, object userState = null)
        {
            return GetBuilder(REQUEST_GETSERIESEPISODES)
                .SetUserState(userState)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_APIKEY, ApiKey)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_ID, SeriesId.ToString())
                .AddUrlSegment(RequestBuilder.URLSEGMENT_SEASON, SeasonNum.ToString())
                .AddUrlSegment(RequestBuilder.URLSEGMENT_EPISODE, EpisodeNum.ToString())
                .GetRequest();
        }

        internal RestRequest GetUpdates(TvdbUpdatePeriod Period, object userState = null)
        {
            return GetBuilder(REQUEST_GETUPDATES)
                .SetUserState(userState)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_APIKEY, ApiKey)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_PERIOD, Period.ToString())
                .GetRequest();
        }

        internal RestRequest GetUpdatesSince(Int64 LastTime, object userState = null)
        {
            return GetBuilder(REQUEST_GETUPDATESSINCE)
                .SetUserState(userState)
                .AddUrlSegment(RequestBuilder.URLSEGMENT_TIME, LastTime.ToString())
                .GetRequest();
        }
    }
}
