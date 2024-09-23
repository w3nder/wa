// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.SendSticker
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.CommonOps
{
  public static class SendSticker
  {
    public static void Send(
      Sticker sticker,
      string jid,
      bool c2cStarted = false,
      Message quotedMessage = null,
      string quotedChat = null)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message message = new Message(true)
        {
          Status = FunXMPP.FMessage.Status.Unsent,
          KeyFromMe = true,
          KeyId = FunXMPP.GenerateMessageId(),
          KeyRemoteJid = jid,
          MediaWaType = FunXMPP.FMessage.Type.Sticker,
          MediaMimeType = sticker.MimeType,
          MediaKey = sticker.MediaKey,
          MediaHash = sticker.FileHash,
          MediaSize = sticker.FileLength,
          MediaUrl = sticker.Url,
          LocalFileUri = sticker.LocalFileUri
        };
        MessageProperties forMessage = MessageProperties.GetForMessage(message);
        forMessage.EnsureCommonProperties.CipherMediaHash = sticker.EncodedFileHash;
        forMessage.EnsureMediaProperties.Height = new uint?((uint) sticker.Height);
        forMessage.EnsureMediaProperties.Width = new uint?((uint) sticker.Width);
        forMessage.Save();
        message.SetQuote(quotedMessage, quotedChat);
        message.SetC2cFlags(c2cStarted);
        db.InsertMessageOnSubmit(message);
        db.LocalFileAddRef(sticker.LocalFileUri, LocalFileType.MessageMedia);
        db.SubmitChanges();
      }));
    }
  }
}
