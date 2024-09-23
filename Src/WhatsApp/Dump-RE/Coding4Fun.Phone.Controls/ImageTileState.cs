// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.ImageTileState
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System.Windows.Media.Animation;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public struct ImageTileState
  {
    public Storyboard Storyboard { get; set; }

    public int Row { get; set; }

    public int Column { get; set; }

    public bool ForceLargeImageCleanup { get; set; }
  }
}
