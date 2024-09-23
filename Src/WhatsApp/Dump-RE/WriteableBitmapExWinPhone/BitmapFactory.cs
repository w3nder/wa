// Decompiled with JetBrains decompiler
// Type: System.Windows.Media.Imaging.BitmapFactory
// Assembly: WriteableBitmapExWinPhone, Version=1.0.9.0, Culture=neutral, PublicKeyToken=null
// MVID: 8B7E3D19-074F-4D11-AD72-780A064DB6A8
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WriteableBitmapExWinPhone.dll

#nullable disable
namespace System.Windows.Media.Imaging
{
  public static class BitmapFactory
  {
    public static WriteableBitmap New(int pixelWidth, int pixelHeight)
    {
      return new WriteableBitmap(pixelWidth, pixelHeight);
    }
  }
}
