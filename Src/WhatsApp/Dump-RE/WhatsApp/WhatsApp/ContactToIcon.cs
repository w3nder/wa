// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactToIcon
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp
{
  public class ContactToIcon : IValueConverter
  {
    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Contact contact)
      {
        Stream picture = contact.GetPicture();
        if (picture != null)
        {
          using (picture)
          {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
            bitmapImage.SetSource(picture);
            return (object) bitmapImage;
          }
        }
      }
      return (object) AssetStore.DefaultContactIcon;
    }

    public object ConvertBack(
      object value,
      System.Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
