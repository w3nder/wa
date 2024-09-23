// Decompiled with JetBrains decompiler
// Type: WhatsApp.PttMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;


namespace WhatsApp
{
  public class PttMessageViewModel : AudioMessageViewModel
  {
    private System.Windows.Media.ImageSource userPic;
    private IDisposable userPicFetchSub;

    public System.Windows.Media.ImageSource PttProfilePicSource
    {
      get
      {
        if (this.userPicFetchSub == null)
          this.userPicFetchSub = ChatPictureStore.Get(this.SenderJid, false, false, true).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (picState =>
          {
            this.userPic = (System.Windows.Media.ImageSource) (picState.Image ?? AssetStore.DefaultContactIcon);
            this.Notify("ThumbnailChanged");
          }));
        return this.userPic;
      }
    }

    public System.Windows.Media.ImageSource PttMicIconSource
    {
      get
      {
        return this.Message.KeyFromMe ? (!this.Message.IsPlayedByTarget() ? (System.Windows.Media.ImageSource) ImageStore.MicIconRemoteUnplayed : (System.Windows.Media.ImageSource) ImageStore.MicIconRemotePlayed) : (!this.Message.IsPlayedByTarget() ? (System.Windows.Media.ImageSource) ImageStore.MicIconLocalNew : (System.Windows.Media.ImageSource) ImageStore.MicIconLocalPlayed);
      }
    }

    public PttMessageViewModel(Message m)
      : base(m)
    {
    }

    public override void Cleanup()
    {
      this.userPicFetchSub.SafeDispose();
      this.userPicFetchSub = (IDisposable) null;
      base.Cleanup();
    }
  }
}
