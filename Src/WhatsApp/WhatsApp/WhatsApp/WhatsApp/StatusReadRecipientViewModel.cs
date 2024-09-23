// Decompiled with JetBrains decompiler
// Type: WhatsApp.StatusReadRecipientViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class StatusReadRecipientViewModel : WaViewModelBase
  {
    private System.Windows.Media.ImageSource picSrc;
    private IDisposable picSub;
    private ReceiptState receipt;

    public System.Windows.Media.ImageSource Picture
    {
      get
      {
        System.Windows.Media.ImageSource picture = this.picSrc;
        if (picture == null)
        {
          if (this.picSub == null)
            this.picSub = ChatPictureStore.Get(this.Jid, false, false, true, ChatPictureStore.SubMode.GetCurrent).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (picState =>
            {
              this.picSrc = (System.Windows.Media.ImageSource) picState.Image;
              this.NotifyPropertyChanged(nameof (Picture));
            }));
          else
            picture = (System.Windows.Media.ImageSource) AssetStore.DefaultContactIcon;
        }
        return picture;
      }
    }

    public string NameStr => JidHelper.GetDisplayNameForContactJid(this.Jid);

    public string DateStr => DateTimeUtils.FormatCompactDate(this.receipt.LocalTimestamp, true);

    public string TimeStr => DateTimeUtils.FormatCompactTime(this.receipt.LocalTimestamp);

    public string Jid => this.receipt.Jid;

    public DateTime Timestamp => this.receipt.Timestamp;

    public StatusReadRecipientViewModel(ReceiptState rs) => this.receipt = rs;
  }
}
