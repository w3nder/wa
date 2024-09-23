// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ITranscoderProgress
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(1058469486, 39399, 18084, 180, 199, 62, 102, 250, 128, 141, 100)]
  [Version(100794368)]
  public interface ITranscoderProgress
  {
    void OnProgress([In] int Percentage);
  }
}
