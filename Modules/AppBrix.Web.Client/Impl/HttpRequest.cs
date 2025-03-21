﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Web.Client.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AppBrix.Web.Client.Impl;

internal sealed class HttpRequest : IHttpRequest
{
    #region Construction
    public HttpRequest(IApp app)
    {
        this.app = app;
    }
    #endregion

    #region Public and overriden methods
    public async Task<IHttpResponse> Send(CancellationToken token = default)
    {
        var client = this.app.GetHttpClientFactory().CreateClient(this.clientName);
        using var response = await this.GetResponse(client, token).ConfigureAwait(false);
        return new HttpResponse<string>(
            new HttpHeaders(response.Headers.Concat(response.Content.Headers)),
            string.Empty,
            (int)response.StatusCode,
            response.ReasonPhrase ?? string.Empty,
            response.Version);
    }

    public async Task<IHttpResponse<T>> Send<T>(CancellationToken token = default)
    {
        var client = this.app.GetHttpClientFactory().CreateClient(this.clientName);
        using var response = await this.GetResponse(client, token).ConfigureAwait(false);
        var responseContent = response.Content;
        var contentValue = await this.GetResponseContent<T>(responseContent, token).ConfigureAwait(false);
        return new HttpResponse<T>(
            new HttpHeaders(response.Headers.Concat(responseContent.Headers)),
            contentValue,
            (int)response.StatusCode,
            response.ReasonPhrase ?? string.Empty,
            response.Version);
    }

    public async Task<IStreamingHttpResponse> SendStream(CancellationToken token = default)
    {
        var client = this.app.GetHttpClientFactory().CreateClient(this.clientName);
        var response = await this.GetResponse(client, token).ConfigureAwait(false);
        var responseContent = response.Content;
        var contentValue = await this.GetResponseContent<Stream>(responseContent, token).ConfigureAwait(false);
        return new StreamingHttpResponse(
            new HttpHeaders(response.Headers.Concat(responseContent.Headers)),
            new StreamingHttpResponseContent(response, contentValue),
            (int)response.StatusCode,
            response.ReasonPhrase ?? string.Empty,
            response.Version);
    }

    public IHttpRequest SetHeader(string header, params string[] values)
    {
        if (string.IsNullOrEmpty(header))
            throw new ArgumentNullException(nameof(header));

        if (values.Length == 0)
            this.headers.Remove(header);
        else
            this.headers[header] = values;

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
    private async Task<HttpResponseMessage> GetResponse(HttpClient client, CancellationToken token = default)
    {
        using var message = new HttpRequestMessage(new System.Net.Http.HttpMethod(this.callMethod), this.requestUrl);

        this.SetHeaders(message.Headers, this.headers.Where(x => !this.IsContentHeader(x.Key)));

        if (this.content is not null)
        {
            message.Content = this.CreateContent(this.content);
            this.SetHeaders(message.Content.Headers, this.headers.Where(x => this.IsContentHeader(x.Key)));
        }

        if (this.httpMessageVersion is not null)
            message.Version = this.httpMessageVersion;

        return await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
    }

    private void SetHeaders(System.Net.Http.Headers.HttpHeaders headers, IEnumerable<KeyValuePair<string, string[]>> toAdd)
    {
        foreach (var header in toAdd)
        {
            headers.Remove(header.Key);
            headers.Add(header.Key, header.Value);
        }
    }

    private bool IsContentHeader(string header) => header.ToUpperInvariant() switch
    {
        "EXPIRES" => true,
        "LAST-MODIFIED" => true,
        _ => header.StartsWith("CONTENT-", StringComparison.OrdinalIgnoreCase)
    };

    private HttpContent CreateContent(object content) => content switch
    {
        string s => new StringContent(s),
        byte[] b => new ByteArrayContent(b),
        Stream s => new StreamContent(s),
        HttpContent c => c,
        IEnumerable<KeyValuePair<string, string>> m => new FormUrlEncodedContent(m),
        _ => new StringContent(JsonSerializer.Serialize(content, content.GetType(), this.app.Get<JsonSerializerOptions>()))
    };

    private async Task<T> GetResponseContent<T>(HttpContent content, CancellationToken token = default)
    {
        var type = typeof(T);

        object? contentValue;

        if (type == typeof(string))
            contentValue = await content.ReadAsStringAsync(token).ConfigureAwait(false);
        else if (type == typeof(byte[]))
            contentValue = await content.ReadAsByteArrayAsync(token).ConfigureAwait(false);
        else if (type == typeof(Stream))
            contentValue = await content.ReadAsStreamAsync(token).ConfigureAwait(false);
        else
            contentValue = await JsonSerializer.DeserializeAsync<T>(
                await content.ReadAsStreamAsync(token).ConfigureAwait(false),
                this.app.Get<JsonSerializerOptions>(),
                token
            ).ConfigureAwait(false);

        return contentValue is T value ? value : default!;
    }
    #endregion

    #region Private fields and constants
    private readonly Dictionary<string, string[]> headers = new Dictionary<string, string[]>();
    private readonly IApp app;
    private string callMethod = "GET";
    private string clientName = string.Empty;
    private object? content;
    private string? requestUrl;
    private Version? httpMessageVersion;
    #endregion
}
