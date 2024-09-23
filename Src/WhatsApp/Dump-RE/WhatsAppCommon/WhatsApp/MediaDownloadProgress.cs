// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaDownloadProgress
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public class MediaDownloadProgress
  {
    public long TotalToDownload;
    public long DownloadedSoFar;
    public int HttpRespCode = -1;
    public bool BackgroundDownloadFailedTryForeground;

    public MediaDownloadProgress.DownloadStatus DownloadState { get; private set; }

    public MediaDownloadProgress(MediaDownloadProgress.DownloadStatus state)
    {
      this.DownloadState = state;
    }

    public enum DownloadStatus
    {
      Unknown,
      Active,
      Completed,
    }
  }
}
