// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.UploadChunkRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class UploadChunkRequest : BaseRequest, IUploadChunkRequest, IBaseRequest
  {
    public long RangeBegin { get; private set; }

    public long RangeEnd { get; private set; }

    public long TotalSessionLength { get; private set; }

    public int RangeLength => (int) (this.RangeEnd - this.RangeBegin + 1L);

    public UploadChunkRequest(
      string sessionUrl,
      IBaseClient client,
      IEnumerable<Option> options,
      long rangeBegin,
      long rangeEnd,
      long totalSessionLength)
      : base(sessionUrl, client, options)
    {
      this.RangeBegin = rangeBegin;
      this.RangeEnd = rangeEnd;
      this.TotalSessionLength = totalSessionLength;
    }

    public Task<UploadChunkResult> PutAsync(Stream stream)
    {
      return this.PutAsync(stream, CancellationToken.None);
    }

    public virtual async Task<UploadChunkResult> PutAsync(
      Stream stream,
      CancellationToken cancellationToken)
    {
      this.Method = "PUT";
      using (HttpResponseMessage response = await this.SendRequestAsync(stream, cancellationToken, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
      {
        if (response.Content != null)
        {
          string inputString = await response.Content.ReadAsStringAsync();
          if (response.StatusCode != HttpStatusCode.Created)
          {
            if (response.StatusCode != HttpStatusCode.OK)
            {
              try
              {
                return new UploadChunkResult()
                {
                  UploadSession = this.Client.HttpProvider.Serializer.DeserializeObject<UploadSession>(inputString)
                };
              }
              catch (SerializationException ex)
              {
                throw new ServiceException(new Error()
                {
                  Code = OneDriveErrorCode.GeneralException.ToString(),
                  Message = "Error deserializing UploadSession response: " + ex.Message,
                  AdditionalData = (IDictionary<string, object>) new Dictionary<string, object>()
                  {
                    {
                      "rawResponse",
                      (object) inputString
                    },
                    {
                      "rawHeaders",
                      (object) string.Join(", ", response.Headers.Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (h => string.Format("{0}: {1}", (object) h.Key, (object) h.Value))))
                    }
                  }
                });
              }
            }
          }
          return new UploadChunkResult()
          {
            ItemResponse = this.Client.HttpProvider.Serializer.DeserializeObject<Item>(inputString)
          };
        }
        throw new ServiceException(new Error()
        {
          Code = OneDriveErrorCode.GeneralException.ToString(),
          Message = "UploadChunkRequest received no response."
        });
      }
    }

    private async Task<HttpResponseMessage> SendRequestAsync(
      Stream stream,
      CancellationToken cancellationToken,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
    {
      if (string.IsNullOrEmpty(this.RequestUrl))
        throw new ArgumentNullException("RequestUrl", "Session Upload URL cannot be null or empty.");
      if (this.Client.AuthenticationProvider == null)
        throw new ArgumentNullException("AuthenticationProvider", "Client.AuthenticationProvider must not be null.");
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage request = this.GetHttpRequestMessage())
      {
        await this.Client.AuthenticationProvider.AuthenticateRequestAsync(request).ConfigureAwait(false);
        request.Content = (HttpContent) new StreamContent(stream);
        request.Content.Headers.ContentRange = new ContentRangeHeaderValue(this.RangeBegin, this.RangeEnd, this.TotalSessionLength);
        request.Content.Headers.ContentLength = new long?((long) this.RangeLength);
        httpResponseMessage = await this.Client.HttpProvider.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);
      }
      return httpResponseMessage;
    }
  }
}
