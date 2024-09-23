// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IMatch
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(48111588, 25784, 17431, 163, 194, 222, 6, 113, 138, 240, 0)]
  [Version(100794368)]
  public interface IMatch
  {
    Range GetRange();

    uint GetGroupCount();

    Range GetGroup([In] int Idx);

    void Dispose();
  }
}
