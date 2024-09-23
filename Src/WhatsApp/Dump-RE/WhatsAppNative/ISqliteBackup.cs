// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ISqliteBackup
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(1630589215, 62132, 19486, 166, 240, 205, 241, 86, 113, 216, 41)]
  [Version(100794368)]
  public interface ISqliteBackup : IClosable
  {
    int GetRemainingPages();

    int GetTotalPages();

    bool Step([In] int desiredPages);
  }
}
