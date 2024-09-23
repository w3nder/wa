// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.AsyncMonitor`1
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class AsyncMonitor<T> : IAsyncMonitor<T>
  {
    private AsyncOperationStatus asyncOperationStatus;
    private IBaseClient client;
    internal string monitorUrl;

    public AsyncMonitor(IBaseClient client, string monitorUrl)
    {
      this.client = client;
      this.monitorUrl = monitorUrl;
    }

    public async Task<T> PollForOperationCompletionAsync(
      IProgress<AsyncOperationStatus> progress,
      CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        using (HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, this.monitorUrl))
        {
          await this.client.AuthenticationProvider.AuthenticateRequestAsync(httpRequestMessage).ConfigureAwait(false);
          using (HttpResponseMessage responseMessage = await this.client.HttpProvider.SendAsync(httpRequestMessage).ConfigureAwait(false))
          {
            if (responseMessage.StatusCode != HttpStatusCode.Accepted && responseMessage.IsSuccessStatusCode)
            {
              using (Stream stream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
                return this.client.HttpProvider.Serializer.DeserializeObject<T>(stream);
            }
            else
            {
              using (Stream stream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
              {
                this.asyncOperationStatus = this.client.HttpProvider.Serializer.DeserializeObject<AsyncOperationStatus>(stream);
                if (this.asyncOperationStatus == null)
                  throw new ServiceException(new Error()
                  {
                    Code = ErrorConstants.Codes.GeneralException,
                    Message = "Error retrieving monitor status."
                  });
                if (string.Equals(this.asyncOperationStatus.Status, "cancelled", StringComparison.OrdinalIgnoreCase))
                  return default (T);
                if (string.Equals(this.asyncOperationStatus.Status, "failed", StringComparison.OrdinalIgnoreCase) || string.Equals(this.asyncOperationStatus.Status, "deleteFailed", StringComparison.OrdinalIgnoreCase))
                {
                  object obj = (object) null;
                  if (this.asyncOperationStatus.AdditionalData != null)
                    this.asyncOperationStatus.AdditionalData.TryGetValue("message", out obj);
                  throw new ServiceException(new Error()
                  {
                    Code = ErrorConstants.Codes.GeneralException,
                    Message = obj as string
                  });
                }
                progress?.Report(this.asyncOperationStatus);
              }
            }
          }
        }
        await Task.Delay(5000, cancellationToken).ConfigureAwait(false);
      }
      return default (T);
    }
  }
}
