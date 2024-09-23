// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IHostResolver
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(2968338116, 14711, 19641, 173, 21, 122, 161, 205, 18, 245, 137)]
  public interface IHostResolver
  {
    string Resolve([In] string Host, [In] bool Shuffle);
  }
}
