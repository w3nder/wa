// Decompiled with JetBrains decompiler
// Type: WhatsApp.LiveLocationMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class LiveLocationMessageViewModel : LocationMessageViewModel
  {
    public LiveLocationMessageViewModel(Message m)
      : base(m)
    {
      this.ExcludedMenuItems = new Set<MessageMenu.MessageMenuItem>((IEnumerable<MessageMenu.MessageMenuItem>) new MessageMenu.MessageMenuItem[1]
      {
        MessageMenu.MessageMenuItem.Forward
      });
    }

    protected override bool ShouldAddFooterPlaceHolder
    {
      get
      {
        return this.MergedPosition == MessageViewModel.GroupingPosition.None || this.MergedPosition == MessageViewModel.GroupingPosition.Bottom;
      }
    }

    protected override bool ShouldShowFooterInAccentColor => false;

    public override Thickness FooterMargin
    {
      get
      {
        return this.IsCurrentlyLive() ? new Thickness(0.0, 0.0, 15.0 * this.zoomMultiplier, 48.0 * this.zoomMultiplier + 15.0 * this.zoomMultiplier) : base.FooterMargin;
      }
    }

    public override bool ShouldUseFooterProtection => false;

    public string GetCaption()
    {
      string caption = this.Message.InternalProperties?.LiveLocationPropertiesField?.Caption;
      return !this.ShouldAddFooterPlaceHolder || string.IsNullOrEmpty(caption) ? caption : string.Format("{0}{1}", (object) caption, (object) this.FooterSpaceHolder);
    }

    public bool IsCurrentlyLive()
    {
      DateTime? funTimestamp = this.Message.FunTimestamp;
      if (funTimestamp.HasValue)
        return (FunRunner.CurrentServerTimeUtc - funTimestamp.Value).TotalSeconds < (double) this.Message.MediaDurationSeconds;
      Log.SendCrashLog((Exception) new InvalidDataException("Message.FunTimestamp is null"), "Message.FunTimestamp is null", logOnlyForRelease: true);
      return false;
    }

    public DateTime GetLocationExpirationTime()
    {
      return this.Message.LocalTimestamp.Value.Add(TimeSpan.FromSeconds((double) this.Message.MediaDurationSeconds));
    }

    public override Set<string> GetTrackedProperties()
    {
      Set<string> trackedProperties = base.GetTrackedProperties();
      if (this.IsCurrentlyLive())
        trackedProperties.Add("MediaDurationSeconds");
      return trackedProperties;
    }

    protected override bool OnMessagePropertyChanged(string prop)
    {
      if (base.OnMessagePropertyChanged(prop))
        return true;
      bool flag = false;
      if (prop == "MediaDurationSeconds")
      {
        this.Notify("Refresh");
        flag = true;
      }
      return flag;
    }
  }
}
