// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ICurve25519
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(2374934321, 25345, 16959, 163, 37, 49, 159, 148, 36, 40, 236)]
  public interface ICurve25519
  {
    void GenKeyPair(out IByteBuffer PubKey, out IByteBuffer PrivKey);

    IByteBuffer Derive([In] IByteBuffer PubKey, [In] IByteBuffer PrivKey);

    IByteBuffer Sign([In] IByteBuffer Message, [In] IByteBuffer SignKey);

    bool Verify([In] IByteBuffer Message, [In] IByteBuffer Signature, [In] IByteBuffer SignKey);
  }
}
