// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IOutOfProcRegistration
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(813379938, 32226, 17388, 130, 166, 55, 2, 252, 130, 20, 193)]
  [Version(100794368)]
  public interface IOutOfProcRegistration
  {
    void Register([In] string ClassName);

    void Dispose();
  }
}
