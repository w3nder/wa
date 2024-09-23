// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IMediaList
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(2290895703, 38810, 17451, 129, 145, 250, 8, 238, 15, 106, 62)]
  [Version(100794368)]
  public interface IMediaList
  {
    int GetItemCount();

    IMediaItem GetItem([In] int Index);

    uint GetItemId([In] int Index);

    void FilterItems([In] MediaItemTypes Type);

    void Dispose();
  }
}
