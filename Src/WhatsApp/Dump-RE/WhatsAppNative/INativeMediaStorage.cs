// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.INativeMediaStorage
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(217169647, 22082, 17551, 181, 28, 130, 254, 250, 95, 23, 45)]
  public interface INativeMediaStorage
  {
    IWAStream CreateFile(
      [In] string Path,
      [In] uint DesiredAccess,
      [In] uint ShareMode,
      [In] uint Disposition,
      [In] uint FlagsAndAttributes);

    bool FileExists([In] string Path);

    void MoveFile([In] string SrcPath, [In] string DstPath);

    void MoveFileWithOverwrite([In] string SrcPath, [In] string DstPath);

    void DeleteFile([In] string Path);

    uint FindFirstFileImpl([In] string Pattern, [In] uint FindData);

    bool FindNextFileImpl([In] uint Handle, [In] uint FindData);

    void FindCloseImpl([In] uint Handle);

    IWAStream GetTempFile();

    void CreateDirectory([In] string Name);

    void RemoveDirectory([In] string Name);
  }
}
