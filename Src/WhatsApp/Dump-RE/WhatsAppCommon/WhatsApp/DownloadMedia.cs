// Decompiled with JetBrains decompiler
// Type: WhatsApp.DownloadMedia
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public class DownloadMedia
  {
    private object statusChangeLock = new object();

    public FunXMPP.FMessage.FunMediaType DownloadType { get; private set; }

    public string DownloadId { get; private set; }

    public byte[] PlainTextHash { get; set; }

    public string DownloadUrl { get; private set; }

    public string LocalUrl { get; private set; }

    public DateTime CreatedTimestamp { get; private set; }

    public DateTime UpdatedTimestamp { get; private set; }

    public DownloadMedia.DownloadStatuses DownloadStatus { get; private set; }

    public DownloadMedia.DownloadMethods DownloadMethod { get; private set; }

    public string ResponseCode { get; private set; }

    public byte[] DecryptionKey { get; private set; }

    public byte[] EncryptedHash { get; private set; }

    public MediaHelper.DownloadContextOptions DownloadContext { get; set; }

    public long DownloadSize { get; private set; }

    public long DownloadMaxSize { get; private set; } = -1;

    public DownloadMedia(
      FunXMPP.FMessage.FunMediaType dnType,
      string downloadId,
      string downloadUrl,
      string localUrl,
      long downloadSize,
      MediaHelper.DownloadContextOptions downloadContext)
    {
      this.DownloadType = dnType;
      this.DownloadId = downloadId;
      this.DownloadUrl = downloadUrl;
      this.LocalUrl = localUrl;
      this.DownloadSize = downloadSize;
      this.DownloadContext = downloadContext;
      this.UpdatedTimestamp = this.CreatedTimestamp = DateTime.UtcNow;
      this.DownloadStatus = DownloadMedia.DownloadStatuses.Unknown;
      this.DownloadMethod = DownloadMedia.DownloadMethods.Unknown;
      this.ResponseCode = (string) null;
      this.DecryptionKey = (byte[]) null;
      this.EncryptedHash = (byte[]) null;
    }

    public DownloadMedia(
      FunXMPP.FMessage.FunMediaType dnType,
      string downloadId,
      byte[] encryptedHash,
      byte[] encryptedKey,
      string localUrl,
      long downloadSize,
      MediaHelper.DownloadContextOptions downloadContext)
    {
      this.DownloadType = dnType;
      this.DownloadId = downloadId;
      this.DownloadUrl = (string) null;
      this.LocalUrl = localUrl;
      this.DownloadSize = downloadSize;
      this.DownloadContext = downloadContext;
      this.UpdatedTimestamp = this.CreatedTimestamp = DateTime.UtcNow;
      this.DownloadStatus = DownloadMedia.DownloadStatuses.Unknown;
      this.DownloadMethod = DownloadMedia.DownloadMethods.Unknown;
      this.ResponseCode = (string) null;
      this.DecryptionKey = encryptedKey;
      this.EncryptedHash = encryptedHash;
    }

    public bool StatusChange(DownloadMedia.DownloadStatuses from, DownloadMedia.DownloadStatuses to)
    {
      return this.StatusChange(new DownloadMedia.DownloadStatuses[1]
      {
        from
      }, to).HasValue;
    }

    public DownloadMedia.DownloadStatuses? StatusChange(
      DownloadMedia.DownloadStatuses[] from,
      DownloadMedia.DownloadStatuses to)
    {
      lock (this.statusChangeLock)
      {
        DownloadMedia.DownloadStatuses downloadStatus = this.DownloadStatus;
        if (((IEnumerable<DownloadMedia.DownloadStatuses>) from).Contains<DownloadMedia.DownloadStatuses>(this.DownloadStatus))
        {
          this.DownloadStatus = to;
          return new DownloadMedia.DownloadStatuses?(downloadStatus);
        }
      }
      return new DownloadMedia.DownloadStatuses?();
    }

    public enum DownloadStatuses
    {
      Unknown,
      Queued,
      Running,
      Paused,
      Finished,
      Cancelled,
    }

    public enum DownloadMethods
    {
      Unknown,
      InApp,
      BackgroundTransfer,
    }
  }
}
