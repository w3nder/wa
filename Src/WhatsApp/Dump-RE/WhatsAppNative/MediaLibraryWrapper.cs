// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.MediaLibraryWrapper
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Activatable(100794368)]
  [MarshalingBehavior]
  public sealed class MediaLibraryWrapper : IMediaLibrary
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern MediaLibraryWrapper();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IMediaList CreateList([In] MediaListTypes Type, [In] uint Parent);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IMediaItem GetItemById([In] uint Id);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IMediaItem GetSpecialFolder([In] uint Type);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SuppressNotification([In] ZMediaNotificationType Type, [In] string Path);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void ForceInsert([In] string Path);
  }
}
