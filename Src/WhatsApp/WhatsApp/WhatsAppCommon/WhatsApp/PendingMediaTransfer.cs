// Decompiled with JetBrains decompiler
// Type: WhatsApp.PendingMediaTransfer
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;


namespace WhatsApp
{
  public class PendingMediaTransfer
  {
    public PendingMediaTransfer.TransferTypes TransferType;
    public DateTime CreatedTime;
    public string Jid;
    public bool FromMe;
    public string Id;
    public Action<Unit> OnNext;
    public Action<Exception> OnError;
    public Action OnCompleted;
    private bool clbSent;

    public PendingMediaTransfer(
      Message msg,
      PendingMediaTransfer.TransferTypes tType,
      Action<Unit> onNext,
      Action<Exception> onError,
      Action onCompleted)
    {
      this.TransferType = tType;
      this.CreatedTime = FunRunner.CurrentServerTimeUtc;
      this.Jid = msg.KeyRemoteJid;
      this.FromMe = msg.KeyFromMe;
      this.Id = msg.KeyId;
      this.OnNext = onNext;
      this.OnError = onError;
      this.OnCompleted = onCompleted;
      if (this.Jid != null && this.Id != null)
        return;
      Log.l(nameof (PendingMediaTransfer), "Unexpected null: {0}, {1}, {2}, {3}, {4}", (object) (this.Jid ?? "null"), (object) this.FromMe, (object) (this.Id ?? "null"), (object) (msg.RemoteResource ?? "null"), (object) this.TransferType);
      Log.SendCrashLog((Exception) new ArgumentNullException("Msg key", "Unexpected null"), nameof (PendingMediaTransfer));
      this.clbSent = true;
    }

    public override bool Equals(object obj)
    {
      return obj is PendingMediaTransfer pendingMediaTransfer && this.Jid == pendingMediaTransfer.Jid && this.FromMe == pendingMediaTransfer.FromMe && this.Id == pendingMediaTransfer.Id;
    }

    public override int GetHashCode()
    {
      if (!this.clbSent && (this.Jid == null || this.Id == null))
      {
        Log.l(nameof (PendingMediaTransfer), "Unexpected null creating hash: {0}, {1}, {2}, {3}", (object) (this.Jid ?? "null"), (object) this.FromMe, (object) (this.Id ?? "null"), (object) this.TransferType);
        Log.SendCrashLog((Exception) new ArgumentNullException("Msg key", "Unexpected null creating hash"), nameof (PendingMediaTransfer));
        this.clbSent = true;
      }
      string jid = this.Jid;
      int num1 = (jid != null ? jid.GetHashCode() : 17) ^ this.FromMe.GetHashCode();
      string id = this.Id;
      int num2 = id != null ? id.GetHashCode() : 29;
      return num1 ^ num2;
    }

    public enum TransferTypes
    {
      None,
      Transcode,
      Upload_NotWeb,
      Upload_Web,
      Download_Foreground_Interactive,
      Download_Foreground_NotInteractive,
      Download_Background,
      Download_Foreground_Interactive_Streaming,
    }
  }
}
