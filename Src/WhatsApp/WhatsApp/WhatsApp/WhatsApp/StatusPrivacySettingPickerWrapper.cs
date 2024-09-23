// Decompiled with JetBrains decompiler
// Type: WhatsApp.StatusPrivacySettingPickerWrapper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class StatusPrivacySettingPickerWrapper
  {
    private ListPicker picker;

    public StatusPrivacySettingPickerWrapper(ListPicker privacyPicker)
    {
      this.picker = privacyPicker;
      this.picker.SelectionChanged += new SelectionChangedEventHandler(this.Picker_SelectionChanged);
      this.ReloadOptions();
    }

    public void ReloadOptions()
    {
      Pair<List<string>, int> options = this.GetOptions();
      this.picker.ItemsSource = (IEnumerable) options.First;
      this.picker.SelectedIndex = options.Second;
    }

    private Pair<List<string>, int> GetOptions()
    {
      List<string> first = new List<string>();
      first.Add(AppResources.StatusPrivacyOptionContacts);
      int whiteListCount = 0;
      int blackListCount = 0;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        whiteListCount = WaStatusHelper.GetWhiteListCount(db);
        blackListCount = WaStatusHelper.GetBlackListCount(db);
      }));
      if (whiteListCount > 0)
        first.Add(Plurals.Instance.GetString(AppResources.StatusPrivacyOptionWhiteListNPlural, whiteListCount));
      else
        first.Add(AppResources.StatusPrivacyOptionWhiteList);
      if (blackListCount > 0)
        first.Add(Plurals.Instance.GetString(AppResources.StatusPrivacyOptionBlackListNPlural, blackListCount));
      else
        first.Add(AppResources.StatusPrivacyOptionBlackList);
      int index;
      switch (Settings.StatusV3PrivacySetting)
      {
        case WaStatusHelper.StatusPrivacySettings.WhiteList:
          index = 1;
          break;
        case WaStatusHelper.StatusPrivacySettings.BlackList:
          index = 2;
          break;
        default:
          index = 0;
          break;
      }
      string str = string.Format("{0}  ", (object) first[index]);
      first.Insert(0, str);
      return new Pair<List<string>, int>(first, 0);
    }

    private void Picker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      int num = this.picker.SelectedIndex - 1;
      Log.l("status privacy", "sel index:{0}, sel setting:{1}", (object) this.picker.SelectedIndex, (object) num);
      switch (num)
      {
        case 0:
          Settings.StatusV3PrivacySetting = WaStatusHelper.StatusPrivacySettings.Contacts;
          break;
        case 1:
          Settings.StatusV3PrivacySetting = WaStatusHelper.StatusPrivacySettings.WhiteList;
          break;
        case 2:
          Settings.StatusV3PrivacySetting = WaStatusHelper.StatusPrivacySettings.BlackList;
          break;
        default:
          return;
      }
      Log.l("status privacy", "new setting:{0}", (object) Settings.StatusV3PrivacySetting);
      this.ReloadOptions();
      WaStatusHelper.SendCurrentPrivacySetting();
      switch (Settings.StatusV3PrivacySetting)
      {
        case WaStatusHelper.StatusPrivacySettings.WhiteList:
          Dictionary<string, GroupParticipantState> whiteList = (Dictionary<string, GroupParticipantState>) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => whiteList = WaStatusHelper.GetWhiteList((SqliteMessagesContext) db)));
          RecipientListPage.StartContactPicker(AppResources.StatusPrivacyOptionWhiteList, (IEnumerable<string>) whiteList.Keys, false, (Brush) UIUtils.SelectionBrush, selectionCountFormat: AppResources.NContactsPlural).ObserveOnDispatcher<RecipientListPage.RecipientListResults>().Subscribe<RecipientListPage.RecipientListResults>((Action<RecipientListPage.RecipientListResults>) (recipientListResults =>
          {
            List<string> jids = recipientListResults?.SelectedJids;
            Log.l("status privacy", "Whitelist selected: {0}", (object) (jids == null ? -1 : jids.Count));
            if (jids == null || !jids.Any<string>())
              return;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => WaStatusHelper.SetWhiteList(db, jids.ToArray(), true)));
            this.ReloadOptions();
          }));
          break;
        case WaStatusHelper.StatusPrivacySettings.BlackList:
          Dictionary<string, GroupParticipantState> blackList = (Dictionary<string, GroupParticipantState>) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => blackList = WaStatusHelper.GetBlackList((SqliteMessagesContext) db)));
          RecipientListPage.StartContactPicker(AppResources.StatusPrivacyOptionBlackList, (IEnumerable<string>) blackList.Keys, false, (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 237, (byte) 87, (byte) 63)), (System.Windows.Media.ImageSource) AssetStore.ExcludeCheckIconWhite, AppResources.NContactsPlural).ObserveOnDispatcher<RecipientListPage.RecipientListResults>().Subscribe<RecipientListPage.RecipientListResults>((Action<RecipientListPage.RecipientListResults>) (recipientListResults =>
          {
            List<string> jids = recipientListResults?.SelectedJids;
            Log.l("status privacy", "Blacklist selected: {0}", (object) (jids == null ? -1 : jids.Count));
            if (jids == null || !jids.Any<string>())
              return;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => WaStatusHelper.SetBlackList(db, jids.ToArray(), true)));
            this.ReloadOptions();
          }));
          break;
      }
    }
  }
}
