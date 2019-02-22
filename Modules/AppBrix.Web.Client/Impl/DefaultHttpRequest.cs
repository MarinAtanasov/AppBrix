// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
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
            using (var response = await this.GetResponse(this.app.Get<IHttpClientFactory>().CreateClient(this.clientName)))
            {
                return new DefaultHttpResponse<string>(
                    new DefaultHttpHeaders(response.Headers.Concat(response.Content.Headers)),
                    null,
                    (int) response.StatusCode,
                    response.ReasonPhrase,
                    response.Version);
            }
        }

        public async Task<IHttpResponse<T>> Send<T>()
        {
            using (var response = await this.GetResponse(this.app.Get<IHttpClientFactory>().CreateClient(this.clientName)))
            {
                var responseContent = response.Content;
                var contentValue = await this.GetResponseContent<T>(responseContent);
                return new DefaultHttpResponse<T>(
                    new DefaultHttpHeaders(response.Headers.Concat(responseContent.Headers)),
                    contentValue,
                    (int) response.StatusCode,
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

        public IHttpRequest SetClientName(string name)
        {
            this.clientName = name;

            return this;
        }

        public IHttpRequest SetContent(object content)
        {
            this.content = content;
            
            return this;
        }

        public IHttpRequest SetMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentNullException(nameof(method));

            this.callMethod = method.ToUpperInvariant();
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
            var message = new HttpRequestMessage(new System.Net.Http.HttpMethod(this.callMethod), this.requestUrl);

            this.SetHeaders(message.Headers, this.headers.Where(x => !this.IsContentHeader(x.Key)));

            if (this.content != null)
            {
                message.Content = this.CreateContent();
                this.SetHeaders(message.Content.Headers, this.headers.Where(x => this.IsContentHeader(x.Key)));
            }

            if (this.httpMessageVersion != null)
                message.Version = this.httpMessageVersion;

            return await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
        }

        private void SetHeaders(HttpHeaders headers, IEnumerable<KeyValuePair<string, List<string>>> toAdd)
        {
            foreach (var header in toAdd)
            {
                if (headers.Contains(header.Key))
                    headers.Remove(header.Key);

                headers.Add(header.Key, header.Value);
            }
        }

        private bool IsContentHeader(string header)
        {
            var caps = header.ToUpperInvariant();
            switch (caps)
            {
                case "EXPIRES":
                case "LAST-MODIFIED":
                    return true;
                default:
                    return caps.StartsWith("CONTENT-");
            }
        }

        private HttpContent CreateContent()
        {
            switch (this.content)
            {
                case string s: return new StringContent(s);
                case byte[] b: return new ByteArrayContent(b);
                case Stream s: return new StreamContent(s);
                case HttpContent c: return c;
                case IEnumerable<KeyValuePair<string, string>> m: return new FormUrlEncodedContent(m);
                default: return new StringContent(JsonConvert.SerializeObject(this.content));
            }
        }

        private async Task<T> GetResponseContent<T>(HttpContent content)
        {
            var type = typeof(T);

            object contentValue;

            if (type == typeof(string))
                contentValue = await content.ReadAsStringAsync();
            else if (type == typeof(byte[]))
                contentValue = await content.ReadAsByteArrayAsync();
            else if (type == typeof(Stream))
                contentValue = await content.ReadAsStreamAsync();
            else if (type == typeof(NameValueCollection))
                contentValue = await content.ReadAsFormDataAsync();
            else
                contentValue = await content.ReadAsAsync<T>();;

            return (T)contentValue;
        }
        #endregion

        #region Private fields and constants
        private readonly Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();
        private readonly IApp app;
        private string callMethod = "GET";
        private string clientName;
        private object content;
        private string requestUrl;
        private Version httpMessageVersion;
        #endregion
    }
}
