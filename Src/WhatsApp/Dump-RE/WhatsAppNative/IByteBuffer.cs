// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IByteBuffer
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(1881050749, 32246, 19890, 141, 0, 238, 23, 3, 197, 38, 249)]
  public interface IByteBuffer
  {
    object GetImpl();

    void PutWithCopyImpl([In] object Value);

    void Reset();

    void PutZeroCopy([In] uint Buffer, [In] uint Length, [In] IAction OnDispose);

    int GetLength();

    void CopyFrom([In] IByteBuffer Buf);

    uint GetPointer();
  }
}
