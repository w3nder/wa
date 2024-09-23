// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatInfoTabHeader
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WhatsApp.CommonOps;

#nullable disable
namespace WhatsApp
{
  public class ChatInfoTabHeader : UserControl
  {
    private Dictionary<string, JidInfo> jidInfoDict;
    private string chatName;
    private string targetJid;
    internal ListBoxItem ContactCardsPanel;
    internal TextBlock ContactCardsTitleBlock;
    internal TextBlock ContactCardsStateBlock;
    internal ListBoxItem LiveLocationPanel;
    internal TextBlock LiveLocationBlock;
    internal RichTextBlock LiveLocationSharingBlock;
    internal ListBoxItem MediaPanel;
    internal TextBlock MediaTitleBlock;
    internal TextBlock MediaStateBlock;
    internal ListBoxItem StarredMessagesPanel;
    internal TextBlock StarredMessagesTitleBlock;
    internal TextBlock StarredMessagesStateBlock;
    internal ListBoxItem CommonGroupsPanel;
    internal TextBlock CommonGroupsTitleBlock;
    internal TextBlock CommonGroupsStateBlock;
    internal ListBoxItem CustomNotificationsPanel;
    internal TextBlock CustomNotificationsTitleBlock;
    internal TextBlock CustomNotificationsStateBlock;
    internal ListBoxItem MutePanel;
    internal TextBlock MuteTitleBlock;
    internal TextBlock MuteStateBlock;
    internal ListBoxItem SaveMediaPanel;
    internal TextBlock SaveMediaTitleBlock;
    internal TextBlock SaveMediaStateBlock;
    internal ListBoxItem EncryptionPanel;
    internal TextBlock EncryptionTitleBlock;
    internal TextBlock EncryptionStateBlock;
    internal Image EncryptionIcon;
    internal ListBoxItem GroupSettingsPanel;
    internal TextBlock GroupSettingsTitleBlock;
    internal TextBlock GroupSettingsBlock;
    private bool _contentLoaded;

    public ChatInfoTabHeader()
    {
      this.InitializeComponent();
      this.LiveLocationBlock.Text = AppResources.LiveLocationLowerCase;
      this.MediaTitleBlock.Text = AppResources.MediaLinksAndDocs;
      this.CustomNotificationsTitleBlock.Text = AppResources.CustomNotificationsTitle;
      this.EncryptionTitleBlock.Text = AppResources.Encryption;
      this.SaveMediaTitleBlock.Text = AppResources.SaveIncomingMedia;
      this.ContactCardsTitleBlock.Text = AppResources.ContactCardsMatched;
      this.GroupSettingsTitleBlock.Text = AppResources.GroupSettingsTitle;
      this.GroupSettingsBlock.Text = AppResources.GroupSettingsBlockText;
    }

    public void Set(Dictionary<string, JidInfo> jiDict, string chatName, string targetJid)
    {
      this.jidInfoDict = jiDict;
      this.chatName = chatName;
      this.targetJid = targetJid;
      this.UpdateAll();
    }

    public void Set(string jid)
    {
      if (jid == null)
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        this.jidInfoDict = new Dictionary<string, JidInfo>();
        this.jidInfoDict[jid] = db.GetJidInfo(jid, CreateOptions.None);
        if (JidHelper.IsUserJid(jid))
        {
          this.chatName = JidHelper.GetDisplayNameForContactJid(jid);
        }
        else
        {
          Conversation conversation = db.GetConversation(jid, CreateOptions.None);
          if (conversation != null)
            this.chatName = conversation.GroupSubject;
        }
        this.targetJid = jid;
      }));
      this.UpdateAll();
    }

    public void UpdateAll()
    {
      this.UpdateStarredMessagesPanel();
      this.UpdateCommonGroupsPanel();
      this.UpdateCustomNotificationsPanel();
      this.UpdateMutePanel();
      this.UpdateSaveMediaPanel();
      this.UpdateEncryptionPanel();
      this.UpdateMediaPanel();
      this.UpdateContactCardsPanel();
      this.UpdateLiveLocationPanel();
      this.UpdateGroupSettingPanel();
    }

    private void UpdateMediaPanel()
    {
      string[] jids = this.jidInfoDict == null ? new string[0] : this.jidInfoDict.Keys.ToArray<string>();
      if (((IEnumerable<string>) jids).Any<string>())
        AppState.Worker.Enqueue((Action) (() =>
        {
          long n = 0;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => n = db.GetMessagesCount(jids, types: new FunXMPP.FMessage.Type[6]
          {
            FunXMPP.FMessage.Type.Audio,
            FunXMPP.FMessage.Type.Document,
            FunXMPP.FMessage.Type.ExtendedText,
            FunXMPP.FMessage.Type.Image,
            FunXMPP.FMessage.Type.Video,
            FunXMPP.FMessage.Type.Gif
          })));
          Log.d("chatInfoTab", "Count {0}", (object) n);
          this.Dispatcher.BeginInvoke((Action) (() => this.MediaStateBlock.Text = Math.Max(0L, n).ToString()));
        }));
      else
        this.MediaPanel.Visibility = Visibility.Collapsed;
    }

    private void UpdateStarredMessagesPanel()
    {
      string[] jids = this.jidInfoDict == null ? new string[0] : this.jidInfoDict.Keys.ToArray<string>();
      if (((IEnumerable<string>) jids).Any<string>())
        AppState.Worker.Enqueue((Action) (() =>
        {
          long n = 0;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => n = db.GetMessagesCount(jids, true)));
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            if (n > 0L)
            {
              this.StarredMessagesTitleBlock.Text = AppResources.StarredMessagesLower;
              this.StarredMessagesStateBlock.Text = n.ToString();
              this.StarredMessagesPanel.Visibility = Visibility.Visible;
            }
            else
              this.StarredMessagesPanel.Visibility = Visibility.Collapsed;
          }));
        }));
      else
        this.StarredMessagesPanel.Visibility = Visibility.Collapsed;
    }

    private void UpdateContactCardsPanel()
    {
      if (!Settings.IsWaAdmin)
        return;
      string[] strArray = this.jidInfoDict == null ? new string[0] : this.jidInfoDict.Keys.ToArray<string>();
      if (strArray.Length == 1 && JidHelper.IsUserJid(strArray[0]) && !JidHelper.IsJidInAddressBook(strArray[0]))
      {
        string jid = strArray[0];
        int numCards = 0;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => numCards = ((IEnumerable<Message>) db.GetTrustedContactVCardsWithJid(jid)).Count<Message>()));
        if (numCards > 0)
        {
          this.ContactCardsTitleBlock.Text = AppResources.ContactCardsMatched;
          this.ContactCardsStateBlock.Text = numCards.ToString();
          this.ContactCardsPanel.Visibility = Visibility.Visible;
        }
        else
          this.ContactCardsPanel.Visibility = Visibility.Collapsed;
      }
      else
        this.ContactCardsPanel.Visibility = Visibility.Collapsed;
    }

    private void UpdateCommonGroupsPanel()
    {
      string[] jids = this.jidInfoDict == null ? new string[0] : this.jidInfoDict.Keys.ToArray<string>();
      if (((IEnumerable<string>) jids).Any<string>() && ((IEnumerable<string>) jids).All<string>((Func<string, bool>) (jid => JidHelper.IsUserJid(jid))))
        AppState.Worker.Enqueue((Action) (() =>
        {
          int n = 0;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => n = db.GetGroupsInCommon(jids).Length));
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            if (n > 0)
            {
              this.CommonGroupsTitleBlock.Text = AppResources.GroupsInCommon;
              this.CommonGroupsStateBlock.Text = n.ToString();
              this.CommonGroupsPanel.Visibility = Visibility.Visible;
            }
            else
              this.CommonGroupsPanel.Visibility = Visibility.Collapsed;
          }));
        }));
      else
        this.CommonGroupsPanel.Visibility = Visibility.Collapsed;
    }

    private void UpdateCustomNotificationsPanel()
    {
      if ((this.jidInfoDict == null ? 0 : (this.jidInfoDict.Any<KeyValuePair<string, JidInfo>>() ? 1 : 0)) != 0)
      {
        this.CustomNotificationsPanel.Visibility = Visibility.Visible;
        this.CustomNotificationsStateBlock.Text = this.GetCustomNotificationStateStr();
      }
      else
        this.CustomNotificationsPanel.Visibility = Visibility.Collapsed;
    }

    private string GetCustomNotificationStateStr()
    {
      JidInfo[] array1 = this.jidInfoDict.Values.ToArray<JidInfo>();
      if (!((IEnumerable<JidInfo>) array1).Any<JidInfo>((Func<JidInfo, bool>) (ji =>
      {
        if (ji == null)
          return false;
        return ji.RingTone != null || ji.NotificationSound != null;
      })))
        return AppResources.CustomNotificationsOff;
      string tonePath = (string) null;
      string filepath = (string) null;
      bool flag = false;
      string[] array2 = ((IEnumerable<JidInfo>) array1).Where<JidInfo>((Func<JidInfo, bool>) (ji => ji != null && JidHelper.IsUserJid(ji.Jid))).Select<JidInfo, string>((Func<JidInfo, string>) (ji => ji.RingTone ?? "")).MakeUnique<string>().ToArray<string>();
      if (array2.Length == 1)
      {
        tonePath = ((IEnumerable<string>) array2).FirstOrDefault<string>();
        if (string.IsNullOrEmpty(tonePath))
          tonePath = Settings.VoipRingtone ?? "Sounds\\Ring01.wma";
      }
      else if (array2.Length > 1)
        flag = true;
      string[] array3 = ((IEnumerable<JidInfo>) array1).Select<JidInfo, string>((Func<JidInfo, string>) (ji => ji != null ? ji.NotificationSound ?? "" : "")).MakeUnique<string>().ToArray<string>();
      if (array3.Length == 1)
      {
        filepath = ((IEnumerable<string>) array3).FirstOrDefault<string>();
        if (string.IsNullOrEmpty(filepath))
          filepath = Settings.IndividualTone;
      }
      else if (array3.Length > 1)
        flag = true;
      if (flag)
        return AppResources.CustomNotificationsTooltip;
      List<string> values = new List<string>();
      if (!string.IsNullOrEmpty(filepath))
        values.Add(CustomTones.GetNotificationSoundNameForPath(filepath));
      if (!string.IsNullOrEmpty(tonePath))
        values.Add(Ringtones.GetRingtoneNameForPath(tonePath));
      return string.Join(", ", (IEnumerable<string>) values);
    }

    private void UpdateMutePanel()
    {
      if (this.jidInfoDict != null && this.jidInfoDict.Any<KeyValuePair<string, JidInfo>>())
      {
        string str1 = this.jidInfoDict.Any<KeyValuePair<string, JidInfo>>((Func<KeyValuePair<string, JidInfo>, bool>) (p => p.Value != null && p.Value.IsMuted())) ? AppResources.MutedTitle : AppResources.MuteTitle;
        DateTime[] array = this.jidInfoDict.Where<KeyValuePair<string, JidInfo>>((Func<KeyValuePair<string, JidInfo>, bool>) (p => p.Value != null && p.Value.IsMuted())).Select<KeyValuePair<string, JidInfo>, DateTime>((Func<KeyValuePair<string, JidInfo>, DateTime>) (p => p.Value.MuteExpirationUtc.Value)).ToArray<DateTime>();
        string str2 = !((IEnumerable<DateTime>) array).Any<DateTime>() ? AppResources.Mute0 : string.Format(AppResources.Muted, (object) DateTimeUtils.FormatMuteEndTime(DateTimeUtils.FunTimeToPhoneTime(((IEnumerable<DateTime>) array).Max<DateTime>())));
        this.MuteTitleBlock.Text = str1;
        this.MuteStateBlock.Text = str2;
        if (((IEnumerable<DateTime>) array).Any<DateTime>())
        {
          this.MuteStateBlock.Foreground = (Brush) UIUtils.AccentBrush;
          this.MuteStateBlock.FontWeight = FontWeights.SemiBold;
        }
        else
        {
          this.MuteStateBlock.Foreground = UIUtils.SubtleBrush;
          this.MuteStateBlock.FontWeight = FontWeights.Normal;
        }
      }
      else
        this.MutePanel.Visibility = Visibility.Collapsed;
    }

    private void UpdateEncryptionPanel()
    {
      if (this.targetJid != null && !JidHelper.IsPsaJid(this.targetJid))
      {
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          if (JidHelper.IsUserJid(this.targetJid))
          {
            this.EncryptionIcon.Source = (System.Windows.Media.ImageSource) AssetStore.LoadAsset("encrypted.png");
            this.EncryptionStateBlock.Text = string.Format("{0} {1}", (object) AppResources.EncryptedIndividual, (object) AppResources.TapToVerify);
          }
          else
          {
            if (!JidHelper.IsGroupJid(this.targetJid))
              return;
            this.EncryptionIcon.Source = (System.Windows.Media.ImageSource) AssetStore.LoadAsset("encrypted.png");
            this.EncryptionStateBlock.Text = AppResources.EncryptedGroup;
          }
        }));
        this.EncryptionPanel.Visibility = Visibility.Visible;
      }
      else
        this.EncryptionPanel.Visibility = Visibility.Collapsed;
    }

    private void UpdateSaveMediaPanel()
    {
      if (this.jidInfoDict != null && this.jidInfoDict.Any<KeyValuePair<string, JidInfo>>())
      {
        JidInfo jidInfo = this.jidInfoDict.FirstOrDefault<KeyValuePair<string, JidInfo>>((Func<KeyValuePair<string, JidInfo>, bool>) (p => p.Value != null)).Value;
        bool? firstState = jidInfo == null ? new bool?() : jidInfo.SaveMediaToPhone;
        this.SaveMediaStateBlock.Text = !this.jidInfoDict.Any<KeyValuePair<string, JidInfo>>((Func<KeyValuePair<string, JidInfo>, bool>) (p =>
        {
          if (p.Value == null)
            return false;
          bool? saveMediaToPhone = p.Value.SaveMediaToPhone;
          bool? nullable = firstState;
          return saveMediaToPhone.GetValueOrDefault() != nullable.GetValueOrDefault() || saveMediaToPhone.HasValue != nullable.HasValue;
        })) ? (!firstState.HasValue ? (Settings.SaveIncomingMedia ? AppResources.SaveIncomingMediaDefaultOn : AppResources.SaveIncomingMediaDefaultOff) : (firstState.Value ? AppResources.SaveIncomingMediaYes : AppResources.SaveIncomingMediaNo)) : AppResources.SaveIncomingMediaTooltip;
      }
      else
        this.SaveMediaPanel.Visibility = Visibility.Collapsed;
    }

    private void UpdateLiveLocationPanel()
    {
      int receivingForGroupCount = LiveLocationManager.Instance.GetJidsReceivingForGroupCount(this.targetJid);
      bool flag = LiveLocationManager.Instance.IsSharingLocationWithJid(this.targetJid);
      if (flag || receivingForGroupCount > 0)
      {
        this.LiveLocationPanel.Visibility = Visibility.Visible;
        this.LiveLocationSharingBlock.Text = new RichTextBlock.TextSet()
        {
          Text = !flag ? (!JidHelper.IsGroupJid(this.targetJid) ? string.Format(AppResources.LiveLocationContactCurrentlySharing, (object) this.chatName) : Plurals.Instance.GetString(AppResources.LiveLocationPeopleCurrentlySharingPlural, receivingForGroupCount)) : (receivingForGroupCount != 0 ? (!JidHelper.IsGroupJid(this.targetJid) ? string.Format(AppResources.LiveLocationYouAndContactCurrentlySharing, (object) this.chatName) : Plurals.Instance.GetString(AppResources.LiveLocationYouAndPeopleCurrentlySharingPlural, receivingForGroupCount)) : AppResources.LiveLocationYouCurrentlySharing)
        };
      }
      else
        this.LiveLocationPanel.Visibility = Visibility.Collapsed;
    }

    public void UpdateGroupSettingPanel()
    {
      if (!Settings.RestrictGroups && Settings.AnnouncementGroupSize <= 0)
      {
        this.GroupSettingsPanel.Visibility = Visibility.Collapsed;
      }
      else
      {
        Conversation convo = (Conversation) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(this.targetJid, CreateOptions.None)));
        if (convo != null && convo.JidType == JidHelper.JidTypes.Group && convo.IsGroupParticipant() && convo.UserIsAdmin(Settings.MyJid))
          this.GroupSettingsPanel.Visibility = Visibility.Visible;
        else
          this.GroupSettingsPanel.Visibility = Visibility.Collapsed;
      }
    }

    private void MediaPanel_Tap(object sender, GestureEventArgs e)
    {
      string[] strArray = this.jidInfoDict == null ? new string[0] : this.jidInfoDict.Keys.ToArray<string>();
      if (!((IEnumerable<string>) strArray).Any<string>())
        return;
      MediaGalleryPage.Start(this.chatName, strArray);
    }

    private void StarredMessagesPanel_Tap(object sender, GestureEventArgs e)
    {
      string[] strArray = this.jidInfoDict == null ? new string[0] : this.jidInfoDict.Keys.ToArray<string>();
      if (!((IEnumerable<string>) strArray).Any<string>())
        return;
      StarredMessagesPage.Start(strArray);
    }

    private void CustomNotificationsPanel_Tap(object sender, GestureEventArgs e)
    {
      CustomTonesPage.Start(this.chatName, this.jidInfoDict);
    }

    private void ContactCardsPanel_Tap(object sender, GestureEventArgs e)
    {
      string[] source = this.jidInfoDict == null ? new string[0] : this.jidInfoDict.Keys.ToArray<string>();
      if (((IEnumerable<string>) source).Count<string>() != 1)
        return;
      ContactVCardPage.Start(((IEnumerable<string>) source).First<string>());
    }

    private void CommonGroupsPanel_Tap(object sender, GestureEventArgs e)
    {
      string[] strArray = this.jidInfoDict == null ? new string[0] : this.jidInfoDict.Keys.ToArray<string>();
      if (!((IEnumerable<string>) strArray).Any<string>())
        return;
      CommonGroupsPage.Start(strArray);
    }

    private void MutePanel_Tap(object sender, GestureEventArgs e)
    {
      string[] strArray = this.jidInfoDict == null ? new string[0] : this.jidInfoDict.Keys.ToArray<string>();
      if (!((IEnumerable<string>) strArray).Any<string>())
        return;
      MuteChatPicker.Launch(strArray).Subscribe<Unit>();
    }

    private void SaveMediaPanel_Tap(object sender, GestureEventArgs e)
    {
      string[] strArray = this.jidInfoDict == null ? new string[0] : this.jidInfoDict.Keys.ToArray<string>();
      if (!((IEnumerable<string>) strArray).Any<string>())
        return;
      SaveIncomingMediaPicker.Launch(strArray).ObserveOnDispatcher<Unit>().Subscribe<Unit>();
    }

    private void EncryptionPanel_Tap(object sender, GestureEventArgs e)
    {
      if (this.targetJid == null)
        return;
      if (JidHelper.IsUserJid(this.targetJid))
      {
        NavUtils.VerifyIdentityForJid(this.targetJid);
      }
      else
      {
        if (!JidHelper.IsGroupJid(this.targetJid))
          return;
        UIUtils.MessageBox(" ", AppResources.EncryptedGroupDetails, (IEnumerable<string>) new string[2]
        {
          AppResources.OkLower,
          AppResources.LearnMoreButton
        }, (Action<int>) (idx =>
        {
          if (idx != 1)
            return;
          new WebBrowserTask()
          {
            Uri = new Uri(WaWebUrls.FaqUrlGroupE2e)
          }.Show();
        }));
      }
    }

    private void LiveLocation_Tap(object sender, GestureEventArgs e)
    {
      LiveLocationView.Start(new Message()
      {
        KeyRemoteJid = this.targetJid
      });
    }

    private void GroupSettings_Tap(object sender, GestureEventArgs e)
    {
      GroupSettingsPage.Start(this.chatName, this.targetJid);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ChatInfoTabHeader.xaml", UriKind.Relative));
      this.ContactCardsPanel = (ListBoxItem) this.FindName("ContactCardsPanel");
      this.ContactCardsTitleBlock = (TextBlock) this.FindName("ContactCardsTitleBlock");
      this.ContactCardsStateBlock = (TextBlock) this.FindName("ContactCardsStateBlock");
      this.LiveLocationPanel = (ListBoxItem) this.FindName("LiveLocationPanel");
      this.LiveLocationBlock = (TextBlock) this.FindName("LiveLocationBlock");
      this.LiveLocationSharingBlock = (RichTextBlock) this.FindName("LiveLocationSharingBlock");
      this.MediaPanel = (ListBoxItem) this.FindName("MediaPanel");
      this.MediaTitleBlock = (TextBlock) this.FindName("MediaTitleBlock");
      this.MediaStateBlock = (TextBlock) this.FindName("MediaStateBlock");
      this.StarredMessagesPanel = (ListBoxItem) this.FindName("StarredMessagesPanel");
      this.StarredMessagesTitleBlock = (TextBlock) this.FindName("StarredMessagesTitleBlock");
      this.StarredMessagesStateBlock = (TextBlock) this.FindName("StarredMessagesStateBlock");
      this.CommonGroupsPanel = (ListBoxItem) this.FindName("CommonGroupsPanel");
      this.CommonGroupsTitleBlock = (TextBlock) this.FindName("CommonGroupsTitleBlock");
      this.CommonGroupsStateBlock = (TextBlock) this.FindName("CommonGroupsStateBlock");
      this.CustomNotificationsPanel = (ListBoxItem) this.FindName("CustomNotificationsPanel");
      this.CustomNotificationsTitleBlock = (TextBlock) this.FindName("CustomNotificationsTitleBlock");
      this.CustomNotificationsStateBlock = (TextBlock) this.FindName("CustomNotificationsStateBlock");
      this.MutePanel = (ListBoxItem) this.FindName("MutePanel");
      this.MuteTitleBlock = (TextBlock) this.FindName("MuteTitleBlock");
      this.MuteStateBlock = (TextBlock) this.FindName("MuteStateBlock");
      this.SaveMediaPanel = (ListBoxItem) this.FindName("SaveMediaPanel");
      this.SaveMediaTitleBlock = (TextBlock) this.FindName("SaveMediaTitleBlock");
      this.SaveMediaStateBlock = (TextBlock) this.FindName("SaveMediaStateBlock");
      this.EncryptionPanel = (ListBoxItem) this.FindName("EncryptionPanel");
      this.EncryptionTitleBlock = (TextBlock) this.FindName("EncryptionTitleBlock");
      this.EncryptionStateBlock = (TextBlock) this.FindName("EncryptionStateBlock");
      this.EncryptionIcon = (Image) this.FindName("EncryptionIcon");
      this.GroupSettingsPanel = (ListBoxItem) this.FindName("GroupSettingsPanel");
      this.GroupSettingsTitleBlock = (TextBlock) this.FindName("GroupSettingsTitleBlock");
      this.GroupSettingsBlock = (TextBlock) this.FindName("GroupSettingsBlock");
    }
  }
}
