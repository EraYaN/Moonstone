using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;

namespace WatTvdb.Utilities
{
    internal partial class RequestBuilder
    {
        public RequestBuilder(string resource, Method method)
        {
            Request = new RestRequest(resource, method);
        }

        #region Properties

        private RestRequest Request { get; set; }

        private Method Method
        {
            get { return Request.Method; }
            set { Request.Method = value; }
        }

        private object UserState
        {
            get { return Request.UserState; }
            set { if (value != null) Request.UserState = value; }
        }

        #endregion

        public RestRequest GetRequest()
        {
            return Request;
        }

        public RequestBuilder SetUserState(object userState)
        {
            UserState = userState;
            return this;
        }

        public RequestBuilder AddParameter(string name, string value)
        {
            if (string.IsNullOrEmpty(value)) return this;
            value = value.EscapeString();

            Request.AddParameter(name, value);
            return this;
        }

        public RequestBuilder AddUrlSegment(string name, string value)
        {
            if (string.IsNullOrEmpty(value)) return this;
            value = value.EscapeString();

            Request.AddUrlSegment(name, value);
            return this;
        }
    }
}
