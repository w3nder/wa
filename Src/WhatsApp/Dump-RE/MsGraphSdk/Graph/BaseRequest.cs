// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.BaseRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class BaseRequest : IBaseRequest
  {
    protected string sdkVersionHeaderName;
    protected string sdkVersionHeaderValue;

    public BaseRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options = null)
    {
      this.Method = "GET";
      this.Client = client;
      this.Headers = (IList<HeaderOption>) new List<HeaderOption>();
      this.QueryOptions = (IList<QueryOption>) new List<QueryOption>();
      this.RequestUrl = this.InitializeUrl(requestUrl);
      this.sdkVersionHeaderName = "SdkVersion";
      this.SdkVersionHeaderPrefix = "graph";
      if (options == null)
        return;
      IEnumerable<HeaderOption> collection1 = options.OfType<HeaderOption>();
      if (collection1 != null)
        ((List<HeaderOption>) this.Headers).AddRange(collection1);
      IEnumerable<QueryOption> collection2 = options.OfType<QueryOption>();
      if (collection2 == null)
        return;
      ((List<QueryOption>) this.QueryOptions).AddRange(collection2);
    }

    public string ContentType { get; set; }

    public IList<HeaderOption> Headers { get; private set; }

    public IBaseClient Client { get; private set; }

    public string Method { get; set; }

    public IList<QueryOption> QueryOptions { get; set; }

    public string RequestUrl { get; internal set; }

    protected string SdkVersionHeaderPrefix { get; set; }

    public async Task SendAsync(
      object serializableObject,
      CancellationToken cancellationToken,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
    {
      using (await this.SendRequestAsync(serializableObject, cancellationToken, completionOption).ConfigureAwait(false))
        ;
    }

    public async Task<T> SendAsync<T>(
      object serializableObject,
      CancellationToken cancellationToken,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
    {
      using (HttpResponseMessage response = await this.SendRequestAsync(serializableObject, cancellationToken, completionOption).ConfigureAwait(false))
        return response.Content != null ? this.Client.HttpProvider.Serializer.DeserializeObject<T>(await response.Content.ReadAsStringAsync()) : default (T);
    }

    public async Task<Stream> SendStreamRequestAsync(
      object serializableObject,
      CancellationToken cancellationToken,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
    {
      return await (await this.SendRequestAsync(serializableObject, cancellationToken, completionOption).ConfigureAwait(false)).Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> SendRequestAsync(
      object serializableObject,
      CancellationToken cancellationToken,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
    {
      if (string.IsNullOrEmpty(this.RequestUrl))
        throw new ServiceException(new Error()
        {
          Code = ErrorConstants.Codes.InvalidRequest,
          Message = ErrorConstants.Messages.RequestUrlMissing
        });
      if (this.Client.AuthenticationProvider == null)
        throw new ServiceException(new Error()
        {
          Code = ErrorConstants.Codes.InvalidRequest,
          Message = ErrorConstants.Messages.AuthenticationProviderMissing
        });
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage request = this.GetHttpRequestMessage())
      {
        await this.AuthenticateRequest(request).ConfigureAwait(false);
        if (serializableObject != null)
        {
          request.Content = !(serializableObject is Stream content) ? (HttpContent) new StringContent(this.Client.HttpProvider.Serializer.SerializeObject(serializableObject)) : (HttpContent) new StreamContent(content);
          if (!string.IsNullOrEmpty(this.ContentType))
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(this.ContentType);
        }
        httpResponseMessage = await this.Client.HttpProvider.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);
      }
      return httpResponseMessage;
    }

    public HttpRequestMessage GetHttpRequestMessage()
    {
      HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(this.Method), this.RequestUrl + this.BuildQueryString());
      this.AddHeadersToRequest(request);
      return request;
    }

    internal string BuildQueryString()
    {
      if (this.QueryOptions == null)
        return (string) null;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (QueryOption queryOption in (IEnumerable<QueryOption>) this.QueryOptions)
      {
        if (stringBuilder.Length == 0)
          stringBuilder.AppendFormat("?{0}={1}", (object) queryOption.Name, (object) queryOption.Value);
        else
          stringBuilder.AppendFormat("&{0}={1}", (object) queryOption.Name, (object) queryOption.Value);
      }
      return stringBuilder.ToString();
    }

    private void AddHeadersToRequest(HttpRequestMessage request)
    {
      if (this.Headers != null)
      {
        foreach (HeaderOption header in (IEnumerable<HeaderOption>) this.Headers)
          request.Headers.TryAddWithoutValidation(header.Name, header.Value);
      }
      if (string.IsNullOrEmpty(this.sdkVersionHeaderValue))
      {
        Version version = IntrospectionExtensions.GetTypeInfo(this.GetType()).Assembly.GetName().Version;
        this.sdkVersionHeaderValue = string.Format("{0}-dotnet-{1}.{2}.{3}", (object) this.SdkVersionHeaderPrefix, (object) version.Major, (object) version.Minor, (object) version.Build);
      }
      request.Headers.Add(this.sdkVersionHeaderName, this.sdkVersionHeaderValue);
    }

    private Task AuthenticateRequest(HttpRequestMessage request)
    {
      return this.Client.AuthenticationProvider.AuthenticateRequestAsync(request);
    }

    private string InitializeUrl(string requestUrl)
    {
      Uri uri = !string.IsNullOrEmpty(requestUrl) ? new Uri(requestUrl) : throw new ServiceException(new Error()
      {
        Code = ErrorConstants.Codes.InvalidRequest,
        Message = ErrorConstants.Messages.BaseUrlMissing
      });
      if (!string.IsNullOrEmpty(uri.Query))
      {
        string str = uri.Query;
        if (str[0] == '?')
          str = str.Substring(1);
        foreach (QueryOption queryOption in ((IEnumerable<string>) str.Split('&')).Select<string, QueryOption>((Func<string, QueryOption>) (queryValue =>
        {
          string[] strArray = queryValue.Split('=');
          return new QueryOption(strArray[0], strArray.Length > 1 ? strArray[1] : string.Empty);
        })))
          this.QueryOptions.Add(queryOption);
      }
      return new UriBuilder(uri) { Query = string.Empty }.ToString();
    }
  }
}
