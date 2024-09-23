// Decompiled with JetBrains decompiler
// Type: WhatsApp.IMessageLoadingHandler
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public interface IMessageLoadingHandler
  {
    void OnInitialMessages(Message[] msgs, int? targetLandingIndex, bool isReInit);

    void OnOlderMessages(Message[] msgs);

    void OnNewerMessages(Message[] msgs);

    void OnMessageInsertion(Message msg);

    void OnMessageDeletion(Message msg);

    void OnDbReset(Message[] newerMsgs);

    void OnUpdatedMessages(Message[] msg);

    void OnMessagesRequestProcessing();

    void OnMessagesRequestProcessed();
  }
}
