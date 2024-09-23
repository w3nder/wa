// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IMp4UtilsOpenCallback
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(2901538198, 17327, 18258, 174, 41, 167, 134, 1, 208, 180, 190)]
  [Version(100794368)]
  public interface IMp4UtilsOpenCallback
  {
    IWAStream Open([In] string filename);
  }
}
