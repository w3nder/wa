﻿// Decompiled with JetBrains decompiler
// Type: System.Windows.Media.Imaging.WriteableBitmapContextExtensions
// Assembly: WriteableBitmapExWinPhone, Version=1.0.9.0, Culture=neutral, PublicKeyToken=null
// MVID: 8B7E3D19-074F-4D11-AD72-780A064DB6A8
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WriteableBitmapExWinPhone.dll

#nullable disable
namespace System.Windows.Media.Imaging
{
  public static class WriteableBitmapContextExtensions
  {
    public static BitmapContext GetBitmapContext(this WriteableBitmap bmp)
    {
      return new BitmapContext(bmp);
    }

    public static BitmapContext GetBitmapContext(this WriteableBitmap bmp, ReadWriteMode mode)
    {
      return new BitmapContext(bmp, mode);
    }
  }
}
