using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using RestSharp;
using RestSharp.Deserializers;

namespace WatTvdb.V1
{
    public partial class Tvdb
    {
        private void ProcessAsyncRequest<T>(RestRequest request, Action<TvdbAsyncResult<T>> callback)
            where T : new()
        {
            ProcessAsyncRequest<T>(BASE_URL, request, callback);
        }

        private void ProcessAsyncRequest<T>(string url, RestRequest request, Action<TvdbAsyncResult<T>> callback)
            where T : new()
        {
            var client = new RestClient(url);
            client.AddHandler("text/xml", Deserializer);

            if (Timeout.HasValue)
                client.Timeout = Timeout.Value;

#if !WINDOWS_PHONE
            if (Proxy != null)
                client.Proxy = Proxy;
#endif

            Error = null;

            var asyncHandle = client.ExecuteAsync<T>(request, resp =>
                {
                    var result = new TvdbAsyncResult<T>
                    {
                        Data = resp.Data != null ? resp.Data : default(T),
                        UserState = request.UserState
                    };

                    ResponseContent = resp.Content;
                    ResponseHeaders = resp.Headers.ToDictionary(k => k.Name, v => v.Value);

                    if (resp.ResponseStatus != ResponseStatus.Completed && resp.ErrorException != null)
                        throw resp.ErrorException;

                    callback(result);
                });
        }

        public void GetMirrors(object UserState, Action<TvdbAsyncResult<TvdbMirrors>> callback)
        {
            //ProcessAsyncRequest<TvdbMirrors>(BuildGetMirrorsRequest(UserState), callback);
            ProcessAsyncRequest<TvdbMirrors>(Generator.GetMirrors(UserState), callback);
        }

        /// <summary>
        /// http://www.thetvdb.com/api/Updates.php?type=none
        /// </summary>
        /// <returns></returns>
        public void GetServerTime(object UserState, Action<TvdbAsyncResult<TvdbServerTime>> callback)
        {
            //ProcessAsyncRequest<TvdbServerTime>(BuildGetServerTimeRequest(UserState), callback);
            ProcessAsyncRequest<TvdbServerTime>(Generator.GetServerTime(UserState), callback);
        }

        /// <summary>
        /// http://www.thetvdb.com/api/{apikey}/languages.xml
        /// </summary>
        /// <returns></returns>
        public void GetLanguages(object UserState, Action<TvdbAsyncResult<List<TvdbLanguage>>> callback)
        {
            //ProcessAsyncRequest<TvdbLanguagesRoot>(BuildGetLanguagesRequest(UserState), root =>
            ProcessAsyncRequest<TvdbLanguagesRoot>(Generator.GetLanguages(UserState), root =>
                {
                    callback(new TvdbAsyncResult<List<TvdbLanguage>>
                    {
                        UserState = root.UserState,
                        Data = root.Data == null ? null : root.Data.Languages
                    });
                });
        }


        /// <summary>
        /// http://www.thetvdb.com/api/GetSeries.php?seriesname={series}
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public void SearchSeries(string search, object UserState, Action<TvdbAsyncResult<List<TvdbSeriesSearchItem>>> callback)
        {
            //ProcessAsyncRequest<TvdbSeriesSearchRoot>(BuildGetSearchSeriesRequest(search, UserState), root =>
            ProcessAsyncRequest<TvdbSeriesSearchRoot>(Generator.GetSearchSeries(search, UserState), root =>
                {
                    callback(new TvdbAsyncResult<List<TvdbSeriesSearchItem>>
                    {
                        UserState = root.UserState,
                        Data = root.Data == null ? null : root.Data.Series
                    });
                });
        }

        /// <summary>
        /// http://thetvdb.com/api/{apikey}/series/79349/en.xml
        /// </summary>
        /// <param name="XMLMirror"></param>
        /// <param name="SeriesId"></param>
        /// <param name="Language"></param>
        /// <returns></returns>
        public void GetSeriesBaseRecord(string XMLMirror, int SeriesId, string Language, object UserState, Action<TvdbAsyncResult<TvdbSeriesBase>> callback)
        {
            if (string.IsNullOrEmpty(Language))
                Language = "en";

            //ProcessAsyncRequest<TvdbSeriesRecordRoot>(XMLMirror, BuildGetSeriesBaseRecordRequest(SeriesId, Language, UserState), root =>
            ProcessAsyncRequest<TvdbSeriesRecordRoot>(XMLMirror, Generator.GetSeriesBaseRecord(SeriesId, Language, UserState), root =>
                {
                    callback(new TvdbAsyncResult<TvdbSeriesBase>
                    {
                        UserState = root.UserState,
                        Data = root.Data == null ? null : root.Data.Series
                    });
                });
        }

        public void GetSeriesBaseRecord(string XMLMirror, int SeriesId, object UserState, Action<TvdbAsyncResult<TvdbSeriesBase>> callback)
        {
            GetSeriesBaseRecord(XMLMirror, SeriesId, null, UserState, callback);
        }

        /// <summary>
        /// http://thetvdb.com/api/{apikey}/series/79349/all/en.xml
        /// </summary>
        /// <param name="XMLMirror"></param>
        /// <param name="SeriesId"></param>
        /// <param name="Language"></param>
        /// <returns></returns>
        public void GetSeriesFullRecord(string XMLMirror, int SeriesId, string Language, object UserState, Action<TvdbAsyncResult<TvdbSeriesFull>> callback)
        {
            if (string.IsNullOrEmpty(Language))
                Language = "en";

            //ProcessAsyncRequest<TvdbSeriesFull>(XMLMirror, BuildGetSeriesFullRecordRequest(SeriesId, Language, UserState), callback);
            ProcessAsyncRequest<TvdbSeriesFull>(XMLMirror, Generator.GetSeriesFullRecord(SeriesId, Language, UserState), callback);
        }

        public void GetSeriesFullRecord(string MirrorPath, int SeriesId, object UserState, Action<TvdbAsyncResult<TvdbSeriesFull>> callback)
        {
            GetSeriesFullRecord(MirrorPath, SeriesId, null, UserState, callback);
        }

        /// <summary>
        /// http://thetvdb.com/api/{apikey}/series/79349/banners.xml
        /// </summary>
        /// <param name="XMLMirror"></param>
        /// <param name="SeriesId"></param>
        /// <returns></returns>
        public void GetSeriesBanners(string XMLMirror, int SeriesId, object UserState, Action<TvdbAsyncResult<List<TvdbBanner>>> callback)
        {
            //ProcessAsyncRequest<TvdbBannerRoot>(XMLMirror, BuildGetSeriesBannersRequest(SeriesId, UserState), root =>
            ProcessAsyncRequest<TvdbBannerRoot>(XMLMirror, Generator.GetSeriesBanners(SeriesId, UserState), root =>
                {
                    callback(new TvdbAsyncResult<List<TvdbBanner>>
                    {
                        UserState = root.UserState,
                        Data = root.Data == null ? null : root.Data.Banners
                    });
                });
        }

        /// <summary>
        /// http://thetvdb.com/api/{apikey}/series/79349/actors.xml
        /// </summary>
        /// <param name="XMLMirror"></param>
        /// <param name="SeriesId"></param>
        /// <returns></returns>
        public void GetSeriesActors(string XMLMirror, int SeriesId, object UserState, Action<TvdbAsyncResult<List<TvdbActor>>> callback)
        {
            //ProcessAsyncRequest<TvdbActorRoot>(XMLMirror, BuildGetSeriesActorsRequest(SeriesId, UserState), root =>
            ProcessAsyncRequest<TvdbActorRoot>(XMLMirror, Generator.GetSeriesActors(SeriesId, UserState), root =>
                {
                    callback(new TvdbAsyncResult<List<TvdbActor>>
                    {
                        UserState = root.UserState,
                        Data = root.Data == null ? null : root.Data.Actors
                    });
                });
        }

        public void GetEpisode(string XMLMirror, int EpisodeId, string Language, object UserState, Action<TvdbAsyncResult<TvdbEpisode>> callback)
        {
            if (string.IsNullOrEmpty(Language))
                Language = "en";

            //ProcessAsyncRequest<TvdbEpisodeRoot>(XMLMirror, BuildGetEpisodeRequest(EpisodeId, Language, UserState), root =>
            ProcessAsyncRequest<TvdbEpisodeRoot>(XMLMirror, Generator.GetEpisodes(EpisodeId, Language, UserState), root =>
                {
                    callback(new TvdbAsyncResult<TvdbEpisode>
                    {
                        UserState = root.UserState,
                        Data = root.Data == null ? null : root.Data.Episode
                    });
                });
        }

        public void GetEpisode(string XMLMirror, int EpisodeId, object UserState, Action<TvdbAsyncResult<TvdbEpisode>> callback)
        {
            GetEpisode(XMLMirror, EpisodeId, null, UserState, callback);
        }

        public void GetSeriesEpisode(string XMLMirror, int SeriesId, int SeasonNum, int EpisodeNum, string Language, object UserState, Action<TvdbAsyncResult<TvdbEpisode>> callback)
        {
            if (string.IsNullOrEmpty(Language))
                Language = "en";

            //ProcessAsyncRequest<TvdbEpisodeRoot>(XMLMirror, BuildGetSeriesEpisodeRequest(SeriesId, SeasonNum, EpisodeNum, Language, UserState), root =>
            ProcessAsyncRequest<TvdbEpisodeRoot>(XMLMirror, Generator.GetSeriesEpisodes(SeriesId, SeasonNum, EpisodeNum, Language, UserState), root =>
                {
                    callback(new TvdbAsyncResult<TvdbEpisode>
                    {
                        UserState = root.UserState,
                        Data = root.Data == null ? null : root.Data.Episode
                    });
                });
        }

        public void GetSeriesEpisode(string XMLMirror, int SeriesId, int SeasonNum, int EpisodeNum, object UserState, Action<TvdbAsyncResult<TvdbEpisode>> callback)
        {
            GetSeriesEpisode(XMLMirror, SeriesId, SeasonNum, EpisodeNum, null, UserState, callback);
        }

        public void GetUpdates(string XMLMirror, TvdbUpdatePeriod Period, object UserState, Action<TvdbAsyncResult<TvdbUpdates>> callback)
        {
            //ProcessAsyncRequest<TvdbUpdates>(XMLMirror, BuildGetUpdatesRequest(Period, UserState), callback);
            ProcessAsyncRequest<TvdbUpdates>(XMLMirror, Generator.GetUpdates(Period, UserState), callback);
        }

        public void GetUpdatesSince(string XMLMirror, Int64 LastTime, object UserState, Action<TvdbAsyncResult<TvdbUpdateItems>> callback)
        {
            //ProcessAsyncRequest<TvdbUpdateItems>(XMLMirror, BuildGetUpdatesSinceRequest(LastTime, UserState), callback);
            ProcessAsyncRequest<TvdbUpdateItems>(XMLMirror, Generator.GetUpdatesSince(LastTime, UserState), callback);
        }
    }
}
