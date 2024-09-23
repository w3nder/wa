// Decompiled with JetBrains decompiler
// Type: WhatsApp.VisualRequestEventArgs
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;

#nullable disable
namespace WhatsApp
{
  public class VisualRequestEventArgs : EventArgs
  {
    public VisualRequestEventArgs(int index) => this.Index = index;

    public int Index { get; private set; }

    public FrameworkElement Visual { get; set; }
  }
}
