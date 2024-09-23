// Decompiled with JetBrains decompiler
// Type: WhatsApp.PrivacySettingItem
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class PrivacySettingItem : WaViewModelBase
  {
    private string item;
    private PrivacyVisibility visibility;
    private Action<string, PrivacyVisibility> onVisibilityChanged;

    public string Title
    {
      get
      {
        string str = (string) null;
        if (this.item == "last")
          str = AppResources.PrivacySettingsLastSeen;
        else if (this.item == "profile")
          str = AppResources.PrivacySettingsProfilePhoto;
        else if (this.item == "status")
          str = AppResources.RevivedStatusV2Title;
        return str ?? "";
      }
    }

    public int Selection
    {
      get
      {
        int selection = 0;
        switch (this.visibility)
        {
          case PrivacyVisibility.None:
            selection = 2;
            break;
          case PrivacyVisibility.Contacts:
            selection = 1;
            break;
        }
        return selection;
      }
      set
      {
        PrivacyVisibility privacyVisibility;
        switch (value)
        {
          case 1:
            privacyVisibility = PrivacyVisibility.Contacts;
            break;
          case 2:
            privacyVisibility = PrivacyVisibility.None;
            break;
          default:
            privacyVisibility = PrivacyVisibility.Everyone;
            break;
        }
        if (privacyVisibility == this.visibility)
          return;
        this.onVisibilityChanged(this.item, this.visibility = privacyVisibility);
      }
    }

    public string NoticeStr { get; set; }

    public Visibility NoticeVisibility => (!string.IsNullOrEmpty(this.NoticeStr)).ToVisibility();

    public PrivacySettingItem(
      string privacyItem,
      PrivacyVisibility privacyVisibility,
      Action<string, PrivacyVisibility> onPrivacyVisibilityChanged)
    {
      this.item = privacyItem;
      this.visibility = privacyVisibility;
      this.onVisibilityChanged = onPrivacyVisibilityChanged;
    }
  }
}
