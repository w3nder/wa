// Decompiled with JetBrains decompiler
// Type: System.Net.Http.MessageProcessingHandler
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http
{
  /// <summary>A base type for handlers which only do some small processing of request and/or response messages.</summary>
  public abstract class MessageProcessingHandler : DelegatingHandler
  {
    protected MessageProcessingHandler()
    {
    }

    protected MessageProcessingHandler(HttpMessageHandler innerHandler)
      : base(innerHandler)
    {
    }

    /// <returns>Returns <see cref="T:System.Net.Http.HttpRequestMessage" />.</returns>
    protected abstract HttpRequestMessage ProcessRequest(
      HttpRequestMessage request,
      CancellationToken cancellationToken);

    /// <returns>Returns <see cref="T:System.Net.Http.HttpResponseMessage" />.</returns>
    protected abstract HttpResponseMessage ProcessResponse(
      HttpResponseMessage response,
      CancellationToken cancellationToken);

    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.</returns>
    protected internal override sealed Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request), SR.net_http_handler_norequest);
      TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
      try
      {
        base.SendAsync(this.ProcessRequest(request, cancellationToken), cancellationToken).ContinueWithStandard<HttpResponseMessage>((Action<Task<HttpResponseMessage>>) (task =>
        {
          if (task.IsFaulted)
            tcs.TrySetException(((Exception) task.Exception).GetBaseException());
          else if (task.IsCanceled)
            tcs.TrySetCanceled();
          else if (task.Result == null)
          {
            tcs.TrySetException((Exception) new InvalidOperationException(SR.net_http_handler_noresponse));
          }
          else
          {
            try
            {
              tcs.TrySetResult(this.ProcessResponse(task.Result, cancellationToken));
            }
            catch (OperationCanceledException ex)
            {
              MessageProcessingHandler.HandleCanceledOperations(cancellationToken, tcs, ex);
            }
            catch (Exception ex)
            {
              tcs.TrySetException(ex);
            }
          }
        }));
      }
      catch (OperationCanceledException ex)
      {
        MessageProcessingHandler.HandleCanceledOperations(cancellationToken, tcs, ex);
      }
      catch (Exception ex)
      {
        tcs.TrySetException(ex);
      }
      return tcs.Task;
    }

    private static void HandleCanceledOperations(
      CancellationToken cancellationToken,
      TaskCompletionSource<HttpResponseMessage> tcs,
      OperationCanceledException e)
    {
      if (cancellationToken.IsCancellationRequested && e.CancellationToken == cancellationToken)
        tcs.TrySetCanceled();
      else
        tcs.TrySetException((Exception) e);
    }
  }
}
