// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.IButtonBase
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public interface IButtonBase
  {
    Orientation Orientation { get; set; }

    Stretch Stretch { get; set; }

    ImageSource ImageSource { get; set; }

    double ButtonWidth { get; set; }

    double ButtonHeight { get; set; }
  }
}
