// Decompiled with JetBrains decompiler
// Type: WhatsApp.SerializeStruct
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Runtime.InteropServices;


namespace WhatsApp
{
  public static class SerializeStruct
  {
    public static int SizeOf<T>() where T : struct => Marshal.SizeOf(typeof (T));

    public static T Read<T>(byte[] buffer, int offset, int len) where T : struct
    {
      IntPtr num = Marshal.AllocCoTaskMem(SerializeStruct.SizeOf<T>());
      try
      {
        Marshal.Copy(buffer, offset, num, len);
        return (T) Marshal.PtrToStructure(num, typeof (T));
      }
      finally
      {
        Marshal.FreeCoTaskMem(num);
      }
    }

    public static void Write<T>(T s, byte[] buffer, int offset, int len)
    {
      GCHandle gcHandle = GCHandle.Alloc((object) buffer, GCHandleType.Pinned);
      try
      {
        Marshal.StructureToPtr((object) s, gcHandle.AddrOfPinnedObject() + offset, false);
      }
      finally
      {
        gcHandle.Free();
      }
    }

    public static void Write<T>(T s, MemoryStream stream) where T : struct
    {
      long position = stream.Position;
      int len = SerializeStruct.SizeOf<T>();
      if (stream.Length < position + (long) len)
        stream.SetLength(position + (long) len);
      SerializeStruct.Write<T>(s, stream.GetBuffer(), (int) position, len);
      stream.Position = position + (long) len;
    }
  }
}
