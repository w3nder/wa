// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.Converters.ThemedImageConverterHelper
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

#nullable disable
namespace Coding4Fun.Phone.Controls.Converters
{
  public static class ThemedImageConverterHelper
  {
    private static readonly Dictionary<string, BitmapImage> ImageCache = new Dictionary<string, BitmapImage>();

    public static BitmapImage GetImage(string path, bool negateResult = false)
    {
      if (string.IsNullOrEmpty(path))
        return (BitmapImage) null;
      bool flag = Application.Current.Resources.Contains((object) "PhoneDarkThemeVisibility") && (Visibility) Application.Current.Resources[(object) "PhoneDarkThemeVisibility"] == 0;
      if (negateResult)
        flag = !flag;
      path = string.Format(path, flag ? (object) "dark" : (object) "light");
      BitmapImage image;
      if (!ThemedImageConverterHelper.ImageCache.TryGetValue(path, out image))
      {
        image = new BitmapImage(new Uri(path, UriKind.Relative));
        ThemedImageConverterHelper.ImageCache.Add(path, image);
      }
      return image;
    }
  }
}
