// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWAStream
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(2724688757, 376, 20354, 151, 188, 196, 53, 189, 145, 186, 84)]
  [Version(100794368)]
  public interface IWAStream
  {
    StreamFlags GetFlags();

    long GetPosition();

    long Seek([In] long Offset, [In] uint Whence);

    long GetLength();

    void SetLength([In] long Length);

    void WriteBase([In] uint Buf, [In] int Length);

    int ReadBase([In] uint Buf, [In] int Length);

    void Write([In] IByteBuffer buf, [In] int Offset, [In] int Length);

    int Read([In] IByteBuffer buf, [In] int Offset, [In] int Length);

    void Flush();

    void Dispose();

    IWAStream Clone();
  }
}
