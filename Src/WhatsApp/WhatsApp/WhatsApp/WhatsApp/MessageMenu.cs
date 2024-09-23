// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageMenu
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Windows;
using WhatsApp.CommonOps;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public static class MessageMenu
  {
    public static Pair<string, Message> CopiedMessage;
    private static Subject<Pair<MessageViewModel, bool>> messageQuotedSubject;

    public static IObservable<Pair<MessageViewModel, bool>> GetMessageQuotedObservable()
    {
      return (IObservable<Pair<MessageViewModel, bool>>) MessageMenu.messageQuotedSubject ?? (IObservable<Pair<MessageViewModel, bool>>) (MessageMenu.messageQuotedSubject = new Subject<Pair<MessageViewModel, bool>>());
    }

    private static void NotifyMessageQuoted(MessageViewModel vm, bool inPrivate)
    {
      if (MessageMenu.messageQuotedSubject == null || vm?.Message == null)
        return;
      MessageMenu.messageQuotedSubject.OnNext(new Pair<MessageViewModel, bool>(vm, inPrivate));
    }

    private static void Star_Click(object sender, EventArgs e)
    {
      MessageViewModel tag = !(sender is MenuItem menuItem) ? (MessageViewModel) null : menuItem.Tag as MessageViewModel;
      if (tag == null || tag.Message == null)
        return;
      Message msg = tag.Message;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        msg.IsStarred = !msg.IsStarred;
        db.SubmitChanges();
      }));
      StarChange.NotifyQrStarChange(msg);
    }

    private static void Reply_Click(object sender, EventArgs e)
    {
      MessageMenu.NotifyMessageQuoted((sender is MenuItem menuItem ? menuItem.Tag : (object) null) as MessageViewModel, false);
    }

    private static void ReplyInPrivate_Click(object sender, EventArgs e)
    {
      MessageMenu.NotifyMessageQuoted((sender is MenuItem menuItem ? menuItem.Tag : (object) null) as MessageViewModel, true);
    }

    private static void Copy_Click(object sender, EventArgs e)
    {
      Message message = (sender is MenuItem menuItem ? menuItem.Tag : (object) null) is MessageViewModel tag ? tag.Message : (Message) null;
      if (message == null)
        return;
      LinkDetector.Result[] updatedFormattings = (LinkDetector.Result[]) null;
      string textOnly = Emoji.ConvertToTextOnly(message.GetMentionsRenderedText(out updatedFormattings), (IEnumerable<LinkDetector.Result>) updatedFormattings, false);
      MessageMenu.CopiedMessage = new Pair<string, Message>(textOnly, message);
      Clipboard.SetText(textOnly);
      Log.d(message.LogInfo(false), "message copied");
    }

    private static void Delete_Click(object sender, EventArgs e)
    {
      MessageViewModel tag = !(sender is MenuItem menuItem) ? (MessageViewModel) null : menuItem.Tag as MessageViewModel;
      if (tag == null)
        return;
      Message msg = tag.Message;
      if (msg.CanRevoke())
      {
        Observable.Return<bool>(true).Decisions(AppResources.DeleteSingleMessageConfirm, new string[3]
        {
          AppResources.DeleteForSelfButton,
          AppResources.DeleteForEveryoneButton,
          AppResources.Cancel
        }).Take<int>(1).ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (result =>
        {
          switch (result)
          {
            case 0:
              MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.DeleteMessage(msg)));
              break;
            case 1:
              RevokeMessage.RevokeMessageFromMe(msg);
              if (Settings.RevokeFirstUseMessageDisplayed)
                break;
              UIUtils.ShowMessageBoxWithLearnMore(AppResources.RevokeFirstUse, WaWebUrls.FaqUrlRecallWp);
              Settings.RevokeFirstUseMessageDisplayed = true;
              break;
          }
        }));
      }
      else
      {
        string deleteForSelfButton = AppResources.DeleteForSelfButton;
        Observable.Return<bool>(true).Decision(AppResources.DeleteSingleMessageConfirm, deleteForSelfButton, AppResources.Cancel).Take<bool>(1).Subscribe<bool>((Action<bool>) (result =>
        {
          if (!result)
            return;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.DeleteMessage(msg)));
        }));
      }
    }

    private static void Save_Click(object sender, EventArgs e)
    {
      Message msg = (sender as MenuItem)?.Tag is MessageViewModel tag ? tag.Message : (Message) null;
      if (msg == null)
        return;
      bool saved = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (msg.MediaWaType == FunXMPP.FMessage.Type.Sticker)
          saved = msg.SaveSticker(db);
        else
          saved = msg.CopyMediaToAlbum(db, "Saved Pictures");
      }));
      Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
      {
        string messageBoxText;
        if (saved)
        {
          messageBoxText = msg.MediaWaType != FunXMPP.FMessage.Type.Sticker ? AppResources.AlbumTileSavedPictures : AppResources.SavedSticker;
        }
        else
        {
          switch (msg.MediaWaType)
          {
            case FunXMPP.FMessage.Type.Video:
              messageBoxText = AppResources.SaveVideoFailure;
              break;
            case FunXMPP.FMessage.Type.Sticker:
              messageBoxText = AppResources.SaveStickerFailure;
              break;
            default:
              messageBoxText = AppResources.SavePictureFailure;
              break;
          }
        }
        int num = (int) MessageBox.Show(messageBoxText);
      }));
    }

    private static void Forward_Click(object sender, EventArgs e)
    {
      MessageViewModel tag = !(sender is MenuItem menuItem) ? (MessageViewModel) null : menuItem.Tag as MessageViewModel;
      if (tag == null)
        return;
      SendMessage.ChooseRecipientAndForwardExisting(new Message[1]
      {
        tag.Message
      });
    }

    private static void AddToContacts_Click(object sender, EventArgs e)
    {
      MessageViewModel tag = !(sender is MenuItem menuItem) ? (MessageViewModel) null : menuItem.Tag as MessageViewModel;
      if (tag == null || ChatPage.Current == null)
        return;
      ChatPage.Current.AddJidToContactsUseVCards(tag.SenderJid);
    }

    private static void AddToExistingContact_Click(object sender, EventArgs e)
    {
      MessageViewModel tag = !(sender is MenuItem menuItem) ? (MessageViewModel) null : menuItem.Tag as MessageViewModel;
      if (tag == null || ChatPage.Current == null)
        return;
      ChatPage.Current.AddJidToContacts(tag.SenderJid, true);
    }

    private static void SendMessage_Click(object sender, EventArgs e)
    {
      MessageViewModel tag = !(sender is MenuItem menuItem) ? (MessageViewModel) null : menuItem.Tag as MessageViewModel;
      if (tag == null)
        return;
      NavUtils.NavigateToChat(tag.SenderJid, false);
    }

    private static void ShowMessageDetails_Click(object sender, EventArgs e)
    {
      MessageViewModel tag = !(sender is MenuItem menuItem) ? (MessageViewModel) null : menuItem.Tag as MessageViewModel;
      if (tag == null || tag.Message == null)
        return;
      MessageDetailPage.Start(tag.Message);
    }

    private static void DebugIndexMessage_Click(object sender, RoutedEventArgs e)
    {
      MenuItem menuItem = sender as MenuItem;
      MessageViewModel vm = menuItem == null ? (MessageViewModel) null : menuItem.Tag as MessageViewModel;
      if (vm == null || vm.Message == null)
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.IndexMessageForSearch(vm.Message.KeyRemoteJid, vm.Message.KeyId, vm.Message.KeyFromMe)));
    }

    private static void DebugMessage_Click(object sender, RoutedEventArgs e)
    {
      Message msg = (sender as MenuItem)?.Tag is MessageViewModel tag ? tag.Message : (Message) null;
      if (msg == null)
        return;
      string displayString = msg.KeyId + "(" + (object) msg.MessageID + ")";
      if (msg.LocalFileUri != null)
      {
        int count = 0;
        int stickerCount = 0;
        int quotedCount = 0;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          count = db.LocalFileCountRef(msg.LocalFileUri, LocalFileType.MessageMedia);
          stickerCount = db.LocalFileCountRef(msg.LocalFileUri, LocalFileType.Sticker);
          quotedCount = db.LocalFileCountRef(msg.LocalFileUri, LocalFileType.QuotedMedia);
        }));
        displayString = displayString + "\nlf(" + (object) count;
        if (msg.MediaWaType == FunXMPP.FMessage.Type.Sticker)
        {
          displayString = displayString + "), sf(" + (object) stickerCount;
          displayString = displayString + "), qf(" + (object) quotedCount;
        }
        displayString = displayString + "): " + msg.LocalFileUri;
      }
      if (msg.QuotedMediaFileUri != null)
      {
        int count = 0;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => count = db.LocalFileCountRef(msg.QuotedMediaFileUri, LocalFileType.QuotedMedia)));
        displayString = displayString + "\nqf(" + (object) count + "): " + msg.QuotedMediaFileUri;
      }
      if (msg.MediaUploadUrl != null)
        displayString = displayString + "\nup: " + msg.MediaUploadUrl;
      if (!string.IsNullOrEmpty(msg.MediaUrl))
        displayString = displayString + "\ndn: " + msg.MediaUrl;
      if (Mms4Helper.IsMms4DownloadMessage(msg))
        displayString += "\nMms4 enabled";
      if (msg.MediaWaType == FunXMPP.FMessage.Type.System)
      {
        SystemMessageWrapper.MessageTypes systemMessageType = msg.GetSystemMessageType();
        displayString = displayString + "\nS:" + systemMessageType.ToString();
      }
      if (displayString.Length >= (int) byte.MaxValue)
        displayString = displayString.Replace('\n', '.');
      UIUtils.MessageBox("debug", displayString, (IEnumerable<string>) new string[1]
      {
        "copy and close"
      }, (Action<int>) (idx =>
      {
        if (idx != 0)
          return;
        Clipboard.SetText(displayString);
      }));
    }

    private static void PaymentInfo_Click(object sender, RoutedEventArgs e)
    {
      Message message = (sender is MenuItem menuItem ? menuItem.Tag : (object) null) is MessageViewModel tag ? tag.Message : (Message) null;
      if (message == null)
        return;
      string displayString = "";
      MessageProperties.PaymentsProperties paymentsPropertiesField = message.InternalProperties?.PaymentsPropertiesField;
      if (paymentsPropertiesField == null)
        displayString += "No matching payment properties found";
      else
        displayString = displayString + "Prop amt: " + (paymentsPropertiesField.Amount ?? "<none>") + " curr: " + (paymentsPropertiesField.Currency ?? "<none>") + " to: " + (paymentsPropertiesField.Receiver ?? "<none>") + " type: " + (paymentsPropertiesField.PayType.ToString() ?? "<none>") + "\n";
      if (PaymentsSettings.IsPaymentsEnabled())
      {
        List<PaymentTransactionInfo> transactions = PaymentTransactionStore.GetTransactions(fmsgId: message.KeyId, fmsgRemoteJid: message.KeyRemoteJid);
        if (transactions != null && transactions.Count > 0)
        {
          foreach (PaymentTransactionInfo paymentTransactionInfo in transactions)
            displayString = displayString + paymentTransactionInfo.TransactionType.ToString() + " " + paymentTransactionInfo.TransactionStatus.ToString() + " " + (object) paymentTransactionInfo.TransactionAmountx1000 + "\n";
        }
        else
          displayString += "No matching payment transactions found";
      }
      if (displayString.Length >= (int) byte.MaxValue)
        displayString = displayString.Replace('\n', '.');
      UIUtils.MessageBox("debug", displayString, (IEnumerable<string>) new string[1]
      {
        "copy and close"
      }, (Action<int>) (idx =>
      {
        if (idx != 0)
          return;
        Clipboard.SetText(displayString);
      }));
    }

    public static IEnumerable<MenuItem> GetMessageMenuItems(MessageViewModel vm)
    {
      Message msg = (Message) null;
      if (vm == null || (msg = vm.Message) == null)
        return (IEnumerable<MenuItem>) null;
      Func<MessageMenu.MessageMenuItem, bool> func = (Func<MessageMenu.MessageMenuItem, bool>) (mmi => vm.ExcludedMenuItems != null && vm.ExcludedMenuItems.Contains(mmi));
      LinkedList<MenuItem> messageMenuItems = new LinkedList<MenuItem>();
      int num = vm.Message.MediaWaType == FunXMPP.FMessage.Type.Divider ? 1 : (vm.Message.MediaWaType == FunXMPP.FMessage.Type.System ? 1 : 0);
      bool flag = JidHelper.IsGroupJid(msg.KeyRemoteJid);
      bool announceOnly = false;
      if (flag)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => announceOnly = db.GetConversation(msg.KeyRemoteJid, CreateOptions.None).IsAnnounceOnlyForUser()));
      if (num == 0)
      {
        bool isStarred = vm.Message.IsStarred;
        if (!func(MessageMenu.MessageMenuItem.Star) | isStarred)
        {
          MenuItem menuItem1 = new MenuItem();
          menuItem1.Header = isStarred ? (object) AppResources.UnstarLower : (object) AppResources.StarLower;
          menuItem1.Tag = (object) vm;
          MenuItem menuItem2 = menuItem1;
          menuItem2.Click += new RoutedEventHandler(MessageMenu.Star_Click);
          messageMenuItems.AddLast(menuItem2);
        }
        if ((!msg.KeyFromMe || msg.Status.GetOverrideWeight() >= FunXMPP.FMessage.Status.ReceivedByServer.GetOverrideWeight()) && !func(MessageMenu.MessageMenuItem.Reply) && !announceOnly)
        {
          MenuItem menuItem3 = new MenuItem();
          menuItem3.Header = (object) AppResources.ReplyToMessage;
          menuItem3.Tag = (object) vm;
          MenuItem menuItem4 = menuItem3;
          menuItem4.Click += new RoutedEventHandler(MessageMenu.Reply_Click);
          messageMenuItems.AddLast(menuItem4);
        }
        if (flag && !msg.KeyFromMe && !func(MessageMenu.MessageMenuItem.ReplyInPrivate))
        {
          MenuItem menuItem5 = new MenuItem();
          menuItem5.Header = (object) AppResources.ReplyToMessageInPrivate;
          menuItem5.Tag = (object) vm;
          MenuItem menuItem6 = menuItem5;
          menuItem6.Click += new RoutedEventHandler(MessageMenu.ReplyInPrivate_Click);
          messageMenuItems.AddLast(menuItem6);
        }
      }
      if (msg.HasText() && !func(MessageMenu.MessageMenuItem.Copy))
      {
        MenuItem menuItem7 = new MenuItem();
        menuItem7.Header = (object) AppResources.Copy;
        menuItem7.Tag = (object) vm;
        MenuItem menuItem8 = menuItem7;
        menuItem8.Click += new RoutedEventHandler(MessageMenu.Copy_Click);
        messageMenuItems.AddLast(menuItem8);
      }
      if (!func(MessageMenu.MessageMenuItem.Delete))
      {
        MenuItem menuItem9 = new MenuItem();
        menuItem9.Header = (object) AppResources.Delete;
        menuItem9.Tag = (object) vm;
        MenuItem menuItem10 = menuItem9;
        menuItem10.Click += new RoutedEventHandler(MessageMenu.Delete_Click);
        messageMenuItems.AddLast(menuItem10);
      }
      bool canDownloadSticker = false;
      if (msg.MediaWaType == FunXMPP.FMessage.Type.Sticker)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => canDownloadSticker = msg.GetSavedSticker(db) == null));
      if (((msg.MediaWaType == FunXMPP.FMessage.Type.Image ? 1 : (msg.MediaWaType == FunXMPP.FMessage.Type.Video ? 1 : 0)) | (canDownloadSticker ? 1 : 0)) != 0 && !func(MessageMenu.MessageMenuItem.SaveMedia) && msg.LocalFileExists())
      {
        MenuItem menuItem11 = new MenuItem();
        menuItem11.Header = (object) AppResources.Save;
        menuItem11.Tag = (object) vm;
        MenuItem menuItem12 = menuItem11;
        menuItem12.Click += new RoutedEventHandler(MessageMenu.Save_Click);
        messageMenuItems.AddLast(menuItem12);
      }
      if (num == 0 && msg.ShouldEnableForward() && !func(MessageMenu.MessageMenuItem.Forward))
      {
        MenuItem menuItem13 = new MenuItem();
        menuItem13.Header = (object) AppResources.Forward;
        menuItem13.Tag = (object) vm;
        MenuItem menuItem14 = menuItem13;
        menuItem14.Click += new RoutedEventHandler(MessageMenu.Forward_Click);
        messageMenuItems.AddLast(menuItem14);
      }
      if (((num != 0 ? 0 : (!msg.KeyFromMe ? 1 : 0)) & (flag ? 1 : 0)) != 0)
      {
        UserStatus sender = vm.Sender;
        if (sender != null || sender.Jid != Settings.MyJid)
        {
          if (!sender.IsInDeviceContactList)
          {
            if (!func(MessageMenu.MessageMenuItem.AddToNewContact))
            {
              MenuItem menuItem15 = new MenuItem();
              menuItem15.Header = (object) AppResources.CreateNewContact;
              menuItem15.Tag = (object) vm;
              MenuItem menuItem16 = menuItem15;
              menuItem16.Click += new RoutedEventHandler(MessageMenu.AddToContacts_Click);
              messageMenuItems.AddLast(menuItem16);
            }
            if (!func(MessageMenu.MessageMenuItem.AddToExistingContact))
            {
              MenuItem menuItem17 = new MenuItem();
              menuItem17.Header = (object) AppResources.AddToExistingContact;
              menuItem17.Tag = (object) vm;
              MenuItem menuItem18 = menuItem17;
              menuItem18.Click += new RoutedEventHandler(MessageMenu.AddToExistingContact_Click);
              messageMenuItems.AddLast(menuItem18);
            }
          }
          if (!func(MessageMenu.MessageMenuItem.SendMessage))
          {
            string displayName = sender.GetDisplayName();
            MenuItem menuItem19 = new MenuItem();
            menuItem19.Header = (object) string.Format(AppResources.OpenSender, (object) displayName);
            menuItem19.Tag = (object) vm;
            MenuItem menuItem20 = menuItem19;
            menuItem20.Click += new RoutedEventHandler(MessageMenu.SendMessage_Click);
            messageMenuItems.AddLast(menuItem20);
          }
        }
      }
      if (num == 0 && msg.KeyFromMe && !func(MessageMenu.MessageMenuItem.ShowDetails) && 1415214000L < msg.FunTimestamp.GetValueOrDefault(FunRunner.CurrentServerTimeUtc).ToUnixTime())
      {
        MenuItem menuItem21 = new MenuItem();
        menuItem21.Header = (object) string.Format(AppResources.MessageDetails);
        menuItem21.Tag = (object) vm;
        MenuItem menuItem22 = menuItem21;
        menuItem22.Click += new RoutedEventHandler(MessageMenu.ShowMessageDetails_Click);
        messageMenuItems.AddLast(menuItem22);
      }
      if (Settings.IsWaAdmin)
      {
        MenuItem menuItem23 = new MenuItem();
        menuItem23.Header = (object) "debug";
        menuItem23.Tag = (object) vm;
        MenuItem menuItem24 = menuItem23;
        menuItem24.Click += new RoutedEventHandler(MessageMenu.DebugMessage_Click);
        messageMenuItems.AddLast(menuItem24);
        if (msg.HasPaymentInfo())
        {
          MenuItem menuItem25 = new MenuItem();
          menuItem25.Header = (object) "payment info";
          menuItem25.Tag = (object) vm;
          MenuItem menuItem26 = menuItem25;
          menuItem26.Click += new RoutedEventHandler(MessageMenu.PaymentInfo_Click);
          messageMenuItems.AddLast(menuItem26);
        }
      }
      return (IEnumerable<MenuItem>) messageMenuItems;
    }

    public enum MessageMenuItem
    {
      Copy,
      Delete,
      Forward,
      AddToNewContact,
      AddToExistingContact,
      SendMessage,
      ShowDetails,
      Reply,
      ReplyInPrivate,
      SaveMedia,
      Star,
    }
  }
}
