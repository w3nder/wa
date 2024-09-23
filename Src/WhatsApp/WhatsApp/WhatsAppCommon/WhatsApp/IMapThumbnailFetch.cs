// Decompiled with JetBrains decompiler
// Type: WhatsApp.IMapThumbnailFetch
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public interface IMapThumbnailFetch
  {
    IObservable<WriteableBitmap> GetMapThumbnail(
      double latitude,
      double longitude,
      int sourceSize,
      int cropSize);

    IObservable<WriteableBitmap> GetMapThumbnail(
      double latitude,
      double longitude,
      int sourceWidth,
      int sourceHeight,
      int cropWidth,
      int cropHeight);
  }
}
