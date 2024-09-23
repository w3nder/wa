// Decompiled with JetBrains decompiler
// Type: WhatsApp.ByteBufferShim
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Runtime.InteropServices;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class ByteBufferShim
  {
    public static byte[] Get(this IByteBuffer bb) => (byte[]) bb.GetImpl();

    public static void PutWithCopy(this IByteBuffer bb, byte[] buf)
    {
      bb.PutWithCopyImpl((object) buf);
    }

    public static void Put(this IByteBuffer bb, byte[] bytes) => bb.Put(bytes, 0, bytes.Length);

    public static void Put(this IByteBuffer bb, byte[] bytes, int offset, int length)
    {
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      if (offset < 0 || offset > bytes.Length || length < 0 || offset + length > bytes.Length)
        throw new IndexOutOfRangeException(string.Format("offset: {0}, length: {1}, buffer length: {2}", (object) offset, (object) length, (object) bytes.Length));
      GCHandle handle = GCHandle.Alloc((object) bytes, GCHandleType.Pinned);
      uint Buffer = (uint) (int) (handle.AddrOfPinnedObject() + offset);
      Action a = (Action) (() => handle.Free());
      bb.PutZeroCopy(Buffer, (uint) length, a.AsComAction());
    }
  }
}
