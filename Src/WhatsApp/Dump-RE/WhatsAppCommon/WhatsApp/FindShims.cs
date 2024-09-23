// Decompiled with JetBrains decompiler
// Type: WhatsApp.FindShims
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Runtime.InteropServices;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class FindShims
  {
    public static IntPtr FindFirstFile(
      this INativeMediaStorage stg,
      string pattern,
      out WIN32_FIND_DATA data)
    {
      IntPtr num = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof (WIN32_FIND_DATA)));
      try
      {
        IntPtr firstFileImpl = (IntPtr) (long) stg.FindFirstFileImpl(pattern, (uint) (int) num);
        data = (WIN32_FIND_DATA) Marshal.PtrToStructure(num, typeof (WIN32_FIND_DATA));
        return firstFileImpl;
      }
      finally
      {
        Marshal.FreeCoTaskMem(num);
      }
    }

    public static bool FindNextFile(
      this INativeMediaStorage stg,
      IntPtr findHandle,
      out WIN32_FIND_DATA data)
    {
      IntPtr num1 = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof (WIN32_FIND_DATA)));
      try
      {
        int num2 = stg.FindNextFileImpl((uint) (int) findHandle, (uint) (int) num1) ? 1 : 0;
        data = (WIN32_FIND_DATA) Marshal.PtrToStructure(num1, typeof (WIN32_FIND_DATA));
        return num2 != 0;
      }
      finally
      {
        Marshal.FreeCoTaskMem(num1);
      }
    }

    public static void FindClose(this INativeMediaStorage stg, IntPtr findHandle)
    {
      stg.FindCloseImpl((uint) (int) findHandle);
    }

    public static bool IsDirectory(this WIN32_FIND_DATA data) => (data.dwFileAttributes & 16U) > 0U;
  }
}
