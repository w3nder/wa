// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ShellButtonPressEvent
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  public enum ShellButtonPressEvent
  {
    KeyDown = 0,
    KeyPress = 1,
    DoubleTap = 2,
    TripleTap = 3,
    PressAndHold = 4,
    PressAndHoldPulse = 5,
    PressAndHoldRelease = 6,
    Cancel = 7,
    ForceSize = 2147483647, // 0x7FFFFFFF
  }
}
