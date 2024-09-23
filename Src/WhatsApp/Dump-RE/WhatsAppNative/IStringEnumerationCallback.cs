// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IStringEnumerationCallback
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(1222598201, 318, 20053, 129, 242, 13, 74, 25, 87, 5, 101)]
  public interface IStringEnumerationCallback
  {
    void OnNext([In] string String);
  }
}
