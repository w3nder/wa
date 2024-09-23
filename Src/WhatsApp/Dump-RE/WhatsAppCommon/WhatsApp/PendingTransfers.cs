// Decompiled with JetBrains decompiler
// Type: WhatsApp.PendingTransfers
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public class PendingTransfers
  {
    private object pendingTransfersLock = new object();
    private Dictionary<int, PendingMediaTransfer> pendingMediaTransfers = new Dictionary<int, PendingMediaTransfer>();

    public void AddTransfer(PendingMediaTransfer pTransfer)
    {
      lock (this.pendingTransfersLock)
        this.pendingMediaTransfers[pTransfer.GetHashCode()] = pTransfer;
      Log.d("media transfer", "added pending transfer {0}", (object) this.pendingMediaTransfers.Count);
    }

    public bool RemoveTransfer(PendingMediaTransfer pTransfer)
    {
      bool flag = false;
      lock (this.pendingTransfersLock)
        flag = this.pendingMediaTransfers.Remove(pTransfer.GetHashCode());
      Log.d("media transfer", "removed pending transfer {0} {1}", (object) this.pendingMediaTransfers.Count, (object) flag);
      return flag;
    }

    public Dictionary<int, PendingMediaTransfer> ExtractPendingTransfers()
    {
      Dictionary<int, PendingMediaTransfer> pendingMediaTransfers;
      lock (this.pendingTransfersLock)
      {
        pendingMediaTransfers = this.pendingMediaTransfers;
        this.pendingMediaTransfers = new Dictionary<int, PendingMediaTransfer>();
      }
      Log.d("media transfer", "extracted pending transfers {0}", (object) pendingMediaTransfers.Count);
      return pendingMediaTransfers;
    }
  }
}
