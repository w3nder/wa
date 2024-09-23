// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaHelper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Windows.Networking.BackgroundTransfer;
using Windows.Web;

#nullable disable
namespace WhatsApp
{
  public class MediaHelper
  {
    private static readonly WebErrorStatus[] BtErrorsWhichInAppMightNotSee;

    public static HttpStatusCode? StatusCodeFromException(Exception ex)
    {
      HttpStatusCode? nullable = new HttpStatusCode?();
      switch (ex)
      {
        case MediaDownloadException downloadException when downloadException.ErrorCode == MediaDownloadException.DownloadErrorCode.UnexpectedHttpCode:
          return new HttpStatusCode?((HttpStatusCode) downloadException.HttpResponseCode);
        case WebException webException:
          StringBuilder stringBuilder = new StringBuilder();
          List<object> objectList = new List<object>();
          stringBuilder.Append("Managed HTTP API signalled failure: {0}");
          objectList.Add((object) webException.Status);
          if (webException.Response is HttpWebResponse response)
          {
            stringBuilder.Append(", response error: {1}");
            objectList.Add((object) response.StatusCode);
            if (response.ContentLength != 0L || response.SupportsHeaders && response.Headers.Count != 0)
            {
              stringBuilder.Append(" ({2}) - appears to come from server");
              objectList.Add((object) (int) response.StatusCode);
              nullable = new HttpStatusCode?(response.StatusCode);
            }
          }
          if (!nullable.HasValue)
            stringBuilder.Append(" - looks like transient connectivity error");
          Log.WriteLineDebug(stringBuilder.ToString(), objectList.ToArray());
          break;
      }
      return nullable;
    }

    public static MediaHelper.DownloadMethodOptions DetermineDownloadOptions(
      long downloadSize,
      long downloadMaxBytes,
      MediaHelper.DownloadContextOptions downloadContext)
    {
      MediaHelper.DownloadMethodOptions downloadOptions = MediaHelper.DownloadMethodOptions.InAppOnly;
      bool flag = MediaHelper.IsUserRequest(downloadContext) || MediaHelper.IsUserWaiting(downloadContext);
      if (!AppState.IsBackgroundAgent)
        downloadOptions = flag && NetworkStateMonitor.Is2GConnection() || downloadSize > 0L && downloadSize < 1048576L ? MediaHelper.DownloadMethodOptions.InAppOnly : (downloadMaxBytes <= 0L ? (flag ? MediaHelper.DownloadMethodOptions.BTandInApp : MediaHelper.DownloadMethodOptions.BTOnly) : MediaHelper.DownloadMethodOptions.InAppOnly);
      return downloadOptions;
    }

    public static bool BTExceptionSuggestsinAppMightWork(Exception ex)
    {
      WebErrorStatus status = BackgroundTransferError.GetStatus(ex.HResult);
      return ((IEnumerable<WebErrorStatus>) MediaHelper.BtErrorsWhichInAppMightNotSee).Contains<WebErrorStatus>(status);
    }

    public static bool IsUserRequest(MediaHelper.DownloadContextOptions downloadContext)
    {
      return (downloadContext & MediaHelper.DownloadContextOptions.UserRequest) == MediaHelper.DownloadContextOptions.UserRequest;
    }

    public static bool IsUserWaiting(MediaHelper.DownloadContextOptions downloadContext)
    {
      return (downloadContext & MediaHelper.DownloadContextOptions.UserWaiting) == MediaHelper.DownloadContextOptions.UserWaiting;
    }

    static MediaHelper()
    {
      // ISSUE: unable to decompile the method.
    }

    public enum DownloadMethodOptions
    {
      InAppOnly = 1,
      BTOnly = 2,
      BTandInApp = 3,
    }

    public enum DownloadContextOptions
    {
      Default,
      UserRequest,
      UserWaiting,
    }
  }
}
