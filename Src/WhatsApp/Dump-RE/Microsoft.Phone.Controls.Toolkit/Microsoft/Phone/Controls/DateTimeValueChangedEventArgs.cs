// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.DateTimeValueChangedEventArgs
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class DateTimeValueChangedEventArgs : EventArgs
  {
    public DateTimeValueChangedEventArgs(DateTime? oldDateTime, DateTime? newDateTime)
    {
      this.OldDateTime = oldDateTime;
      this.NewDateTime = newDateTime;
    }

    public DateTime? OldDateTime { get; private set; }

    public DateTime? NewDateTime { get; private set; }
  }
}
