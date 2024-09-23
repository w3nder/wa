// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IAxolotlSessionCipherCallbacks
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(26241064, 59270, 18692, 190, 171, 139, 144, 69, 39, 244, 149)]
  [Version(100794368)]
  public interface IAxolotlSessionCipherCallbacks
  {
    void DecryptCallback([In] IByteBuffer PlainTextBuffer);
  }
}
