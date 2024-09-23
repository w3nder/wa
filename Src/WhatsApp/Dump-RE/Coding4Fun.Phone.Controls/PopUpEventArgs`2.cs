// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.PopUpEventArgs`2
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class PopUpEventArgs<T, TPopUpResult> : EventArgs
  {
    public TPopUpResult PopUpResult { get; set; }

    public Exception Error { get; set; }

    public T Result { get; set; }
  }
}
