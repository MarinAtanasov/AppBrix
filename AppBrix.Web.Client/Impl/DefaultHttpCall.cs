// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace AppBrix.Web.Client.Impl
{
    internal sealed class DefaultHttpCall : IHttpCall
    {
        #region Public and overriden methods
        public IHttpResponse<T> MakeCall<T>()
        {
            using (var client = new HttpClient())
            {
                var response = this.GetResponse(client);
                var headers = response.Headers;
                var content = response.Content;
                var contentHeaders = content.Headers;
                var contentValue = this.GetResponseContent<T>(content);
                return new DefaultHttpResponse<T>(
                    new DefaultHttpHeaders(headers),
                    new DefaultHttpContent<T>(contentValue, new DefaultHttpHeaders(contentHeaders)),
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    response.Version);
            }
        }

        public IHttpCall SetHeader(string header, params string[] values)
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

        public IHttpCall SetContent<T>(T content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            this.content = content;
            
            return this;
        }

        public IHttpCall SetContentHeader(string header, params string[] values)
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

        public IHttpCall SetMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentNullException("method");

            this.callMethod = method.ToUpperInvariant();
            return this;
        }

        public IHttpCall SetTimeout(TimeSpan timeout)
        {
            this.timeout = timeout;
            return this;
        }

        public IHttpCall SetUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            this.requestUrl = url;
            return this;
        } 

        public IHttpCall SetVersion(Version version)
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
                message.Content = this.CreateContent();
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

        private HttpContent CreateContent()
        {
            var type = this.content.GetType();

            HttpContent result;
            if (typeof(Stream).IsAssignableFrom(type))
            {
                result = new StreamContent((Stream)this.content);
            }
            else if (typeof(IEnumerable<KeyValuePair<string, string>>).IsAssignableFrom(type))
            {
                result = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)this.content);
            }
            else if (typeof(byte[]).IsAssignableFrom(type))
            {
                result = new ByteArrayContent((byte[])this.content);
            }
            else
            {
                result = new StringContent(this.content.ToString());
            }
            return result;
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
        private object content;
        private TimeSpan timeout;
        private string requestUrl;
        private Version httpMessageVersion;
        #endregion
    }
}
