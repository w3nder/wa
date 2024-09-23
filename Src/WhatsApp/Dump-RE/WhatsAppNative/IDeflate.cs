// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IDeflate
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(1666043095, 31132, 18466, 154, 88, 35, 45, 169, 238, 151, 170)]
  public interface IDeflate
  {
    void Initialize([In] int level);

    void SetInputBuffer([In] IByteBuffer buf);

    void SetOutputBuffer([In] IByteBuffer buf);

    void SetInputLength([In] int Length);

    int GetOutputLength();

    void ResetOutputBuffer();

    bool Deflate([In] bool Flush);
  }
}
