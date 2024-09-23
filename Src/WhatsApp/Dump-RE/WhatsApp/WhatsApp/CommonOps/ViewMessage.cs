// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.ViewMessage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using WhatsApp.Events;
using WhatsApp.UtilsFrontend;
using WhatsApp.WaCollections;
using WhatsAppNative;
using Windows.Storage;
using Windows.System;

#nullable disable
namespace WhatsApp.CommonOps
{
  public static class ViewMessage
  {
    public static void View(Message msg, bool viewThisOnly = false, bool forceDownload = false)
    {
      if (msg == null)
        return;
      Log.d("view msg", "{0} | localfile:{1}", (object) msg.LogInfo(), (object) msg.LocalFileUri);
      if (msg.IsPtt())
      {
        ViewMessage.PlayPtt(msg);
      }
      else
      {
        bool flag1 = false;
        switch (msg.MediaWaType)
        {
          case FunXMPP.FMessage.Type.Image:
          case FunXMPP.FMessage.Type.Audio:
          case FunXMPP.FMessage.Type.Document:
          case FunXMPP.FMessage.Type.Sticker:
            if (!flag1 && string.IsNullOrEmpty(msg.LocalFileUri))
            {
              ViewMessage.ToggleDownload(msg);
              break;
            }
            if (msg.MediaWaType == FunXMPP.FMessage.Type.Image)
            {
              if (msg.LocalFileExists())
              {
                bool flag2 = false;
                switch (MessageExtensions.GetUploadActionState(msg))
                {
                  case MessageExtensions.MediaUploadActionState.Retryable:
                    MediaUploadServices.RetryMediaMessageSend(msg);
                    flag2 = true;
                    break;
                  case MessageExtensions.MediaUploadActionState.Cancellable:
                    MediaUploadServices.CancelMediaMessageSend(msg);
                    flag2 = true;
                    break;
                }
                if (flag2)
                  break;
                Message startingMsg = msg;
                string[] jids;
                if (!viewThisOnly)
                  jids = new string[1]{ msg.KeyRemoteJid };
                else
                  jids = (string[]) null;
                string keyRemoteJid = msg.KeyRemoteJid;
                ChatMediaPage.Start(startingMsg, jids, keyRemoteJid);
                break;
              }
              Log.l("view image msg", "file is missing | {0} | localfile:{1}", (object) msg.LogInfo(), (object) msg.LocalFileUri);
              int num = (int) MessageBox.Show(AppResources.MediaNotFound);
              break;
            }
            if (msg.MediaWaType == FunXMPP.FMessage.Type.Audio)
            {
              ViewMessage.ToggleAudioPlayback(msg);
              break;
            }
            if (msg.MediaWaType == FunXMPP.FMessage.Type.Document)
            {
              ViewMessage.LaunchDocumentViewer(msg.LocalFileUri);
              break;
            }
            if (msg.MediaWaType == FunXMPP.FMessage.Type.Video || msg.MediaWaType == FunXMPP.FMessage.Type.Gif)
            {
              if (flag1 || msg.LocalFileExists())
              {
                bool flag3 = false;
                switch (MessageExtensions.GetUploadActionState(msg))
                {
                  case MessageExtensions.MediaUploadActionState.Retryable:
                    MediaUploadServices.RetryMediaMessageSend(msg);
                    flag3 = true;
                    break;
                  case MessageExtensions.MediaUploadActionState.Cancellable:
                    MediaUploadServices.CancelMediaMessageSend(msg);
                    flag3 = true;
                    break;
                }
                if (flag3)
                  break;
                VideoPlay fsEvent = new VideoPlay();
                fsEvent.videoDuration = new long?((long) msg.MediaDurationSeconds);
                VideoPlay videoPlay = fsEvent;
                DateTime now = DateTime.Now;
                DateTime? localTimestamp = msg.LocalTimestamp;
                long? nullable = new long?((long) (int) (localTimestamp.HasValue ? new TimeSpan?(now - localTimestamp.GetValueOrDefault()) : new TimeSpan?()).Value.TotalSeconds);
                videoPlay.videoAge = nullable;
                fsEvent.videoSize = new double?((double) msg.MediaSize);
                int num = (int) AudioPlaybackManager.BackgroundMedia.Stop();
                new VideoPlayback(msg).LaunchVideoPlayer(fsEvent);
                break;
              }
              Log.l("view video msg", "file is missing | {0} | localfile:{1}", (object) msg.LogInfo(), (object) msg.LocalFileUri);
              int num1 = (int) MessageBox.Show(AppResources.MediaNotFound);
              break;
            }
            if (msg.MediaWaType != FunXMPP.FMessage.Type.Sticker)
              break;
            Log.l("view sticker", "not implemented | {0} | localfile:{1}", (object) msg.LogInfo(), (object) msg.LocalFileUri);
            break;
          case FunXMPP.FMessage.Type.Video:
          case FunXMPP.FMessage.Type.Gif:
            flag1 = !forceDownload && msg.HasSidecar();
            goto case FunXMPP.FMessage.Type.Image;
          case FunXMPP.FMessage.Type.Contact:
            IEnumerable<ContactVCard> contactVcards = (IEnumerable<ContactVCard>) ((object) msg.GetContactCards() ?? (object) new ContactVCard[0]);
            if (contactVcards == null)
              break;
            if (msg.KeyFromMe)
            {
              ShareContactPage.Start(contactVcards, true).Subscribe<ContactVCard>();
              break;
            }
            if (msg.HasMultipleContacts())
            {
              BulkContactCardViewPage.Start(msg.MediaName, contactVcards);
              break;
            }
            contactVcards.FirstOrDefault<ContactVCard>().ToSaveContactTask().GetShowTaskAsync<SaveContactResult>().Subscribe<IEvent<SaveContactResult>>();
            break;
          case FunXMPP.FMessage.Type.Location:
            LocationView.Start(msg.MessageID);
            break;
          case FunXMPP.FMessage.Type.System:
            string message = (string) null;
            switch (msg.GetSystemMessageType())
            {
              case SystemMessageWrapper.MessageTypes.Rename:
              case SystemMessageWrapper.MessageTypes.GroupParticipantNumberChanged:
                string keyRemoteJid1 = msg.KeyRemoteJid;
                string newJid = msg.RemoteResource;
                string str = newJid;
                if (keyRemoteJid1.Equals(str))
                  return;
                bool isNewKnown = false;
                string changeNumberText = SystemMessageUtils.GetChangeNumberText(msg);
                ContactsContext.Instance((Action<ContactsContext>) (db =>
                {
                  UserStatus userStatus = db.GetUserStatus(newJid, false);
                  isNewKnown = userStatus != null && userStatus.IsInDevicePhonebook;
                }));
                string[] buttonTitles;
                if (isNewKnown)
                  buttonTitles = new string[1]
                  {
                    AppResources.ChangeNumberDialogMessageButton
                  };
                else
                  buttonTitles = new string[2]
                  {
                    AppResources.ChangeNumberDialogMessageButton,
                    AppResources.ChangeNumberDialogAddNumberButton
                  };
                MessageBoxControl.Show(" ", changeNumberText, (IEnumerable<string>) buttonTitles, (Action<int>) (idx =>
                {
                  if (idx == 0)
                    NavUtils.NavigateToChat(newJid, true);
                  if (idx != 1)
                    return;
                  AddContact.Launch(newJid, isNewKnown);
                }), true);
                return;
              case SystemMessageWrapper.MessageTypes.BroadcastListCreated:
                return;
              case SystemMessageWrapper.MessageTypes.Error:
                return;
              case SystemMessageWrapper.MessageTypes.GainedAdmin:
                return;
              case SystemMessageWrapper.MessageTypes.GroupDeleted:
                return;
              case SystemMessageWrapper.MessageTypes.GroupCreated:
                return;
              case SystemMessageWrapper.MessageTypes.IdentityChanged:
                UIUtils.MessageBox(" ", string.Format(AppResources.SecurityCodeChangedDescription, (object) JidHelper.GetDisplayNameForContactJid(msg.GetSenderJid())), (IEnumerable<string>) new string[2]
                {
                  AppResources.OkLower,
                  AppResources.LearnMoreButton
                }, (Action<int>) (idx =>
                {
                  if (idx != 1)
                    return;
                  new WebBrowserTask()
                  {
                    Uri = new Uri(WaWebUrls.FaqUrlSecurityCodeChanged)
                  }.Show();
                }));
                return;
              case SystemMessageWrapper.MessageTypes.ConversationEncrypted:
                string url = (string) null;
                if (JidHelper.IsGroupJid(msg.KeyRemoteJid))
                  message = AppResources.GroupEnabledEncryptedDetails;
                else if (JidHelper.IsBroadcastJid(msg.KeyRemoteJid))
                {
                  message = AppResources.BroadcastEnabledEncryptedDetails;
                }
                else
                {
                  if (JidHelper.IsUserJid(msg.KeyRemoteJid))
                  {
                    UserStatus user = UserCache.Get(msg.KeyRemoteJid, false);
                    if ((user != null ? (user.IsEnterprise() ? 1 : 0) : 0) != 0)
                    {
                      message = string.Format(AppResources.ChatEnabledEncryptedDetailsEnterprise, (object) (user.GetVerifiedNameForDisplay() ?? JidHelper.GetPhoneNumber(msg.KeyRemoteJid, true)));
                      url = WaWebUrls.GetFaqUrlGeneral("26000103");
                    }
                  }
                  if (message == null)
                    message = AppResources.ChatEnabledEncryptedDetails;
                }
                if (url == null)
                  url = WaWebUrls.SecurityUrl;
                UIUtils.ShowMessageBoxWithLearnMore(message, url);
                return;
              case SystemMessageWrapper.MessageTypes.MissedCall:
                CallContact.Call(msg.GetSenderJid(), context: "from system message");
                return;
              case SystemMessageWrapper.MessageTypes.MissedVideoCall:
                CallContact.VideoCall(msg.GetSenderJid());
                return;
              case SystemMessageWrapper.MessageTypes.GroupInviteChanged:
                return;
              case SystemMessageWrapper.MessageTypes.ConvBizIsVerified:
                UIUtils.ShowMessageBoxWithGeneralLearnMore(string.Format(AppResources.VerifiedHighChatAlert, (object) (SystemMessageUtils.ExtractVnameFromBizSystemMessageData(msg.BinaryData, msg.KeyRemoteJid) ?? "?")), "26000052");
                return;
              case SystemMessageWrapper.MessageTypes.ConvBizIsUnVerified:
                UIUtils.ShowMessageBoxWithGeneralLearnMore(string.Format(AppResources.VerifiedLowUnknownChatAlert, (object) (SystemMessageUtils.ExtractVnameFromBizSystemMessageData(msg.BinaryData, msg.KeyRemoteJid) ?? "?")), "26000063");
                return;
              case SystemMessageWrapper.MessageTypes.ConvBizNowStandard:
                UIUtils.ShowMessageBoxWithGeneralLearnMore(string.Format(AppResources.VerifiedNowStandardTransitionAlert, (object) (SystemMessageUtils.ExtractVnameFromBizSystemMessageData(msg.BinaryData, msg.KeyRemoteJid) ?? "?")), "26000066");
                return;
              case SystemMessageWrapper.MessageTypes.ConvBizNowUnverified:
                UIUtils.ShowMessageBoxWithGeneralLearnMore(string.Format(AppResources.VerifiedNowUnverifiedTransitionAlert, (object) (SystemMessageUtils.ExtractVnameFromBizSystemMessageData(msg.BinaryData, msg.KeyRemoteJid) ?? "?")), "26000064");
                return;
              case SystemMessageWrapper.MessageTypes.GroupPhotoChange | SystemMessageWrapper.MessageTypes.ConvBizNowStandard:
                return;
              case SystemMessageWrapper.MessageTypes.ConvBizNowVerified:
                UIUtils.ShowMessageBoxWithGeneralLearnMore(string.Format(AppResources.VerifiedNowVerifiedTransitionAlert, (object) (SystemMessageUtils.ExtractVnameFromBizSystemMessageData(msg.BinaryData, msg.KeyRemoteJid) ?? "?")), "26000065");
                return;
              case SystemMessageWrapper.MessageTypes.GroupDescriptionChanged:
                Conversation convo = (Conversation) null;
                MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(msg.KeyRemoteJid, CreateOptions.None)));
                if (convo == null)
                  return;
                GroupInfoPage.Start((NavigationService) null, convo, true);
                return;
              case SystemMessageWrapper.MessageTypes.VerifiedBizInitial:
                Pair<string, string> verifiedBizInitial = SystemMessageUtils.GetPopupMessageForVerifiedBizInitial(msg);
                UIUtils.ShowMessageBoxWithGeneralLearnMore(verifiedBizInitial.First, verifiedBizInitial.Second);
                return;
              case SystemMessageWrapper.MessageTypes.VerifiedBizTransit:
                Pair<string, string> verifiedBizTransit = SystemMessageUtils.GetPopupMessageForVerifiedBizTransit(msg);
                UIUtils.ShowMessageBoxWithGeneralLearnMore(verifiedBizTransit.First, verifiedBizTransit.Second);
                return;
              case SystemMessageWrapper.MessageTypes.GroupRestrictionLocked:
                return;
              case SystemMessageWrapper.MessageTypes.GroupRestrictionUnlocked:
                return;
              case SystemMessageWrapper.MessageTypes.GroupMadeAnnouncementOnly:
                return;
              case SystemMessageWrapper.MessageTypes.GroupMadeNotAnnouncementOnly:
                return;
              case SystemMessageWrapper.MessageTypes.LostAdmin:
                return;
              case SystemMessageWrapper.MessageTypes.GroupDescriptionDeleted:
                return;
              case SystemMessageWrapper.MessageTypes.VerifiedBizInitial2Tier:
                Pair<string, string> verifiedBizInitial2Tier = SystemMessageUtils.GetPopupMessageForVerifiedBizInitial2Tier(msg);
                UIUtils.ShowMessageBoxWithGeneralLearnMore(verifiedBizInitial2Tier.First, verifiedBizInitial2Tier.Second);
                return;
              case SystemMessageWrapper.MessageTypes.VerifiedBizTransit2Tier:
                Pair<string, string> verifiedBizTransit2Tier = SystemMessageUtils.GetPopupMessageForVerifiedBizTransit2Tier(msg);
                UIUtils.ShowMessageBoxWithGeneralLearnMore(verifiedBizTransit2Tier.First, verifiedBizTransit2Tier.Second);
                return;
              case SystemMessageWrapper.MessageTypes.VerifiedBizOneTime2Tier:
                Pair<string, string> verifiedBizOneTime2Tier = SystemMessageUtils.GetPopupMessageForVerifiedBizOneTime2Tier(msg);
                UIUtils.ShowMessageBoxWithGeneralLearnMore(verifiedBizOneTime2Tier.First, verifiedBizOneTime2Tier.Second);
                return;
              default:
                return;
            }
          case FunXMPP.FMessage.Type.LiveLocation:
            LiveLocationView.Start(msg);
            break;
        }
      }
    }

    public static void ToggleDownload(Message msg)
    {
      if (msg.DownloadInProgress)
      {
        Log.l("view msg", "cancel downloading | {0}", (object) msg.LogInfo());
        WhatsApp.MediaDownload.CancelMessageDownload(msg);
      }
      else
        ViewMessage.Download(msg);
    }

    public static void Download(Message msg)
    {
      if (!string.IsNullOrEmpty(msg.LocalFileUri))
        return;
      DiskSpace diskSpace = NativeInterfaces.Misc.GetDiskSpace(Constants.IsoStorePath);
      bool flag = false;
      if (diskSpace.FreeBytes / 1024UL / 1024UL < 15UL)
      {
        int num = (int) MessageBox.Show(AppResources.CriticalStoragePopUp, AppResources.CriticalStorageSpaceTitle, MessageBoxButton.OK);
        flag = true;
      }
      else if (diskSpace.FreeBytes / 1024UL / 1024UL < 100UL && MessageBox.Show(AppResources.LowStoragePopUp, AppResources.LowStorageSpaceTitle, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
        flag = true;
      if (flag)
        Log.l("view msg", "skip download | {0}", (object) msg.LogInfo());
      else if (msg.DownloadInProgress)
      {
        Log.l("view msg", "download already in progress | {0}", (object) msg.LogInfo());
      }
      else
      {
        Log.l("view msg", "download now | {0}", (object) msg.LogInfo());
        WhatsApp.Events.MediaDownload mediaDownloadEvent = FieldStats.GetFsMediaDownloadEvent(msg);
        msg.SetPendingMediaSubscription("user initiated media download", PendingMediaTransfer.TransferTypes.Download_Foreground_Interactive, WhatsApp.MediaDownload.TransferForMessageObservable(msg, WhatsApp.MediaDownload.TransferFromForeground(msg, mediaDownloadEvent, true), mediaDownloadEvent));
      }
    }

    private static void ToggleAudioPlayback(Message msg)
    {
      PlayAudioMessage instance = PlayAudioMessage.GetInstance(true);
      if (msg.PlaybackInProgress)
        instance.Player.Pause();
      else
        instance.Player.Play(msg);
    }

    private static void PlayPtt(Message msg)
    {
      if (msg == null)
        return;
      if (msg.LocalFileExists())
      {
        bool skipPlaybackAction = false;
        if (msg.KeyFromMe)
        {
          bool reupload = false;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            switch (msg.Status)
            {
              case FunXMPP.FMessage.Status.Uploading:
              case FunXMPP.FMessage.Status.UploadingCustomHash:
                msg.Status = FunXMPP.FMessage.Status.Canceled;
                db.SubmitChanges();
                skipPlaybackAction = true;
                break;
              case FunXMPP.FMessage.Status.Canceled:
                msg.Status = FunXMPP.FMessage.Status.Uploading;
                db.SubmitChanges();
                skipPlaybackAction = true;
                reupload = true;
                break;
            }
          }));
          if (reupload)
            msg.SetPendingMediaSubscription("ptt reupload", PendingMediaTransfer.TransferTypes.Upload_NotWeb, WhatsApp.MediaUpload.SendMediaObservable(msg));
        }
        if (skipPlaybackAction)
          return;
        ViewMessage.ToggleAudioPlayback(msg);
      }
      else if (string.IsNullOrEmpty(msg.LocalFileUri))
      {
        WhatsApp.Events.MediaDownload mediaDownloadEvent = FieldStats.GetFsMediaDownloadEvent(msg);
        msg.SetPendingMediaSubscription("Media download", PendingMediaTransfer.TransferTypes.Download_Foreground_Interactive, WhatsApp.MediaDownload.TransferForMessageObservable(msg, WhatsApp.MediaDownload.TransferFromForeground(msg, mediaDownloadEvent, true), mediaDownloadEvent));
      }
      else
      {
        Log.l(msg.LogInfo(), "ptt playback | file missing | path={0}", (object) msg.LocalFileUri);
        int num = (int) MessageBox.Show(AppResources.MediaNotFound);
      }
    }

    public static async void LaunchDocumentViewer(string filepath)
    {
      if (string.IsNullOrEmpty(filepath))
        return;
      filepath = MediaStorage.GetAbsolutePath(filepath);
      int length = filepath.LastIndexOf('\\');
      string str1 = filepath.Substring(0, length);
      string str2 = filepath.Substring(length + 1);
      Log.l("view msg", "view document | filepath:{0},dir:{1},filename:{2}", (object) filepath, (object) str1, (object) str2);
      int num = await Launcher.LaunchFileAsync((IStorageFile) await StorageFile.GetFileFromPathAsync(filepath)) ? 1 : 0;
    }

    public static void StopOnDispose(Message msg, FunXMPP.FMessage.Type funType)
    {
      if (msg == null || !msg.PlaybackInProgress)
        return;
      Log.d(nameof (StopOnDispose), "{0} | {1} | {2}", (object) msg.KeyId, (object) msg.MediaWaType, (object) funType);
      switch (funType)
      {
        case FunXMPP.FMessage.Type.Audio:
          PlayAudioMessage instance = PlayAudioMessage.GetInstance(false);
          if (!msg.PlaybackInProgress || instance == null)
            break;
          instance.Player.Stop();
          break;
        case FunXMPP.FMessage.Type.Video:
        case FunXMPP.FMessage.Type.Gif:
          Log.l(nameof (StopOnDispose), "Not implemented yet for {0}", (object) funType);
          break;
      }
    }
  }
}
