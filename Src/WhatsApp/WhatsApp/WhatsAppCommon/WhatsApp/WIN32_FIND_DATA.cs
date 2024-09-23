// Decompiled with JetBrains decompiler
// Type: WhatsApp.WIN32_FIND_DATA
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Runtime.InteropServices;


namespace WhatsApp
{
  [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
  public struct WIN32_FIND_DATA
  {
    [FieldOffset(0)]
    public uint dwFileAttributes;
    [FieldOffset(4)]
    public long ftCreationTime;
    [FieldOffset(12)]
    public long ftLastAccessTime;
    [FieldOffset(20)]
    public long ftLastWriteTime;
    [FieldOffset(28)]
    public uint nFileSizeHigh;
    [FieldOffset(32)]
    public uint nFileSizeLow;
    [FieldOffset(36)]
    public uint dwReserved0;
    [FieldOffset(40)]
    public uint dwReserved1;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    [FieldOffset(44)]
    public string cFileName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
    [FieldOffset(564)]
    public string cAlternateFileName;
  }
}
