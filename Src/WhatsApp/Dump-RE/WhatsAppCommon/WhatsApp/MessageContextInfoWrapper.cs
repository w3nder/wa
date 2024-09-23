// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageContextInfoWrapper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;
using WhatsApp.ProtoBuf;

#nullable disable
namespace WhatsApp
{
  public class MessageContextInfoWrapper
  {
    private Message msg;
    private FunXMPP.FMessage fmsg;
    private Message quotedMsg;
    private ContextInfo contextInfo;
    private bool deserializeAttempted;

    public string MessageRemoteJid
    {
      get
      {
        string keyRemoteJid = this.msg?.KeyRemoteJid;
        if (keyRemoteJid != null)
          return keyRemoteJid;
        return this.fmsg?.key.remote_jid;
      }
    }

    public ContextInfo ContextInfo
    {
      get
      {
        if (this.contextInfo == null && !this.deserializeAttempted)
        {
          this.deserializeAttempted = true;
          this.contextInfo = WhatsApp.ProtoBuf.Message.CreateFromPlainText(this.msg?.ProtoBuf ?? this.fmsg?.proto_buf)?.GetContextInfo();
        }
        return this.contextInfo;
      }
    }

    public string QuoteRemoteJid => this.ContextInfo?.RemoteJid;

    public string QuoteFromJid => this.ContextInfo?.RemoteJid ?? this.MessageRemoteJid;

    public string QuoteAuthorJid
    {
      get
      {
        string quoteAuthorJid = this.ContextInfo?.Participant;
        if (quoteAuthorJid == null)
        {
          string messageRemoteJid = this.MessageRemoteJid;
          if (JidHelper.IsUserJid(messageRemoteJid))
            quoteAuthorJid = this.msg.KeyFromMe ? Settings.MyJid : messageRemoteJid;
        }
        return quoteAuthorJid;
      }
    }

    public string QuotedKeyId => this.ContextInfo?.StanzaId;

    public Message QuotedMessage
    {
      get
      {
        if (this.quotedMsg == null)
        {
          WhatsApp.ProtoBuf.Message quotedMessage = this.ContextInfo?.QuotedMessage;
          if (quotedMessage != null)
          {
            string quoteFromJid = this.QuoteFromJid;
            string quoteAuthorJid = this.QuoteAuthorJid;
            FunXMPP.FMessage fMessage = new FunXMPP.FMessage(new FunXMPP.FMessage.Key(quoteFromJid, quoteAuthorJid == Settings.MyJid, this.QuotedKeyId));
            quotedMessage.PopulateFMessage(fMessage);
            this.quotedMsg = new Message(fMessage);
            if (JidHelper.IsMultiParticipantsChatJid(quoteFromJid))
              this.quotedMsg.RemoteResource = quoteAuthorJid;
          }
        }
        return this.quotedMsg;
      }
    }

    public List<string> MentionedJids => this.ContextInfo?.MentionedJid ?? new List<string>();

    public MessageContextInfoWrapper(Message m) => this.msg = m;

    public MessageContextInfoWrapper(FunXMPP.FMessage m) => this.fmsg = m;
  }
}
