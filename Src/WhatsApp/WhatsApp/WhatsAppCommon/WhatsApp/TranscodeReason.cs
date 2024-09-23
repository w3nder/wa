// Decompiled with JetBrains decompiler
// Type: WhatsApp.TranscodeReason
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  [Flags]
  public enum TranscodeReason
  {
    None = 0,
    BadCodec = 1,
    TimeCrop = 2,
    FrameCrop = 4,
    MaxBitrate = 8,
    FileSize = 16, // 0x00000010
    TranscodeAudio = 32, // 0x00000020
    BadContainer = 64, // 0x00000040
  }
}
