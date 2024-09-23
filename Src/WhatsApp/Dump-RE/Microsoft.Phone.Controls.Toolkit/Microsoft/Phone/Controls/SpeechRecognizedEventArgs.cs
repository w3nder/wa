// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.SpeechRecognizedEventArgs
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using Windows.Phone.Speech.Recognition;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public sealed class SpeechRecognizedEventArgs : EventArgs
  {
    public SpeechRecognizedEventArgs(SpeechRecognitionResult result)
    {
      this.Result = result;
      this.Canceled = false;
    }

    public SpeechRecognitionResult Result { get; private set; }

    public bool Canceled { get; set; }
  }
}
