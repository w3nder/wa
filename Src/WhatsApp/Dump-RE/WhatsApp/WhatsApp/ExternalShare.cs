// Decompiled with JetBrains decompiler
// Type: WhatsApp.ExternalShare
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public class ExternalShare
  {
    public static IExternalShare GetSourceForUri(IDictionary<string, string> queryStrings)
    {
      string str;
      if (queryStrings.TryGetValue("Source", out str) && str == UriShareContent.URI_SCHEME)
      {
        UriShareContent sourceForUri = new UriShareContent(queryStrings);
        if (sourceForUri.IsValid)
          return (IExternalShare) sourceForUri;
      }
      return (IExternalShare) new ExternalShare81();
    }

    public static ExternalShare.ExternalShareResult ShareTextContent(
      List<string> jids,
      string content,
      bool setDraftOnly)
    {
      Log.l(nameof (ExternalShare), "sharing text content {0} {1}", (object) (content == null ? -1 : content.Length), (object) setDraftOnly);
      if (content == null)
        content = "";
      content = content.MaybeTruncateToMaxRealCharLength(65536);
      if (!LinkDetector.IsSendableText(content) && !setDraftOnly)
      {
        Log.l("external", "Don't share non display data");
        setDraftOnly = true;
      }
      jids.Count<string>();
      foreach (string jid1 in jids)
      {
        string jid = jid1;
        if (setDraftOnly)
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => ConversationHelper.SetDraft(db, jid, content)));
        else
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            Message message = new Message(true)
            {
              KeyFromMe = true,
              KeyRemoteJid = jid,
              KeyId = FunXMPP.GenerateMessageId(),
              Data = content,
              Status = FunXMPP.FMessage.Status.Unsent,
              MediaWaType = FunXMPP.FMessage.Type.Undefined
            };
            if (jids.Count > 1)
            {
              MessageProperties forMessage = MessageProperties.GetForMessage(message);
              forMessage.EnsureCommonProperties.Multicast = new bool?(true);
              forMessage.Save();
            }
            db.InsertMessageOnSubmit(message);
            db.SubmitChanges();
          }));
      }
      return ExternalShare.ExternalShareResult.Shared;
    }

    public enum ExternalShareResult
    {
      Discarded,
      FileError,
      FileErrorPhoto,
      FileErrorVideo,
      MissingData,
      None,
      Shared,
      Unknown,
    }
  }
}
