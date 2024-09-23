// Decompiled with JetBrains decompiler
// Type: WhatsApp.GroupSettingsPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class GroupSettingsPageViewModel : PageViewModelBase
  {
    private bool isLocked;
    private bool isAnnouncementOnly;
    private IDisposable jidInfoTableUpdateSub;

    public bool IsLocked
    {
      get => this.isLocked;
      set
      {
        if (this.isLocked == value)
          return;
        this.isLocked = value;
        this.NotifyPropertyChanged("RestrictStateStr");
      }
    }

    public bool IsAnnouncementOnly
    {
      get => this.isAnnouncementOnly;
      set
      {
        if (this.isAnnouncementOnly == value)
          return;
        this.isAnnouncementOnly = value;
        this.NotifyPropertyChanged("AnnouncementStateStr");
      }
    }

    public override string PageTitle => AppResources.GroupSettingsTitle;

    public string TargetNameStr { get; }

    public string Jid { get; }

    public Visibility RestrictPanelVisibility
    {
      get => !Settings.RestrictGroups ? Visibility.Collapsed : Visibility.Visible;
    }

    public Visibility AnnouncePanelVisibility
    {
      get => Settings.AnnouncementGroupSize <= 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    public string RestrictTitleStr => AppResources.EditGroupInfoTitle;

    public string RestrictStateStr
    {
      get => !this.IsLocked ? AppResources.AllParticipants : AppResources.AdminOnly;
    }

    public string AnnouncementTitleStr => AppResources.SendMessagesTitle;

    public string AnnouncementStateStr
    {
      get => !this.IsAnnouncementOnly ? AppResources.AllParticipants : AppResources.AdminOnly;
    }

    public GroupSettingsPageViewModel(
      string targetName,
      string nextJid,
      PageOrientation initialOrientation)
      : base(initialOrientation)
    {
      this.TargetNameStr = targetName;
      this.Jid = nextJid;
    }

    protected override void DisposeManagedResources()
    {
      this.jidInfoTableUpdateSub.SafeDispose();
      this.jidInfoTableUpdateSub = (IDisposable) null;
      base.DisposeManagedResources();
    }
  }
}
