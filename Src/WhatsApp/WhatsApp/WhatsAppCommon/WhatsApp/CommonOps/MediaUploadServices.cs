// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.MediaUploadServices
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.CommonOps
{
  public static class MediaUploadServices
  {
    public static void RetryMediaMessageSend(Message msg, bool webRetry = false)
    {
      if (MessageExtensions.GetUploadActionState(msg) != MessageExtensions.MediaUploadActionState.Retryable)
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        bool flag = false;
        MessageMiscInfo miscInfo = msg.GetMiscInfo();
        if (miscInfo != null)
          flag = miscInfo.TranscoderData != null;
        msg.Status = flag ? FunXMPP.FMessage.Status.Pending : FunXMPP.FMessage.Status.Uploading;
        db.SubmitChanges();
      }));
      if (msg.Status == FunXMPP.FMessage.Status.Pending)
      {
        AppState.ProcessPendingMessage(msg);
      }
      else
      {
        PendingMediaTransfer.TransferTypes tType = webRetry ? PendingMediaTransfer.TransferTypes.Upload_Web : PendingMediaTransfer.TransferTypes.Upload_NotWeb;
        msg.SetPendingMediaSubscription("Media re-upload", tType, MediaUpload.SendMediaObservable(msg, webRetry));
      }
    }

    public static void CancelMediaMessageSend(Message msg)
    {
      if (MessageExtensions.GetUploadActionState(msg) != MessageExtensions.MediaUploadActionState.Cancellable)
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        msg.Status = FunXMPP.FMessage.Status.Canceled;
        msg.CancelPendingMedia();
        db.SubmitChanges();
      }));
      if (!AppState.GetConnection().EventHandler.Qr.Session.Active)
        return;
      AppState.GetConnection().SendQrReceived(new FunXMPP.FMessage.Key(msg.KeyRemoteJid, msg.KeyFromMe, msg.KeyId), FunXMPP.FMessage.Status.Canceled);
    }
  }
}
