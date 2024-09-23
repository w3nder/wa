// Decompiled with JetBrains decompiler
// Type: WhatsApp.LoginInvalid
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;


namespace WhatsApp
{
  public class LoginInvalid : PhoneApplicationPage
  {
    private bool _contentLoaded;

    public LoginInvalid() => this.InitializeComponent();

    private void VerifyButton_Click(object sender, RoutedEventArgs e)
    {
      string str;
      Settings.PushName = str = "";
      Settings.CountryCode = str;
      Settings.PhoneNumber = str;
      Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.NewlyEntered;
      Settings.DeleteMany((IEnumerable<Settings.Key>) new Settings.Key[13]
      {
        Settings.Key.LoginFailed,
        Settings.Key.LoginFailedReason,
        Settings.Key.LastPrivacyCheckUtc,
        Settings.Key.LastBlockListCheckUtc,
        Settings.Key.LastPropertiesQueryUtc,
        Settings.Key.LastGroupsUpdatedUtc,
        Settings.Key.QrBlob,
        Settings.Key.LastFullSyncUtc,
        Settings.Key.NextFullSyncUtc,
        Settings.Key.SyncBackoff,
        Settings.Key.SyncHistory,
        Settings.Key.RemovedJids,
        Settings.Key.BackupKey
      });
      AppState.GetConnection().Encryption?.Reset();
      using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
        oneDriveManifest.Delete();
      Backup.RemoveSavedSummary();
      Settings.SuppressRestoreFromBackupAtReg = true;
      NavUtils.NavigateToPage(this.NavigationService, "PhoneNumberEntry", "ClearStack=true");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/LoginInvalid.xaml", UriKind.Relative));
    }
  }
}
