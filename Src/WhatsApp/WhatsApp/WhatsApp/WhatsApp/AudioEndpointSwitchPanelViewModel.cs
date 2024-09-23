// Decompiled with JetBrains decompiler
// Type: WhatsApp.AudioEndpointSwitchPanelViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class AudioEndpointSwitchPanelViewModel : PropChangedBase
  {
    private ICommand switchAudioOutputCommand_;
    private AudioEndpointSwitchPanel panel_;

    public ICommand SwitchAudioOutputCommand
    {
      get
      {
        return this.switchAudioOutputCommand_ ?? (this.switchAudioOutputCommand_ = (ICommand) new UIUtils.DelegateCommand(new Action<object>(this.ToggleAudioOutputSetting)));
      }
    }

    public string SwitchButtonText
    {
      get
      {
        switch ((WaAudioRouting.Endpoint) Settings.DefaultAudioEndpoint)
        {
          case WaAudioRouting.Endpoint.Speaker:
            return AppResources.Speaker.ToLower();
          case WaAudioRouting.Endpoint.Earpiece:
            return AppResources.Handset.ToLower();
          default:
            return "";
        }
      }
    }

    public BitmapImage SwitchButtonIcon
    {
      get
      {
        switch ((WaAudioRouting.Endpoint) Settings.DefaultAudioEndpoint)
        {
          case WaAudioRouting.Endpoint.Speaker:
            return AssetStore.LoadDarkThemeAsset("speaker.png");
          case WaAudioRouting.Endpoint.Earpiece:
            return AssetStore.LoadDarkThemeAsset("handset.png");
          default:
            return (BitmapImage) null;
        }
      }
    }

    public AudioEndpointSwitchPanelViewModel(AudioEndpointSwitchPanel panel)
    {
      this.panel_ = panel;
      this.panel_.RoundButton.ButtonIcon = (BitmapSource) this.SwitchButtonIcon;
    }

    public event EventHandler AudioEndpointChanged;

    protected void NotifyAudioEndpointChanged()
    {
      if (this.AudioEndpointChanged == null)
        return;
      this.AudioEndpointChanged((object) this, new EventArgs());
    }

    public void ToggleAudioOutputSetting(object dummy = null)
    {
      WaAudioRouting.Endpoint defaultAudioEndpoint = (WaAudioRouting.Endpoint) Settings.DefaultAudioEndpoint;
      WaAudioRouting.Endpoint endpoint = WaAudioRouting.Endpoint.Speaker;
      switch (defaultAudioEndpoint)
      {
        case WaAudioRouting.Endpoint.Speaker:
          endpoint = WaAudioRouting.Endpoint.Earpiece;
          break;
        case WaAudioRouting.Endpoint.Earpiece:
          endpoint = WaAudioRouting.Endpoint.Speaker;
          break;
      }
      Settings.DefaultAudioEndpoint = (int) endpoint;
      this.NotifyPropertyChanged("SwitchButtonText");
      this.panel_.RoundButton.ButtonIcon = (BitmapSource) this.SwitchButtonIcon;
      this.NotifyAudioEndpointChanged();
    }
  }
}
