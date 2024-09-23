// Decompiled with JetBrains decompiler
// Type: WhatsApp.PhoneContactItemViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.IO;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class PhoneContactItemViewModel : RecipientItemViewModel
  {
    public Contact PhoneContact { get; private set; }

    public PhoneContactItemViewModel(Contact user)
      : base((UserStatus) null)
    {
      this.PhoneContact = user;
      this.itemType = RecipientListPage.RecipientItemType.Contact;
    }

    public override string Jid => this.PhoneContact.GetIdentifier();

    public override bool ShowSubtitleRow => false;

    public override string GetTitle() => this.PhoneContact.DisplayName;

    protected override IObservable<System.Windows.Media.ImageSource> GetPictureSourceObservableImpl(
      bool getCurrent,
      bool trackChange)
    {
      System.Windows.Media.ImageSource imageSource = (System.Windows.Media.ImageSource) null;
      Stream picture = this.PhoneContact.GetPicture();
      if (picture != null)
      {
        using (picture)
        {
          BitmapImage bitmapImage = new BitmapImage();
          bitmapImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
          bitmapImage.SetSource(picture);
          imageSource = (System.Windows.Media.ImageSource) bitmapImage;
        }
      }
      else
        imageSource = (System.Windows.Media.ImageSource) AssetStore.DefaultContactIcon;
      return Observable.Return<System.Windows.Media.ImageSource>(imageSource);
    }
  }
}
