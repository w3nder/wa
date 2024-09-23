// Decompiled with JetBrains decompiler
// Type: WhatsApp.CustomTonesPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class CustomTonesPage : PhoneApplicationPage
  {
    private static Dictionary<string, JidInfo> nextInstanceJidInfos;
    private static string nextInstanceTargetName;
    private CustomTonesPageViewModel viewModel;
    private bool _contentLoaded;

    public CustomTonesPage()
    {
      this.InitializeComponent();
      Dictionary<string, JidInfo> instanceJidInfos = CustomTonesPage.nextInstanceJidInfos;
      CustomTonesPage.nextInstanceJidInfos = (Dictionary<string, JidInfo>) null;
      if (instanceJidInfos == null)
        return;
      this.DataContext = (object) (this.viewModel = new CustomTonesPageViewModel(CustomTonesPage.nextInstanceTargetName, instanceJidInfos, this.Orientation));
    }

    public static void Start(string targetName, Dictionary<string, JidInfo> jidInfos)
    {
      if (jidInfos == null || !jidInfos.Any<KeyValuePair<string, JidInfo>>())
        return;
      CustomTonesPage.nextInstanceTargetName = targetName;
      CustomTonesPage.nextInstanceJidInfos = jidInfos;
      NavUtils.NavigateToPage(nameof (CustomTonesPage));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.viewModel != null)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
    }

    private void NotificationSoundPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.viewModel == null)
        return;
      string[] jids = this.viewModel.GetJids();
      if (!((IEnumerable<string>) jids).Any<string>())
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.viewModel.SubscribeToJidInfoChanges(db)));
      SelectAlertPage.StartForJids(jids);
    }

    private void RingtonePanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.viewModel == null)
        return;
      string[] jids = this.viewModel.GetJids();
      if (!((IEnumerable<string>) jids).Any<string>())
        return;
      string currentRingtonePath = this.viewModel.GetCurrentRingtonePath();
      TonePickerPage.Start(Ringtones.LoadRingtones(), currentRingtonePath).ObserveOnDispatcher<Ringtones.Tone>().Subscribe<Ringtones.Tone>((Action<Ringtones.Tone>) (tone =>
      {
        if (tone == null)
          tone = Ringtones.GetGlobalRingtone();
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          this.viewModel.SubscribeToJidInfoChanges(db);
          foreach (string jid in jids)
            db.GetJidInfo(jid, CreateOptions.CreateToDbIfNotFound).RingTone = tone.Filepath;
          db.SubmitChanges();
        }));
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/CustomTonesPage.xaml", UriKind.Relative));
    }
  }
}
