// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaHttpProvider
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Graph;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class WaHttpProvider : IHttpProvider, IDisposable
  {
    private const int maxRedirects = 5;
    internal bool disposeHandler;
    internal HttpMessageHandler httpMessageHandler;

    public WaHttpProvider(ISerializer serializer = null)
      : this((HttpMessageHandler) null, true, serializer)
    {
      Log.d("wahttp", "http provider created");
      this.Serializer = serializer ?? (ISerializer) new Microsoft.Graph.Serializer();
    }

    private WaHttpProvider(
      HttpClientHandler httpClientHandler,
      bool disposeHandler,
      ISerializer serializer = null)
      : this((HttpMessageHandler) httpClientHandler, disposeHandler, serializer)
    {
    }

    private WaHttpProvider(
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
      this.Serializer = serializer ?? (ISerializer) new Microsoft.Graph.Serializer();
    }

    public ISerializer Serializer { get; private set; }

    public void Dispose() => Log.d("wahttp", "http provider disposed");

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
      TaskCompletionSource<HttpResponseMessage> result = new TaskCompletionSource<HttpResponseMessage>();
      NativeWeb.Create<HttpResponseMessage>(NativeWeb.Options.CacheDns, (Action<IWebRequest, IObserver<HttpResponseMessage>>) ((req, observer) =>
      {
        using (cancellationToken.Register((Action) (() => req.Cancel())))
        {
          HttpResponseMessage response = new HttpResponseMessage();
          MemoryStream stream = new MemoryStream();
          response.RequestMessage = request;
          response.Content = (HttpContent) new WaHttpContent((Stream) stream);
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(string.Join("\r\n", request.Headers.Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (kv => string.Format("{0}: {1}", (object) kv.Key, (object) string.Join(" ", kv.Value))))));
          if (request.Content != null)
          {
            foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) request.Content.Headers)
            {
              if (stringBuilder.Length > 0)
                stringBuilder.Append("\r\n");
              stringBuilder.Append(string.Format("{0}: {1}", (object) header.Key, (object) string.Join(" ", header.Value)));
            }
            long? contentLength = request.Content.Headers.ContentLength;
            if (contentLength.HasValue)
            {
              if (stringBuilder.Length > 0)
                stringBuilder.Append("\r\n");
              stringBuilder.Append(string.Format("Content-Length: {0}", (object) contentLength.Value));
            }
          }
          string str1 = stringBuilder.ToString();
          IWebRequest req1 = req;
          string url = request.RequestUri.ToString();
          NativeWeb.Callback callbackObject = new NativeWeb.Callback();
          callbackObject.OnBeginResponse = (Action<int, string>) ((code, headers) =>
          {
            response.StatusCode = (HttpStatusCode) code;
            foreach (KeyValuePair<string, string> header in NativeWeb.ParseHeaders(headers))
            {
              string key = header.Key;
              string str2 = header.Value;
              if (!response.Headers.TryAddWithoutValidation(key, str2) && !response.Content.Headers.TryAddWithoutValidation(key, str2))
                Log.p("wahttp", "Unknown header: {0}", (object) key);
            }
          });
          callbackObject.OnWrite = (Action<Action<byte[], int, int>>) (a =>
          {
            HttpContent content = request.Content;
            if (content == null)
              return;
            using (Stream result2 = content.ReadAsStreamAsync().Result)
            {
              byte[] buffer = new byte[4096];
              int num;
              do
              {
                num = result2.Read(buffer, 0, buffer.Length);
                if (num > 0)
                  a(buffer, 0, num);
              }
              while (num > 0);
            }
          });
          callbackObject.OnBytesIn = (Action<byte[]>) (bytes => stream.Write(bytes, 0, bytes.Length));
          callbackObject.OnEndResponse = (Action) (() => observer.OnNext(response));
          string upperInvariant = request.Method.ToString().ToUpperInvariant();
          string defaultUserAgent = req.GetDefaultUserAgent();
          string headers1 = str1;
          req1.Open(url, (IWebCallback) callbackObject, upperInvariant, defaultUserAgent, headers1);
        }
      })).Subscribe<HttpResponseMessage>((Action<HttpResponseMessage>) (response => result.SetResult(response)), (Action<Exception>) (exInner =>
      {
        if (exInner is TaskCanceledException)
          result.SetException((Exception) new ServiceException(new Error()
          {
            Code = ErrorConstants.Codes.Timeout,
            Message = ErrorConstants.Messages.RequestTimedOut
          }, exInner));
        if (exInner.GetHResult() == 2147954402U || exInner.GetHResult() == 2147954429U)
          result.SetException((Exception) new ServiceException(new Error()
          {
            Code = ErrorConstants.Codes.Timeout,
            Message = ErrorConstants.Messages.RequestTimedOut
          }, exInner));
        else if (exInner.GetHResult() == 2147943635U || exInner.GetHResult() == 2147943636U || cancellationToken.IsCancellationRequested)
          result.SetException((Exception) new OperationCanceledException("aborted", exInner, cancellationToken));
        else
          result.SetException((Exception) new ServiceException(new Error()
          {
            Code = ErrorConstants.Codes.GeneralException,
            Message = ErrorConstants.Messages.UnexpectedExceptionOnSend
          }, exInner));
      }));
      return await result.Task;
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
