// Decompiled with JetBrains decompiler
// Type: WhatsApp.CustomTonesPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class CustomTonesPageViewModel : PageViewModelBase
  {
    private string targetNameStr;
    private bool isCustomTonesEnabled;
    private Visibility? ringtonePanelVisibility;
    private Dictionary<string, JidInfo> jidInfos;
    private IDisposable jidInfoTableUpdateSub;

    public override string PageTitle => AppResources.CustomNotificationsTitle;

    public string TargetNameStr => this.targetNameStr;

    public bool EnableCustomTones
    {
      get => this.isCustomTonesEnabled;
      set
      {
        this.isCustomTonesEnabled = value;
        string[] jids = this.jidInfos == null ? (string[]) null : this.jidInfos.Keys.ToArray<string>();
        if (jids == null || !((IEnumerable<string>) jids).Any<string>())
          return;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          this.SubscribeToJidInfoChanges(db);
          if (this.isCustomTonesEnabled)
          {
            foreach (string jid in jids)
            {
              JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.CreateToDbIfNotFound);
              if (jidInfo != null)
              {
                jidInfo.NotificationSound = (JidHelper.IsUserJid(jid) ? Settings.IndividualTone : Settings.GroupTone) ?? CustomTones.DefaultAlertPath;
                if (JidHelper.IsUserJid(jid))
                  jidInfo.RingTone = Ringtones.GetGlobalRingtonePath();
              }
            }
          }
          else
          {
            foreach (string jid in jids)
            {
              JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.None);
              if (jidInfo != null)
                jidInfo.NotificationSound = jidInfo.RingTone = (string) null;
            }
          }
          db.SubmitChanges();
        }));
      }
    }

    public string NotificationSoundTitleStr => AppResources.MessageSoundTitle;

    public string NotificationSoundStateStr
    {
      get
      {
        if (!this.isCustomTonesEnabled)
          return CustomTones.GetNotificationSoundNameForPath(Settings.IndividualTone);
        Dictionary<string, JidInfo> jidInfos = this.jidInfos;
        if (jidInfos == null || !jidInfos.Any<KeyValuePair<string, JidInfo>>())
          return (string) null;
        string[] array = jidInfos.Values.Select<JidInfo, string>((Func<JidInfo, string>) (ji => ji != null ? ji.NotificationSound ?? "" : "")).MakeUnique<string>().ToArray<string>();
        string str;
        if (((IEnumerable<string>) array).Count<string>() <= 1)
        {
          string filepath = ((IEnumerable<string>) array).FirstOrDefault<string>();
          if (string.IsNullOrEmpty(filepath))
            filepath = Settings.IndividualTone;
          str = CustomTones.GetNotificationSoundNameForPath(filepath);
        }
        else
          str = "-";
        return str ?? CustomTones.DefaultAlertName;
      }
    }

    public string RingtoneTitleStr => AppResources.RingtoneTitle;

    public string RingtoneStateStr
    {
      get
      {
        if (!this.ringtonePanelVisibility.HasValue || this.ringtonePanelVisibility.Value == Visibility.Collapsed)
          return (string) null;
        if (!this.isCustomTonesEnabled)
          return Ringtones.GetGlobalRingtoneName();
        Dictionary<string, JidInfo> jidInfos = this.jidInfos;
        if (jidInfos == null || !jidInfos.Any<KeyValuePair<string, JidInfo>>())
          return (string) null;
        string currentRingtonePath = this.GetCurrentRingtonePath();
        return currentRingtonePath != null ? Ringtones.GetRingtoneNameForPath(currentRingtonePath) : "-";
      }
    }

    public Visibility RingtonePanelVisibility
    {
      get
      {
        Dictionary<string, JidInfo> jidInfos = this.jidInfos;
        if (jidInfos == null || !jidInfos.Any<KeyValuePair<string, JidInfo>>((Func<KeyValuePair<string, JidInfo>, bool>) (p => JidHelper.IsUserJid(p.Key))))
          return Visibility.Collapsed;
        this.ringtonePanelVisibility = new Visibility?(Visibility.Visible);
        return this.ringtonePanelVisibility.Value;
      }
    }

    public double ContentOpacity => !this.isCustomTonesEnabled ? 0.5 : 1.0;

    public CustomTonesPageViewModel(
      string targetName,
      Dictionary<string, JidInfo> jiDict,
      PageOrientation initialOrientation)
      : base(initialOrientation)
    {
      this.targetNameStr = targetName;
      this.jidInfos = jiDict;
      this.isCustomTonesEnabled = jiDict != null && jiDict.Values.Any<JidInfo>((Func<JidInfo, bool>) (ji =>
      {
        if (ji == null)
          return false;
        return ji.NotificationSound != null || ji.RingTone != null;
      }));
    }

    protected override void DisposeManagedResources()
    {
      this.jidInfoTableUpdateSub.SafeDispose();
      this.jidInfoTableUpdateSub = (IDisposable) null;
      base.DisposeManagedResources();
    }

    public void SubscribeToJidInfoChanges(MessagesContext db)
    {
      if (this.jidInfoTableUpdateSub != null)
        return;
      this.jidInfoTableUpdateSub = MessagesContext.Events.JidInfoUpdateSubject.Where<DbDataUpdate>((Func<DbDataUpdate, bool>) (u => u.UpdatedObj is JidInfo updatedObj && this.jidInfos != null && this.jidInfos.ContainsKey(updatedObj.Jid))).ObserveOnDispatcher<DbDataUpdate>().Subscribe<DbDataUpdate>((Action<DbDataUpdate>) (_ => this.OnJidInfoTableUpdated()));
    }

    public string[] GetJids()
    {
      return this.jidInfos != null ? this.jidInfos.Keys.ToArray<string>() : new string[0];
    }

    public string GetCurrentRingtonePath()
    {
      Dictionary<string, JidInfo> jidInfos = this.jidInfos;
      if (jidInfos == null || !jidInfos.Any<KeyValuePair<string, JidInfo>>())
        return (string) null;
      string[] array = jidInfos.Values.Select<JidInfo, string>((Func<JidInfo, string>) (ji => ji != null ? ji.RingTone ?? "" : "")).MakeUnique<string>().ToArray<string>();
      if (((IEnumerable<string>) array).Count<string>() != 1)
        return (string) null;
      string currentRingtonePath = ((IEnumerable<string>) array).FirstOrDefault<string>();
      if (string.IsNullOrEmpty(currentRingtonePath))
        currentRingtonePath = Settings.VoipRingtone ?? "Sounds\\Ring01.wma";
      return currentRingtonePath;
    }

    public void RefreshAsync()
    {
      Dictionary<string, JidInfo> jiDict = this.jidInfos;
      if (jiDict == null || !jiDict.Any<KeyValuePair<string, JidInfo>>())
        return;
      AppState.Worker.Enqueue((Action) (() =>
      {
        if (jiDict.Any<KeyValuePair<string, JidInfo>>((Func<KeyValuePair<string, JidInfo>, bool>) (p => p.Value != null && p.Value.NotificationSound != null)))
          this.isCustomTonesEnabled = false;
        else
          this.isCustomTonesEnabled = false;
      }));
    }

    private void OnJidInfoTableUpdated()
    {
      string[] jids = this.jidInfos.Keys.ToArray<string>();
      Dictionary<string, JidInfo> jiDict = new Dictionary<string, JidInfo>();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        foreach (string str in jids)
          jiDict[str] = db.GetJidInfo(str, CreateOptions.None);
      }));
      this.jidInfos = jiDict;
      this.isCustomTonesEnabled = jiDict != null && jiDict.Values.Any<JidInfo>((Func<JidInfo, bool>) (ji =>
      {
        if (ji == null)
          return false;
        return ji.NotificationSound != null || ji.RingTone != null;
      }));
      this.NotifyPropertyChanged("EnableCustomTones");
      this.NotifyPropertyChanged("NotificationSoundStateStr");
      this.NotifyPropertyChanged("RingtoneStateStr");
      this.NotifyPropertyChanged("ContentOpacity");
    }
  }
}
