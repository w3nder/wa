// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.HttpProvider
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class HttpProvider : IHttpProvider, IDisposable
  {
    private const int maxRedirects = 5;
    internal bool disposeHandler;
    internal HttpClient httpClient;
    internal HttpMessageHandler httpMessageHandler;

    public HttpProvider(ISerializer serializer = null)
      : this((HttpMessageHandler) null, true, serializer)
    {
    }

    public HttpProvider(
      HttpClientHandler httpClientHandler,
      bool disposeHandler,
      ISerializer serializer = null)
      : this((HttpMessageHandler) httpClientHandler, disposeHandler, serializer)
    {
    }

    internal HttpProvider(
      HttpMessageHandler httpMessageHandler,
      bool disposeHandler,
      ISerializer serializer)
    {
      this.disposeHandler = disposeHandler;
      HttpMessageHandler httpMessageHandler1 = httpMessageHandler;
      if (httpMessageHandler1 == null)
        httpMessageHandler1 = (HttpMessageHandler) new HttpClientHandler()
        {
          AllowAutoRedirect = false
        };
      this.httpMessageHandler = httpMessageHandler1;
      this.httpClient = new HttpClient(this.httpMessageHandler, this.disposeHandler);
      this.CacheControlHeader = new CacheControlHeaderValue()
      {
        NoCache = true,
        NoStore = true
      };
      this.Serializer = serializer ?? (ISerializer) new Microsoft.Graph.Serializer();
    }

    public CacheControlHeaderValue CacheControlHeader
    {
      get => this.httpClient.DefaultRequestHeaders.CacheControl;
      set => this.httpClient.DefaultRequestHeaders.CacheControl = value;
    }

    public TimeSpan OverallTimeout
    {
      get => this.httpClient.Timeout;
      set
      {
        try
        {
          this.httpClient.Timeout = value;
        }
        catch (InvalidOperationException ex)
        {
          throw new ServiceException(new Error()
          {
            Code = ErrorConstants.Codes.NotAllowed,
            Message = ErrorConstants.Messages.OverallTimeoutCannotBeSet
          }, (Exception) ex);
        }
      }
    }

    public ISerializer Serializer { get; private set; }

    public void Dispose()
    {
      if (this.httpClient == null)
        return;
      this.httpClient.Dispose();
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
      return this.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
    }

    public async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      HttpCompletionOption completionOption,
      CancellationToken cancellationToken)
    {
      HttpResponseMessage response = await this.SendRequestAsync(request, completionOption, cancellationToken);
      if (this.IsRedirect(response.StatusCode))
      {
        response = await this.HandleRedirect(response, completionOption, cancellationToken);
        if (response == null)
          throw new ServiceException(new Error()
          {
            Code = ErrorConstants.Codes.GeneralException,
            Message = ErrorConstants.Messages.LocationHeaderNotSetOnRedirect
          });
      }
      if (response.IsSuccessStatusCode || this.IsRedirect(response.StatusCode))
        return response;
      using (response)
      {
        ErrorResponse errorResponse = await this.ConvertErrorResponseAsync(response);
        Error error;
        if (errorResponse == null || errorResponse.Error == null)
        {
          if (response != null && response.StatusCode == HttpStatusCode.NotFound)
            error = new Error()
            {
              Code = ErrorConstants.Codes.ItemNotFound
            };
          else
            error = new Error()
            {
              Code = ErrorConstants.Codes.GeneralException,
              Message = ErrorConstants.Messages.UnexpectedExceptionResponse
            };
        }
        else
          error = errorResponse.Error;
        IEnumerable<string> values;
        if (string.IsNullOrEmpty(error.ThrowSite) && response.Headers.TryGetValues("X-ThrowSite", out values))
          error.ThrowSite = values.FirstOrDefault<string>();
        throw new ServiceException(error);
      }
    }

    internal async Task<HttpResponseMessage> HandleRedirect(
      HttpResponseMessage initialResponse,
      HttpCompletionOption completionOption,
      CancellationToken cancellationToken,
      int redirectCount = 0)
    {
      if (initialResponse.Headers.Location == (Uri) null)
        return (HttpResponseMessage) null;
      using (initialResponse)
      {
        using (HttpRequestMessage redirectRequest = new HttpRequestMessage(initialResponse.RequestMessage.Method, initialResponse.Headers.Location))
        {
          foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) initialResponse.RequestMessage.Headers)
            redirectRequest.Headers.Add(header.Key, header.Value);
          HttpResponseMessage response = await this.SendRequestAsync(redirectRequest, completionOption, cancellationToken);
          if (!this.IsRedirect(response.StatusCode))
            return response;
          if (++redirectCount > 5)
            throw new ServiceException(new Error()
            {
              Code = ErrorConstants.Codes.TooManyRedirects,
              Message = string.Format(ErrorConstants.Messages.TooManyRedirectsFormatString, (object) 5)
            });
          return await this.HandleRedirect(response, completionOption, cancellationToken, redirectCount);
        }
      }
    }

    internal async Task<HttpResponseMessage> SendRequestAsync(
      HttpRequestMessage request,
      HttpCompletionOption completionOption,
      CancellationToken cancellationToken)
    {
      HttpResponseMessage httpResponseMessage;
      try
      {
        httpResponseMessage = await this.httpClient.SendAsync(request, completionOption, cancellationToken);
      }
      catch (TaskCanceledException ex)
      {
        throw new ServiceException(new Error()
        {
          Code = ErrorConstants.Codes.Timeout,
          Message = ErrorConstants.Messages.RequestTimedOut
        }, (Exception) ex);
      }
      catch (Exception ex)
      {
        throw new ServiceException(new Error()
        {
          Code = ErrorConstants.Codes.GeneralException,
          Message = ErrorConstants.Messages.UnexpectedExceptionOnSend
        }, ex);
      }
      return httpResponseMessage;
    }

    private async Task<ErrorResponse> ConvertErrorResponseAsync(HttpResponseMessage response)
    {
      try
      {
        using (Stream stream = await response.Content.ReadAsStreamAsync())
          return this.Serializer.DeserializeObject<ErrorResponse>(stream);
      }
      catch (Exception ex)
      {
        return (ErrorResponse) null;
      }
    }

    private bool IsRedirect(HttpStatusCode statusCode)
    {
      return statusCode >= HttpStatusCode.MultipleChoices && statusCode < HttpStatusCode.BadRequest && statusCode != HttpStatusCode.NotModified;
    }
  }
}
