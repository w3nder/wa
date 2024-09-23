// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IFieldStatsManaged
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(3543934736, 25051, 20281, 137, 180, 208, 239, 27, 108, 216, 179)]
  [Version(100794368)]
  public interface IFieldStatsManaged
  {
    void Invoke([In] IAction Callback);

    void Send([In] uint Flags);
  }
}
