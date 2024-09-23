// Decompiled with JetBrains decompiler
// Type: WhatsApp.PhoneContactJidItemViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.IO;
using System.Windows.Media.Imaging;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class PhoneContactJidItemViewModel : JidItemViewModel
  {
    private Contact Contact;

    public PhoneContactJidItemViewModel(Contact c) => this.Contact = c;

    public override string GetTitle() => this.Contact.DisplayName;

    public override string Jid => this.Contact.GetIdentifier();

    public override System.Windows.Media.ImageSource GetDefaultPicture()
    {
      System.Windows.Media.ImageSource defaultPicture = (System.Windows.Media.ImageSource) null;
      Stream picture = this.Contact.GetPicture();
      if (picture != null)
      {
        using (picture)
        {
          BitmapImage bitmapImage = new BitmapImage();
          bitmapImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
          bitmapImage.SetSource(picture);
          defaultPicture = (System.Windows.Media.ImageSource) bitmapImage;
        }
      }
      else
        defaultPicture = (System.Windows.Media.ImageSource) AssetStore.DefaultContactIcon;
      return defaultPicture;
    }
  }
}
