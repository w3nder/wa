// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmailChatHistoryUWP
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Storage.Streams;

#nullable disable
namespace WhatsApp
{
  internal class EmailChatHistoryUWP : EmailChatHistory
  {
    public const int MessageLimitPerEmail = 40000;

    public EmailChatHistoryUWP(string jid)
      : base(jid)
    {
    }

    protected static async Task GenerateEmailAttachment(
      InMemoryRandomAccessStream outputStream,
      Conversation convo,
      IEnumerable<Message> messages,
      Dictionary<string, string> chatParticipants)
    {
      using (DataWriter writer = new DataWriter((IOutputStream) outputStream))
      {
        writer.put_UnicodeEncoding((UnicodeEncoding) 0);
        writer.put_ByteOrder((ByteOrder) 0);
        bool flag = convo.Jid.IsGroupJid();
        string nameByJid = flag ? (string) null : EmailChatHistory.GetNameByJid(convo.Jid, ref chatParticipants);
        string pushName = Settings.PushName;
        foreach (Message message in messages)
        {
          string senderName = message.KeyFromMe ? pushName : (flag ? EmailChatHistory.GetNameByJid(message.GetSenderJid(), ref chatParticipants) : nameByJid);
          int num = (int) writer.WriteString(EmailChatHistory.GetEmailLine(message, senderName, true));
        }
        int num1 = (int) await (IAsyncOperation<uint>) writer.StoreAsync();
        int num2 = await writer.FlushAsync() ? 1 : 0;
        writer.DetachStream();
      }
    }

    protected override IObservable<Unit> Send()
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        IEnumerable<Message> msgs = (IEnumerable<Message>) null;
        Conversation convo = (Conversation) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          convo = db.GetConversation(this.jid, CreateOptions.None);
          if (convo == null)
            return;
          msgs = ((IEnumerable<Message>) convo.GetLatestMessages(db, new int?(40000), new int?())).Reverse<Message>();
        }));
        if (convo != null && msgs != null)
        {
          if (msgs.Any<Message>())
          {
            try
            {
              Dictionary<string, string> chatParticipants = new Dictionary<string, string>();
              string emailSubject = EmailChatHistory.GenerateEmailSubject(convo, ref chatParticipants);
              Dictionary<char, bool> dictionary = ((IEnumerable<char>) Path.GetInvalidFileNameChars()).ToDictionary<char, char, bool>((Func<char, char>) (c => c), (Func<char, bool>) (c => true));
              StringBuilder stringBuilder = new StringBuilder(emailSubject.Length);
              foreach (char key in emailSubject)
              {
                if (!dictionary.ContainsKey(key))
                  stringBuilder.Append(key);
              }
              string str = string.Format(AppResources.EmailChatHistoryBody, (object) stringBuilder);
              InMemoryRandomAccessStream attachmentStream = new InMemoryRandomAccessStream();
              Task emailAttachment1 = EmailChatHistoryUWP.GenerateEmailAttachment(attachmentStream, convo, msgs, chatParticipants);
              EmailMessage mail = new EmailMessage();
              mail.put_Subject(emailSubject);
              mail.put_Body(str);
              emailAttachment1.Wait();
              EmailAttachment emailAttachment2 = new EmailAttachment(stringBuilder.ToString() + ".txt", (IRandomAccessStreamReference) RandomAccessStreamReference.CreateFromStream((IRandomAccessStream) attachmentStream));
              mail.Attachments.Add(emailAttachment2);
              Deployment.Current.Dispatcher.BeginInvoke((Action) (async () =>
              {
                await EmailManager.ShowComposeNewEmailAsync(mail);
                ((IDisposable) attachmentStream).SafeDispose();
                observer.OnNext(new Unit());
              }));
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "emailchat");
            }
            observer.OnCompleted();
            return (Action) (() => { });
          }
        }
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }
  }
}
