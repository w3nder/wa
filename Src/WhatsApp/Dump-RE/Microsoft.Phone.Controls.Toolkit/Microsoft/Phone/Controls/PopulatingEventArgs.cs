// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.PopulatingEventArgs
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class PopulatingEventArgs : RoutedEventArgs
  {
    public string Parameter { get; private set; }

    public bool Cancel { get; set; }

    public PopulatingEventArgs(string parameter) => this.Parameter = parameter;
  }
}
