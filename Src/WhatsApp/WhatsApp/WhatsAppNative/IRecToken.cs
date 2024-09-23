// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IRecToken
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(2854577577, 59274, 18564, 158, 198, 122, 243, 234, 249, 65, 98)]
  [Version(100794368)]
  public interface IRecToken
  {
    IByteBuffer GetToken([In] int TokenOffset);

    void SetSalt([In] IByteBuffer Salt);

    void Encode([In] IByteBuffer Bytes);

    void Decode([In] IByteBuffer Bytes);
  }
}
