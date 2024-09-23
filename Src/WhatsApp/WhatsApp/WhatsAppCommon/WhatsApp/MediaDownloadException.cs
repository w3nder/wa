// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaDownloadException
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class MediaDownloadException : OperationCanceledException
  {
    private static string LogHdrEx = nameof (MediaDownloadException);

    public MediaDownloadException(MediaDownloadException.DownloadErrorCode code, string msg)
      : base(msg ?? string.Format("Unexpected Download error:{0}, error:", (object) code))
    {
      this.ErrorCode = code;
      this.HttpResponseCode = -1;
      if (code == MediaDownloadException.DownloadErrorCode.UnexpectedHttpCode)
      {
        Log.l(MediaDownloadException.LogHdrEx, "Invalid values {0}", (object) code);
        throw new InvalidOperationException("Invalid value for " + MediaDownloadException.LogHdrEx);
      }
    }

    public MediaDownloadException(
      MediaDownloadException.DownloadErrorCode code,
      int httpCode,
      string msg)
      : base(msg ?? string.Format("Unexpected Download error:{0}, httpCode:{1}", (object) code, (object) httpCode))
    {
      this.ErrorCode = code;
      this.HttpResponseCode = httpCode;
      if (code != MediaDownloadException.DownloadErrorCode.UnexpectedHttpCode || httpCode <= 0)
      {
        Log.l(MediaDownloadException.LogHdrEx, "Invalid values {0}, {1}", (object) code, (object) httpCode);
        throw new InvalidOperationException("Invalid values for " + MediaDownloadException.LogHdrEx);
      }
    }

    public int HttpResponseCode { get; private set; }

    public MediaDownloadException.DownloadErrorCode ErrorCode { get; private set; }

    public enum DownloadErrorCode
    {
      Unknown,
      UnexpectedHttpCode,
      UnexpectedProcessingState,
      NotConnected,
      NoAccessToServer,
      CancelledByUser,
    }
  }
}
