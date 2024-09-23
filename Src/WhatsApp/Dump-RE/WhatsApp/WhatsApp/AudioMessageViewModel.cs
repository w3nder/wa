// Decompiled with JetBrains decompiler
// Type: WhatsApp.AudioMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Media.Imaging;
using WhatsApp.CommonOps;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class AudioMessageViewModel : FileMessageViewModelBase
  {
    private bool isPtt;

    public AudioMessageViewModel(Message m)
      : base(m)
    {
      this.isPtt = m.IsPtt();
    }

    public override void Cleanup()
    {
      base.Cleanup();
      if (this.Message == null)
        return;
      ViewMessage.StopOnDispose(this.Message, FunXMPP.FMessage.Type.Audio);
    }

    public override Thickness ViewPanelMargin => new Thickness(6.0 * this.zoomMultiplier);

    public override Thickness FooterMargin
    {
      get
      {
        if (!this.isPtt)
          return new Thickness(0.0, 0.0, 9.0 * this.zoomMultiplier, 9.0 * this.zoomMultiplier);
        return !this.ShouldShowOnOutgoingSide ? new Thickness(0.0, 0.0, 18.0 * this.zoomMultiplier + this.ThumbnailWidth, 6.0 * this.zoomMultiplier) : base.FooterMargin;
      }
    }

    public Thickness ActionButtonMargin
    {
      get
      {
        if (!this.isPtt)
          return new Thickness(16.0 * this.zoomMultiplier, 0.0, 0.0, 0.0);
        if (this.Message.KeyFromMe)
          return new Thickness(16.0 * this.zoomMultiplier, 0.0, 0.0, 0.0);
        return this.isGroupMsg && this.GroupedPosition != MessageViewModel.GroupingPosition.Middle && this.GroupedPosition != MessageViewModel.GroupingPosition.Bottom ? new Thickness(10.0 * this.zoomMultiplier, 16.0 * this.zoomMultiplier, 0.0, 0.0) : new Thickness(10.0 * this.zoomMultiplier, 0.0, 0.0, 0.0);
      }
    }

    private bool ShouldShiftPlaybackControlsDown
    {
      get
      {
        return this.isPtt && this.IsIncomingGroupMessage && this.QuotedMessage == null && this.GroupedPosition != MessageViewModel.GroupingPosition.Bottom && this.GroupedPosition != MessageViewModel.GroupingPosition.Middle;
      }
    }

    public override bool ShouldShowMediaInfo
    {
      get => !this.isPtt && this.Message.LocalFileUri == null && !this.Message.TransferInProgress;
    }

    public Thickness PlaybackBarMargin
    {
      get
      {
        return new Thickness(18.0 * this.zoomMultiplier, (double) (20 + (this.ShouldShiftPlaybackControlsDown ? 15 : 0)) * this.zoomMultiplier, 12.0 * this.zoomMultiplier, 0.0);
      }
    }

    public Thickness TransferIndicatorMargin
    {
      get
      {
        return new Thickness(18.0 * this.zoomMultiplier, (this.ShouldShiftPlaybackControlsDown ? 15.0 : 0.0) * this.zoomMultiplier, 12.0 * this.zoomMultiplier, 0.0);
      }
    }

    public Thickness PttDurationMargin
    {
      get
      {
        return new Thickness(18.0 * this.zoomMultiplier, (double) (30 + (this.ShouldShiftPlaybackControlsDown ? 16 : 0)) * this.zoomMultiplier, 0.0, 0.0);
      }
    }

    public double PttPlaybackValue
    {
      get
      {
        return this.Message.MediaDurationSeconds > 0 ? Math.Max(0.0, Math.Min(1.0, this.Message.PlaybackValue / ((double) this.Message.MediaDurationSeconds * 1000.0))) : 0.0;
      }
    }

    public string PttDurationStr
    {
      get
      {
        if (this.Message.MediaDurationSeconds <= 0)
          return (string) null;
        if (!this.Message.PlaybackInProgress)
          return DateTimeUtils.FormatDuration(this.Message.MediaDurationSeconds);
        int seconds = (int) Math.Round(this.Message.PlaybackValue / 1000.0);
        if (seconds > this.Message.MediaDurationSeconds)
          seconds = this.Message.MediaDurationSeconds;
        return DateTimeUtils.FormatDuration(seconds);
      }
    }

    public override Thickness MediaInfoMargin
    {
      get
      {
        return new Thickness(18.0 * this.zoomMultiplier, (this.ShouldShiftPlaybackControlsDown ? 15.0 : 0.0) * this.zoomMultiplier, 0.0, 0.0);
      }
    }

    public bool ShouldShowPlaybackBar
    {
      get => !this.Message.TransferInProgress && this.Message.LocalFileUri != null;
    }

    public Visibility PlaybackBarVisibility => this.ShouldShowPlaybackBar.ToVisibility();

    public bool ShouldShowThumbnailOnRight => this.isPtt && !this.ShouldShowOnOutgoingSide;

    public override string MediaInfoStr
    {
      get
      {
        if (!this.isPtt || this.Message.TransferInProgress)
          return base.MediaInfoStr;
        string mediaInfoStr = (string) null;
        if (this.Message.KeyFromMe)
        {
          switch (MessageExtensions.GetUploadActionState(this.Message))
          {
            case MessageExtensions.MediaUploadActionState.Retryable:
              mediaInfoStr = AppResources.Retry;
              break;
            case MessageExtensions.MediaUploadActionState.Cancellable:
              mediaInfoStr = AppResources.Cancel;
              break;
          }
        }
        else if (this.Message.LocalFileUri == null)
          mediaInfoStr = Utils.FileSizeFormatter.Format(this.Message.MediaSize);
        return mediaInfoStr;
      }
    }

    public override BitmapSource ActionButtonIcon
    {
      get
      {
        if (!this.ShouldShowActionButton)
          return (BitmapSource) null;
        BitmapSource actionButtonIcon = (BitmapSource) null;
        if (this.isPtt)
        {
          if (this.Message.KeyFromMe)
          {
            switch (MessageExtensions.GetUploadActionState(this.Message))
            {
              case MessageExtensions.MediaUploadActionState.Retryable:
                actionButtonIcon = (BitmapSource) ImageStore.UploadIcon;
                break;
              case MessageExtensions.MediaUploadActionState.Cancellable:
                actionButtonIcon = AssetStore.DismissIconWhite;
                break;
            }
          }
          else if (this.Message.LocalFileUri == null)
            actionButtonIcon = (BitmapSource) ImageStore.DownloadIcon;
          if (actionButtonIcon == null)
            actionButtonIcon = this.Message.PlaybackInProgress ? (BitmapSource) ImageStore.PauseIcon : (BitmapSource) ImageStore.PlayIcon;
        }
        else
          actionButtonIcon = this.Message.LocalFileUri == null ? (BitmapSource) ImageStore.DownloadIcon : (this.Message.PlaybackInProgress ? (BitmapSource) ImageStore.PauseIcon : (BitmapSource) ImageStore.PlayIcon);
        return actionButtonIcon;
      }
    }

    public override bool ShouldUseSeparateHeaderLine
    {
      get => this.IsIncomingGroupMessage && this.ShouldShowPushName;
    }

    public override Set<string> GetTrackedProperties()
    {
      Set<string> trackedProperties = base.GetTrackedProperties();
      if (this.isPtt && !this.Message.IsPlayedByTarget())
        trackedProperties.Add("Status");
      trackedProperties.Add("PlaybackValue");
      trackedProperties.Add("PlaybackInProgress");
      return trackedProperties;
    }

    protected override Size GetTargetThumbnailSizeImpl()
    {
      double num = (!this.isPtt || !this.IsIncomingGroupMessage ? 80.0 : 96.0) * this.zoomMultiplier;
      return new Size(num, num);
    }

    protected override bool OnMessagePropertyChanged(string prop)
    {
      if (base.OnMessagePropertyChanged(prop))
        return true;
      bool flag = false;
      switch (prop)
      {
        case "PlaybackValue":
          this.OnMessagePlaybackValueChanged();
          flag = true;
          break;
        case "PlaybackInProgress":
          this.OnMessagePlaybackStatusChanged();
          flag = true;
          break;
      }
      return flag;
    }

    private void OnMessagePlaybackValueChanged() => this.Notify("PlaybackProgressChanged");

    private void OnMessagePlaybackStatusChanged() => this.Notify("PlaybackStateChanged");
  }
}
