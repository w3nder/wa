// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IRegex
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(883451922, 25663, 19512, 159, 161, 255, 207, 185, 16, 125, 11)]
  [Version(100794368)]
  public interface IRegex
  {
    void Initialize([In] string Pattern, [In] RegexOptions Options);

    IMatch GetMatch([In] string Input, [In] int Offset, [In] int Length);

    void Dispose();
  }
}
