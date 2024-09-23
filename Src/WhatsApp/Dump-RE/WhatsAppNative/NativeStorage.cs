// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.NativeStorage
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  public sealed class NativeStorage : INativeMediaStorage
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IWAStream CreateFile(
      [In] string Path,
      [In] uint DesiredAccess,
      [In] uint ShareMode,
      [In] uint Disposition,
      [In] uint FlagsAndAttributes);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool FileExists([In] string Path);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void MoveFile([In] string SrcPath, [In] string DstPath);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void MoveFileWithOverwrite([In] string SrcPath, [In] string DstPath);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void DeleteFile([In] string Path);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint FindFirstFileImpl([In] string Pattern, [In] uint FindData);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool FindNextFileImpl([In] uint Handle, [In] uint FindData);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void FindCloseImpl([In] uint Handle);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IWAStream GetTempFile();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void CreateDirectory([In] string Name);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void RemoveDirectory([In] string Name);
  }
}
