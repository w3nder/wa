// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IMediaLibrary
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(3402427968, 37884, 17395, 149, 199, 19, 123, 173, 38, 183, 208)]
  public interface IMediaLibrary
  {
    IMediaList CreateList([In] MediaListTypes Type, [In] uint Parent);

    IMediaItem GetItemById([In] uint Id);

    IMediaItem GetSpecialFolder([In] uint Type);

    void SuppressNotification([In] ZMediaNotificationType Type, [In] string Path);

    void ForceInsert([In] string Path);
  }
}
