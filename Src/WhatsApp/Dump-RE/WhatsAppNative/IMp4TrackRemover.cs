// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IMp4TrackRemover
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(992591491, 33318, 16422, 135, 62, 204, 91, 5, 50, 30, 60)]
  public interface IMp4TrackRemover
  {
    int GetTrackCount();

    string GetTrackDescription([In] int Index);

    int GetTrackId([In] int Index);

    void RemoveTracks([In] int[] Ids, [In] string OutputFilename);
  }
}
