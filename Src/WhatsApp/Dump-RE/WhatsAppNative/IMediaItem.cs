// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IMediaItem
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(1357267839, 12383, 18349, 153, 51, 25, 44, 158, 212, 12, 250)]
  public interface IMediaItem
  {
    uint GetIntAttribute([In] MediaItemIntegers IntId);

    string GetStringAttribute([In] MediaItemStrings StringId);

    long GetFileTimeAttribute([In] MediaItemTimes TimeId);

    MediaItemTypes GetType();

    uint GetId();

    IByteBuffer GetThumbnail();

    IByteBuffer GetFullSize();

    IMediaList GetChildren([In] MediaListTypes Type);

    void Dispose();
  }
}
