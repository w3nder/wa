// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.HighlyStructuredMessage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Threading.Tasks;


namespace WhatsApp.ProtoBuf
{
  public class HighlyStructuredMessage
  {
    public static async Task<bool> RehydrateMessageAsync(
      string jid,
      string keyId,
      string remoteResource,
      DateTime timestamp,
      string senderJid,
      ulong serial,
      byte[] serializedMessage)
    {
      Message.HighlyStructuredMessage hsmMsg = Message.HighlyStructuredMessage.Deserialize(serializedMessage);
      Log.l("hsm", "running TranslateAsync");
      HsmTranslateResult hsmTranslateResult = await HsmLangPack.TranslateAsync(hsmMsg);
      Log.l("hsm", "ran TranslateAsync");
      TaskCompletionSource<bool> completedSource = new TaskCompletionSource<bool>();
      FunXMPP.Connection connection = AppState.GetConnection();
      if (hsmTranslateResult != null && hsmTranslateResult.ResultCode == HsmTranslateResultCode.Succeeded)
      {
        if (UserCache.Get(senderJid, false).VerifiedName != VerifiedNameState.PendingCertification)
        {
          FunXMPP.FMessage message = new FunXMPP.FMessage.Builder().Key(new FunXMPP.FMessage.Key(jid, false, keyId)).NewIncomingInstance().Timestamp(new DateTime?(timestamp)).Media_wa_type(FunXMPP.FMessage.Type.Undefined).VerifiedName(serial).Remote_resource(remoteResource).Data(hsmTranslateResult.ResultString).IsHighlyStructuredMessageRehydrate(true).Build();
          connection.EventHandler.StoreIncomingMessage(message);
          completedSource.SetResult(true);
        }
        else
        {
          WAThreadPool.QueueUserWorkItem((Action) (() => VerifiedNamesCertifier.ScheduleCertifyVerifiedUserAction(senderJid, new DateTime?())));
          completedSource.SetResult(false);
        }
      }
      else if (hsmTranslateResult != null && hsmTranslateResult.ResultCode != HsmTranslateResultCode.TryAgainLater || DateTime.Now - timestamp > TimeSpan.FromHours(24.0))
      {
        Action onComplete = (Action) (() => completedSource.SetResult(true));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("error", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("type", "structure-unavailable")
        }, (byte[]) null);
        connection.SendReceipt(jid, (string) null, keyId, "error", child, onComplete);
      }
      else
        completedSource.SetResult(false);
      return await completedSource.Task;
    }
  }
}
