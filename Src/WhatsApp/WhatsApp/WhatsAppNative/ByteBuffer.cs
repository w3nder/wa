// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ByteBuffer
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class ByteBuffer : IByteBuffer
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern ByteBuffer();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern object GetImpl();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void PutWithCopyImpl([In] object Value);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Reset();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void PutZeroCopy([In] uint Buffer, [In] uint Length, [In] IAction OnDispose);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetLength();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void CopyFrom([In] IByteBuffer Buf);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetPointer();
  }
}
