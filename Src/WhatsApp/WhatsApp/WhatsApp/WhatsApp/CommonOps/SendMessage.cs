// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.SendMessage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp.CommonOps
{
  public static class SendMessage
  {
    public static void ChooseRecipientAndSendNew(Message[] msgs, string pickerTitle)
    {
      SendMessage.ChooseRecipientAndSendImpl(msgs, false, pickerTitle);
    }

    public static void ChooseRecipientAndForwardExisting(Message[] msgs, bool excludeStatus = false)
    {
      SendMessage.ChooseRecipientAndSendImpl(msgs, true, AppResources.Forward, (Func<string, IObservable<bool>>) (selJid =>
      {
        string name = (string) null;
        if (JidHelper.IsUserJid(selJid))
          name = JidHelper.GetDisplayNameForContactJid(selJid);
        else
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            Conversation conversation = db.GetConversation(selJid, CreateOptions.None);
            if (conversation == null)
              Log.SendCrashLog((Exception) new ArgumentNullException(), "forwarding to null chat", logOnlyForRelease: true);
            else
              name = conversation.GroupSubject;
          }));
        string str1 = AppResources.ConfirmForward;
        string str2 = AppResources.ConfirmForwardThese;
        if (JidHelper.IsMultiParticipantsChatJid(selJid))
        {
          str1 = AppResources.ConfirmForwardToGroup;
          str2 = AppResources.ConfirmForwardTheseToGroup;
        }
        return SendMessage.ValidateRecipientPreSending(selJid, ((IEnumerable<Message>) msgs).Select<Message, FunXMPP.FMessage.Type>((Func<Message, FunXMPP.FMessage.Type>) (m => m.MediaWaType)).ToArray<FunXMPP.FMessage.Type>()).Decision(Bidi.Format(msgs.Length > 1 ? str2 : str1, name)).Take<bool>(1);
      }), excludeStatus);
    }

    private static void ChooseRecipientAndSendImpl(
      Message[] msgs,
      bool forwardExisting,
      string recipientPickerTitle = null,
      Func<string, IObservable<bool>> confirmSelectionFunc = null,
      bool excludeStatus = false)
    {
      int[] array = ((IEnumerable<Message>) msgs).Select<Message, int>((Func<Message, int>) (m => m.MediaDurationSeconds)).ToArray<int>();
      int maxDuration = ((IEnumerable<int>) array).Any<int>() ? ((IEnumerable<int>) array).Max() : 0;
      RecipientListPage.StartRecipientPicker(recipientPickerTitle, ((IEnumerable<Message>) msgs).FirstOrDefault<Message>()?.MediaWaType, ((IEnumerable<Message>) msgs).FirstOrDefault<Message>()?.KeyRemoteJid, statusVisibilityObservable: excludeStatus ? Observable.Return<bool>(false) : (IObservable<bool>) null, maxDuration: maxDuration).ObserveOnDispatcher<RecipientListPage.RecipientListResults>().Subscribe<RecipientListPage.RecipientListResults>((Action<RecipientListPage.RecipientListResults>) (recipientListResults =>
      {
        List<string> selectedJids = recipientListResults?.SelectedJids;
        Log.l("forward", "send to recipients:{0}", (object) (selectedJids == null ? -1 : selectedJids.Count));
        if (selectedJids == null || !selectedJids.Any<string>())
          return;
        SendMessage.SendTo(msgs, forwardExisting, selectedJids);
      }));
    }

    private static void SendTo(Message[] msgs, bool forwardExisting, List<string> destJids)
    {
      if (destJids == null || destJids.Count == 0)
        throw new ArgumentNullException("destJid is null");
      if (((IEnumerable<Message>) msgs).Any<Message>((Func<Message, bool>) (m => m.MediaWaType == FunXMPP.FMessage.Type.Divider || m.MediaWaType == FunXMPP.FMessage.Type.System)))
        msgs = ((IEnumerable<Message>) msgs).Where<Message>((Func<Message, bool>) (m => m.MediaWaType != FunXMPP.FMessage.Type.Divider && m.MediaWaType != FunXMPP.FMessage.Type.System)).ToArray<Message>();
      bool sendToStatus = destJids.Contains("status@broadcast");
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        foreach (Message msg in msgs)
        {
          foreach (string destJid in destJids)
          {
            Message message;
            if (forwardExisting)
            {
              message = msg.CreateForwardMessage(destJid, db);
            }
            else
            {
              message = new Message();
              message.CopyFrom((SqliteMessagesContext) db, msg, true, true);
              message.KeyRemoteJid = destJid;
            }
            MessageProperties messageProperties = (MessageProperties) null;
            if (JidHelper.IsStatusJid(destJid) && message.MediaWaType == FunXMPP.FMessage.Type.Undefined)
            {
              message.MediaWaType = FunXMPP.FMessage.Type.ExtendedText;
              messageProperties = MessageProperties.GetForMessage(message);
              MessageProperties.ExtendedTextProperties extendedTextProperties = messageProperties.EnsureExtendedTextProperties;
              if (!extendedTextProperties.Font.HasValue)
                extendedTextProperties.Font = new int?(0);
              if (!extendedTextProperties.BackgroundArgb.HasValue)
                extendedTextProperties.BackgroundArgb = new uint?(WaStatusHelper.ColorToUint(WaStatusHelper.GetRandomTextStatusBackgroundColor()));
            }
            if (destJids.Count > 1)
            {
              messageProperties = MessageProperties.GetForMessage(message);
              messageProperties.EnsureCommonProperties.Multicast = new bool?(true);
            }
            messageProperties?.Save();
            db.InsertMessageOnSubmit(message);
          }
          if (sendToStatus)
            FieldStats.ReportFsStatusPostEvent(msg.MediaWaType, forwardExisting ? wam_enum_status_post_origin.FORWARD_FROM_MESSAGES : wam_enum_status_post_origin.EXTERNAL_SHARE, wam_enum_status_post_result.OK);
        }
        db.SubmitChanges();
      }));
      if (destJids.Count == 1 && !JidHelper.IsStatusJid(destJids.First<string>()))
        NavUtils.NavigateToChat(destJids.First<string>(), true);
      else
        NavUtils.NavigateToPage("ContactsPage");
    }

    public static IObservable<bool> ValidateRecipientPreSending(string jid)
    {
      return SendMessage.ValidateRecipientPreSending(jid, (FunXMPP.FMessage.Type[]) null);
    }

    public static IObservable<bool> ValidateRecipientPreSending(
      string jid,
      FunXMPP.FMessage.Type msgType)
    {
      return SendMessage.ValidateRecipientPreSending(jid, new FunXMPP.FMessage.Type[1]
      {
        msgType
      });
    }

    public static IObservable<bool> ValidateRecipientPreSending(
      string jid,
      FunXMPP.FMessage.Type[] msgTypes)
    {
      return jid == null ? Observable.Return<bool>(false) : Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        IObservable<bool> source = !JidHelper.IsGroupJid(jid) ? (!JidHelper.IsUserJid(jid) ? Observable.Return<bool>(true) : SendMessage.CheckContactBlockedStatus(jid)) : SendMessage.CheckGroupMembership(jid);
        if (msgTypes == null || !((IEnumerable<FunXMPP.FMessage.Type>) msgTypes).Any<FunXMPP.FMessage.Type>())
          source.Subscribe(observer);
        else
          source.Subscribe<bool>((Action<bool>) (res =>
          {
            if (res)
              SendMessage.CheckMessageTypeSupport(jid, msgTypes).Subscribe(observer);
            else
              observer.OnNext(false);
          }), (Action) (() => observer.OnCompleted()));
        return (Action) (() => { });
      }));
    }

    private static IObservable<bool> CheckMessageTypeSupport(
      string jid,
      FunXMPP.FMessage.Type[] msgTypes)
    {
      if (jid == null)
        return Observable.Return<bool>(false);
      return msgTypes == null || !((IEnumerable<FunXMPP.FMessage.Type>) msgTypes).Any<FunXMPP.FMessage.Type>() ? Observable.Return<bool>(true) : Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        observer.OnNext(true);
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    private static IObservable<bool> CheckGroupMembership(string jid)
    {
      if (jid == null)
        return Observable.Return<bool>(false);
      if (!JidHelper.IsGroupJid(jid))
        throw new ArgumentException("invalid group jid: " + jid);
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        bool isGroupParticipant = false;
        bool announceOnly = true;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          Conversation conversation = db.GetConversation(jid, CreateOptions.None);
          if (conversation == null)
            return;
          isGroupParticipant = conversation.IsGroupParticipant();
          announceOnly = conversation.IsAnnounceOnlyForUser();
        }));
        if (!isGroupParticipant | announceOnly)
        {
          UIUtils.ShowMessageBox(AppResources.NotGroupParticipantPopupTitle, !isGroupParticipant ? AppResources.NotGroupParticipantPopupBody : AppResources.AnnouncementOnlyGroupSendMessageNotAdmin).Subscribe<Unit>((Action<Unit>) (_ =>
          {
            observer.OnNext(false);
            observer.OnCompleted();
          }));
        }
        else
        {
          observer.OnNext(true);
          observer.OnCompleted();
        }
        return (Action) (() => { });
      }));
    }

    private static IObservable<bool> CheckContactBlockedStatus(string jid)
    {
      if (jid == null)
        return Observable.Return<bool>(false);
      return JidHelper.IsUserJid(jid) ? BlockContact.PromptUnblockIfBlocked(jid) : throw new ArgumentException("invalid group jid:" + jid);
    }
  }
}
