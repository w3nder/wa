// Decompiled with JetBrains decompiler
// Type: WhatsApp.Streaming.Mp4CannotStreamException
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp.Streaming
{
  public class Mp4CannotStreamException : Exception
  {
    public Mp4CannotStreamException(string message, Exception innerException = null)
      : base(message, innerException)
    {
    }
  }
}
