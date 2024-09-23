// Decompiled with JetBrains decompiler
// Type: WhatsApp.BackgroundDownloadDetails
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;


namespace WhatsApp
{
  public class BackgroundDownloadDetails
  {
    public CancellationTokenSource CancelSource;
    public Task BackgroundDownloadTask;
    public DownloadOperation BackgroundDownload;
    public IObserver<MediaDownloadProgress> BackgroundDownloadObserver;

    public BackgroundDownloadDetails(
      CancellationTokenSource cts,
      DownloadOperation d,
      IObserver<MediaDownloadProgress> obs)
    {
      this.CancelSource = cts;
      this.BackgroundDownload = d;
      this.BackgroundDownloadObserver = obs;
    }
  }
}
