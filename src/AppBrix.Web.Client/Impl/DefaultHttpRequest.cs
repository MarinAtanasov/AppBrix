// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace AppBrix.Web.Client.Impl
{
    internal sealed class DefaultHttpRequest : IHttpRequest
    {
        #region Construction
        public DefaultHttpRequest(IApp app)
        {
            this.app = app;
        }
        #endregion

        #region Public and overriden methods
        public async Task<IHttpResponse> Send()
        {
            using (var client = app.GetFactory().Get<HttpClient>())
            {
                var response = await this.GetResponse(client);
                return new DefaultHttpResponse<string>(
                    new DefaultHttpHeaders(response.Headers),
                    null,
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    response.Version);
            }
        }

        public async Task<IHttpResponse<T>> Send<T>()
        {
            using (var client = app.GetFactory().Get<HttpClient>())
            {
                var response = await this.GetResponse(client);
                var httpResponseHeaders = response.Headers;
                var httpContent = response.Content;
                var httpContentHeaders = httpContent.Headers;
                var contentValue = await this.GetResponseContent<T>(httpContent);
                return new DefaultHttpResponse<T>(
                    new DefaultHttpHeaders(httpResponseHeaders),
                    new DefaultHttpContent<T>(contentValue, new DefaultHttpHeaders(httpContentHeaders)),
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    response.Version);
            }
        }

        public IHttpRequest SetHeader(string header, params string[] values)
        {
            if (string.IsNullOrEmpty(header))
                throw new ArgumentNullException(nameof(header));

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

        public IHttpRequest SetContent(object content)
        {
            this.content = content;
            
            return this;
        }

        public IHttpRequest SetContentHeader(string header, params string[] values)
        {
            if (string.IsNullOrEmpty(header))
                throw new ArgumentNullException(nameof(header));

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

        public IHttpRequest SetMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentNullException(nameof(method));

            this.callMethod = method.ToUpperInvariant();
            return this;
        }

        public IHttpRequest SetTimeout(TimeSpan timeout)
        {
            this.timeout = timeout;
            return this;
        }

        public IHttpRequest SetUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            this.requestUrl = url;
            return this;
        } 

        public IHttpRequest SetVersion(Version version)
        {
            this.httpMessageVersion = version;
            return this;
        }
        #endregion

        #region Private methods
        private async Task<HttpResponseMessage> GetResponse(HttpClient client)
        {
            if (this.timeout.TotalMilliseconds > 0)
                client.Timeout = this.timeout;

            var message = new HttpRequestMessage(new System.Net.Http.HttpMethod(this.callMethod), this.requestUrl);

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

            return await client.SendAsync(message);
        }

        private HttpContent CreateContent()
        {
            var typeInfo = this.content.GetType();

            HttpContent result;
            if (typeof(Stream).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                result = new StreamContent((Stream)this.content);
            }
            else if (typeof(IEnumerable<KeyValuePair<string, string>>).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                result = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)this.content);
            }
            else if (typeof(byte[]).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                result = new ByteArrayContent((byte[])this.content);
            }
            else
            {
                result = new StringContent(this.content.ToString());
            }
            return result;
        }

        private async Task<T> GetResponseContent<T>(HttpContent content)
        {
            object contentValue;

            var type = typeof(T);
            if (type == typeof(string))
                contentValue = await content.ReadAsStringAsync();
            else if (type == typeof(byte[]))
                contentValue = await content.ReadAsByteArrayAsync();
            else if (type == typeof(Stream))
                contentValue = await content.ReadAsStreamAsync();
            else
                throw new ArgumentException(
                    $"Unsupported type: {typeof(T)}. Supported types are {typeof(string).FullName}, {typeof(Stream).FullName} and {typeof(byte[]).FullName}.");

            return (T)contentValue;
        }
        #endregion

        #region Private fields and constants
        private readonly IDictionary<string, ICollection<string>> headers = new Dictionary<string, ICollection<string>>();
        private readonly IDictionary<string, ICollection<string>> contentHeaders = new Dictionary<string, ICollection<string>>();
        private readonly IApp app;
        private string callMethod = "GET";
        private object content;
        private TimeSpan timeout;
        private string requestUrl;
        private Version httpMessageVersion;
        #endregion
    }
}
