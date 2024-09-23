// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaStorageExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public static class MediaStorageExtensions
  {
    public static void RemoveDirectoryRecursive(
      this IMediaStorage stg,
      string path,
      bool swallowError = false,
      bool renameFirst = false,
      string renameTarget = null)
    {
      path = stg.GetFullFsPath(path);
      NativeInterfaces.Misc.RemoveDirectoryRecursive(path, swallowError, renameFirst, renameTarget);
    }
  }
}
