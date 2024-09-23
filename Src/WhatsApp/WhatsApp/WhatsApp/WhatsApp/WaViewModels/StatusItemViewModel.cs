// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.StatusItemViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;


namespace WhatsApp.WaViewModels
{
  public class StatusItemViewModel : JidItemViewModel
  {
    private WaStatus status;
    private string key;

    public virtual WaStatus Status
    {
      get => this.status;
      set
      {
        if (this.status == value)
          return;
        this.status = value;
        this.Refresh();
      }
    }

    public override object Model => (object) this.Status;

    public override string Key => this.key ?? (this.key = this.Status?.MessageKeyId);

    public override string Jid => this.Status?.Jid;

    public virtual bool ShowViewProgress => false;

    public override bool ShowSubtitle => !JidHelper.IsPsaJid(this.Jid);

    public StatusItemViewModel(WaStatus status)
    {
      this.status = status;
      this.EnableContextMenu = false;
    }

    public override RichTextBlock.TextSet GetSubtitle()
    {
      string str = this.Status == null ? "" : DateTimeUtils.FormatLastSeen(DateTimeUtils.FunTimeToPhoneTime(this.Status.Timestamp));
      return new RichTextBlock.TextSet() { Text = str };
    }

    protected override IObservable<System.Windows.Media.ImageSource> GetPictureSourceObservableImpl(
      string jid,
      bool fetchCurrent,
      bool trackChange)
    {
      return this.GetPictureSourceObservableImpl(fetchCurrent, trackChange);
    }

    protected override IObservable<System.Windows.Media.ImageSource> GetPictureSourceObservableImpl(
      bool getCurrent,
      bool trackChange)
    {
      return Observable.Create<System.Windows.Media.ImageSource>((Func<IObserver<System.Windows.Media.ImageSource>, Action>) (observer =>
      {
        bool disposed = false;
        MemoryStream thumbStream = (MemoryStream) null;
        int? msgId = this.Status?.MessageId;
        if (JidHelper.IsPsaJid(this.Status?.Jid))
          Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
          {
            observer.OnNext((System.Windows.Media.ImageSource) AssetStore.WhatsAppAvatar);
            observer.OnCompleted();
          }));
        else if (msgId.HasValue)
        {
          Message msg = (Message) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => msg = db.GetMessageById(msgId.Value)));
          if (msg != null)
          {
            if (msg.IsTextStatus())
              observer.OnCompleted();
            bool isLargeSize = false;
            thumbStream = msg.GetThumbnailStream(true, out isLargeSize);
            if (thumbStream == null)
              observer.OnCompleted();
            else
              Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
              {
                if (disposed)
                  return;
                System.Windows.Media.ImageSource imageSource = (System.Windows.Media.ImageSource) null;
                using (thumbStream)
                  imageSource = (System.Windows.Media.ImageSource) BitmapUtils.CreateBitmap((Stream) thumbStream, 800, 800);
                thumbStream = (MemoryStream) null;
                observer.OnNext(imageSource);
                observer.OnCompleted();
              }));
          }
        }
        else
          observer.OnCompleted();
        return (Action) (() =>
        {
          disposed = true;
          thumbStream.SafeDispose();
          thumbStream = (MemoryStream) null;
        });
      }));
    }

    public override Brush PictureBackgroundBrush
    {
      get
      {
        int? msgId = this.Status?.MessageId;
        if (msgId.HasValue)
        {
          Message msg = (Message) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => msg = db.GetMessageById(msgId.Value)));
          uint? backgroundArgb = (uint?) msg?.InternalProperties?.ExtendedTextPropertiesField?.BackgroundArgb;
          if (backgroundArgb.HasValue)
            return (Brush) new SolidColorBrush(Color.FromArgb((byte) (backgroundArgb.Value >> 24 & (uint) byte.MaxValue), (byte) (backgroundArgb.Value >> 16 & (uint) byte.MaxValue), (byte) (backgroundArgb.Value >> 8 & (uint) byte.MaxValue), (byte) (backgroundArgb.Value & (uint) byte.MaxValue)));
        }
        return base.PictureBackgroundBrush;
      }
    }

    public override string GetTitle()
    {
      string jid = this.Status?.Jid;
      if (JidHelper.IsPsaJid(jid))
        return Constants.OffcialName;
      if (!JidHelper.IsSelfJid(jid))
        return "";
      Message msg = (Message) null;
      bool isPending = false;
      int n = 0;
      int? msgId = this.Status?.MessageId;
      if (msgId.HasValue)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          msg = db.GetMessageById(msgId.Value);
          if (msg == null)
            return;
          isPending = !msg.IsDeliveredToServer();
          if (isPending)
            return;
          isPending = false;
          n = db.GetReceiptCountForMessage(msgId.Value, new FunXMPP.FMessage.Status[2]
          {
            FunXMPP.FMessage.Status.ReadByTarget,
            FunXMPP.FMessage.Status.PlayedByTarget
          });
        }));
      return !isPending ? Plurals.Instance.GetString(AppResources.StatusReadRecipientCountPlural, n) : AppResources.StatusPending;
    }
  }
}
