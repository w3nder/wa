// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp
{
  public class ContactMessageViewModel : MessageViewModel
  {
    private object vCardPhotoLock = new object();
    private WeakReference<byte[]> vCardPhotoBytesRef;
    private IDisposable waPicSub;
    private bool? shouldShowWhatsAppableContactActions;
    private bool? isBulkContactCard;

    public static int ContactThumbnailSize => (int) (100.0 * ResolutionHelper.ZoomMultiplier + 0.5);

    private byte[] CachedVCardPhotoBytes
    {
      get
      {
        lock (this.vCardPhotoLock)
        {
          byte[] target = (byte[]) null;
          if (this.vCardPhotoBytesRef != null && this.vCardPhotoBytesRef.TryGetTarget(out target) && target != null)
            return target;
          string base64PhotoData;
          if ((base64PhotoData = ContactVCardParser.GetBase64PhotoData(this.Message.Data)) == null)
          {
            Log.l("msg vm", "parse vcard failed");
          }
          else
          {
            try
            {
              target = Convert.FromBase64String(base64PhotoData);
            }
            catch (Exception ex)
            {
              target = (byte[]) null;
              Log.LogException(ex, "parse vcard photo failed");
            }
            if (target != null && ((IEnumerable<byte>) target).Any<byte>())
            {
              if (this.vCardPhotoBytesRef == null)
                this.vCardPhotoBytesRef = new WeakReference<byte[]>(target);
              else
                this.vCardPhotoBytesRef.SetTarget(target);
            }
          }
          return target;
        }
      }
    }

    public override Thickness FooterMargin
    {
      get => new Thickness(0.0, 0.0, 15.0 * this.zoomMultiplier, this.zoomMultiplier * 69.0);
    }

    public bool ShouldShowWhatsAppableContactActions
    {
      get
      {
        if (this.shouldShowWhatsAppableContactActions.HasValue)
          return this.shouldShowWhatsAppableContactActions.Value;
        this.shouldShowWhatsAppableContactActions = new bool?(false);
        ContactVCard contactVcard = ContactVCard.Create(this.Message.Data);
        if (contactVcard != null && ((IEnumerable<ContactVCard.PhoneNumber>) contactVcard.PhoneNumbers).Any<ContactVCard.PhoneNumber>((Func<ContactVCard.PhoneNumber, bool>) (pn => JidHelper.IsUserJid(pn.Jid))))
        {
          bool isSuspicious = true;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => isSuspicious = SuspiciousJid.IsMessageSuspicious(db, this.Message)));
          if (!isSuspicious)
            this.shouldShowWhatsAppableContactActions = new bool?(true);
        }
        return this.shouldShowWhatsAppableContactActions.Value;
      }
    }

    public bool IsBulkContactCard
    {
      get
      {
        if (this.isBulkContactCard.HasValue)
          return this.isBulkContactCard.Value;
        this.isBulkContactCard = new bool?(this.Message.HasMultipleContacts());
        return this.isBulkContactCard.Value;
      }
    }

    public ContactMessageViewModel(Message m)
      : base(m)
    {
    }

    public override void Cleanup()
    {
      base.Cleanup();
      this.waPicSub.SafeDispose();
      this.waPicSub = (IDisposable) null;
    }

    protected override string GetTextStr() => this.Message.MediaName;

    public override bool IsThumbnailAvailable()
    {
      byte[] cachedVcardPhotoBytes = this.CachedVCardPhotoBytes;
      return cachedVcardPhotoBytes != null && ((IEnumerable<byte>) cachedVcardPhotoBytes).Any<byte>();
    }

    protected override Size GetTargetThumbnailSizeImpl()
    {
      return new Size((double) ContactMessageViewModel.ContactThumbnailSize, (double) ContactMessageViewModel.ContactThumbnailSize);
    }

    protected override IObservable<MessageViewModel.ThumbnailState> GetThumbnailObservableImpl(
      MessageViewModel.ThumbnailOptions thumbOptions = MessageViewModel.ThumbnailOptions.Standard)
    {
      Message msg = this.Message;
      return Observable.Create<MessageViewModel.ThumbnailState>((Func<IObserver<MessageViewModel.ThumbnailState>, Action>) (observer =>
      {
        MemoryStream vCardPhotoStream = (MemoryStream) null;
        byte[] cachedVcardPhotoBytes = this.CachedVCardPhotoBytes;
        if (cachedVcardPhotoBytes != null)
        {
          if (((IEnumerable<byte>) cachedVcardPhotoBytes).Any<byte>())
          {
            try
            {
              vCardPhotoStream = new MemoryStream(cachedVcardPhotoBytes);
            }
            catch (Exception ex)
            {
              vCardPhotoStream = (MemoryStream) null;
            }
          }
        }
        if (vCardPhotoStream != null)
          MessageViewModel.InvokeAsync((Action) (() =>
          {
            System.Windows.Media.ImageSource thumb = (System.Windows.Media.ImageSource) null;
            using (vCardPhotoStream)
            {
              BitmapImage bitmapImage = new BitmapImage();
              bitmapImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
              bitmapImage.SetSource((Stream) vCardPhotoStream);
              thumb = (System.Windows.Media.ImageSource) bitmapImage;
            }
            observer.OnNext(new MessageViewModel.ThumbnailState(thumb, msg.KeyId, false));
          }));
        ContactVCard contactVcard = ContactVCard.Create(msg.Data);
        string jid = contactVcard == null ? (string) null : ((IEnumerable<ContactVCard.PhoneNumber>) contactVcard.PhoneNumbers).Where<ContactVCard.PhoneNumber>((Func<ContactVCard.PhoneNumber, bool>) (pn => pn.Jid != null && JidHelper.IsUserJid(pn.Jid))).Select<ContactVCard.PhoneNumber, string>((Func<ContactVCard.PhoneNumber, string>) (pn => pn.Jid)).FirstOrDefault<string>();
        if (contactVcard == null || string.IsNullOrEmpty(jid))
        {
          observer.OnNext(new MessageViewModel.ThumbnailState((System.Windows.Media.ImageSource) null, msg.KeyId, false));
          return (Action) (() => { });
        }
        IDisposable chatPicSub = (IDisposable) null;
        chatPicSub = ChatPictureStore.Get(jid, false, false, false, ChatPictureStore.SubMode.GetCurrent).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (picState =>
        {
          observer.OnNext(new MessageViewModel.ThumbnailState((System.Windows.Media.ImageSource) picState.Image, msg.KeyId, false));
          if (picState.Image == null)
            return;
          observer.OnCompleted();
        }));
        return (Action) (() =>
        {
          chatPicSub.SafeDispose();
          chatPicSub = (IDisposable) null;
        });
      }));
    }
  }
}
