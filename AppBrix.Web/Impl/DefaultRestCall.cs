// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace AppBrix.Web.Impl
{
    internal sealed class DefaultRestCall : IRestCall
    {
        #region Public and overriden methods
        public IRestResponse<T> MakeCall<T>()
        {
            using (var client = new HttpClient())
            {
                var response = this.GetResponse(client);
                var headers = response.Headers;
                var content = response.Content;
                var contentHeaders = content.Headers;
                var contentValue = this.GetResponseContent<T>(content);
                return new DefaultRestResponse<T>(
                    new DefaultRestHeaders(headers),
                    new DefaultRestContent<T>(contentValue, new DefaultRestHeaders(contentHeaders)),
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    response.Version);
            }
        }

        public IRestCall SetHeader(string header, params string[] values)
        {
            if (string.IsNullOrEmpty(header))
                throw new ArgumentNullException("header");

            if (values == null || values.Length == 0)
            {
                if (this.headers.ContainsKey(header))
                {
                    this.headers.Remove(header);
                }
            }
            else
            {
                this.headers[header] = new List<string>(values);
            }
            return this;
        }

        public IRestCall SetContent<T>(T content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            object objContent = content;

            if (typeof(Stream).IsAssignableFrom(typeof(T)))
            {
                this.content = new StreamContent((Stream)objContent);
            }
            else if (typeof(IEnumerable<KeyValuePair<string, string>>).IsAssignableFrom(typeof(T)))
            {
                this.content = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)objContent);
            }
            else if (typeof(byte[]).IsAssignableFrom(typeof(T)))
            {
                this.content = new ByteArrayContent((byte[])objContent);
            }
            else
            {
                this.content = new StringContent(content.ToString());
            }
            return this;
        }

        public IRestCall SetContentHeader(string header, params string[] values)
        {
            if (string.IsNullOrEmpty(header))
                throw new ArgumentNullException("header");

            if (values == null || values.Length == 0)
            {
                if (this.contentHeaders.ContainsKey(header))
                {
                    this.contentHeaders.Remove(header);
                }
            }
            else
            {
                this.contentHeaders[header] = new List<string>(values);
            }
            return this;
        }

        public IRestCall SetMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentNullException("method");

            this.callMethod = method.ToUpperInvariant();
            return this;
        }

        public IRestCall SetTimeout(TimeSpan timeout)
        {
            this.timeout = timeout;
            return this;
        }

        public IRestCall SetUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            this.requestUrl = url;
            return this;
        } 

        public IRestCall SetVersion(Version version)
        {
            this.httpMessageVersion = version;
            return this;
        }
        #endregion

        #region Private methods
        private HttpResponseMessage GetResponse(HttpClient client)
        {
            if (this.timeout.TotalMilliseconds > 0)
                client.Timeout = this.timeout;

            var message = new HttpRequestMessage(new HttpMethod(this.callMethod), this.requestUrl);

            foreach (var header in this.headers)
            {
                if (message.Headers.Contains(header.Key))
                    message.Headers.Remove(header.Key);

                message.Headers.Add(header.Key, header.Value);
            }

            if (this.content != null)
            {
                message.Content = this.content;
                foreach (var header in this.contentHeaders)
                {
                    if (message.Content.Headers.Contains(header.Key))
                        message.Content.Headers.Remove(header.Key);

                    message.Content.Headers.Add(header.Key, header.Value);
                }
            }

            if (this.httpMessageVersion != null)
                message.Version = this.httpMessageVersion;

            return client.SendAsync(message).Result;
        }

        private T GetResponseContent<T>(HttpContent content)
        {
            object contentValue;

            var type = typeof(T);
            if (type == typeof(string))
                contentValue = content.ReadAsStringAsync().Result;
            else if (type == typeof(byte[]))
                contentValue = content.ReadAsByteArrayAsync().Result;
            else if (type == typeof(Stream))
                contentValue = content.ReadAsStreamAsync().Result;
            else
                throw new ArgumentException(string.Format("Unsupported type: {0}. Supported types are string, Stream and byte[].", typeof(T)));

            return (T)contentValue;
        }
        #endregion

        #region Private fields and constants
        private readonly IDictionary<string, ICollection<string>> headers = new Dictionary<string, ICollection<string>>();
        private readonly IDictionary<string, ICollection<string>> contentHeaders = new Dictionary<string, ICollection<string>>();
        private string callMethod = "GET";
        private HttpContent content;
        private TimeSpan timeout;
        private string requestUrl;
        private Version httpMessageVersion;
        #endregion
    }
}
